#include "stdafx.h"
#include <iphlpapi.h>
#include "AuditDataFile.h"
#include "GraphicsAdaptersScanner.h"


//////////////////////////////////////////////////////////////////////////////
//
// Windows Domain / Workgroup registry keys and values
//
// Windows 9X
#define WIN_DISPLAY_KEY							"SYSTEM\\CurrentControlSet\\Services\\Class\\Display"
#define WIN_ADAPTER_NAME_VALUE					"DriverDesc"
#define WIN_DEVICE_INFO_KEY						"INFO"
#define WIN_ADAPTER_CHIP_VALUE					"ChipType"
#define WIN_ADAPTER_MEMORY_VALUE				"VideoMemory"

// Windows NT
#define NT_ENUM_KEY								"SYSTEM\\CurrentControlSet\\Enum"
#define NT_ENUM_CLASS_VALUE						"Class"
#define NT_ENUM_DISPLAY_VALUE					"Display"
#define NT_ENUM_SERVICE_VALUE					"Service"
#define NT_SERVICES_KEY							"SYSTEM\\CurrentControlSet\\Services"
#define NT_SERVICE_DEVICE_KEY					"Device0"
#define NT_ADAPTER_NAME_VALUE					"HardwareInformation.AdapterString"
#define NT_ADAPTER_CHIP_VALUE					"HardwareInformation.ChipType"
#define NT_ADAPTER_MEMORY_VALUE					"HardwareInformation.MemorySize"

// Windows XP
#define XP_ENUM_KEY								"SYSTEM\\CurrentControlSet\\Control\\Video"
#define XP_ADAPTER_NAME_VALUE					"HardwareInformation.AdapterString"
#define XP_ADAPTER_CHIP_VALUE					"HardwareInformation.ChipType"
#define XP_ADAPTER_MEMORY_VALUE					"HardwareInformation.MemorySize"

// Storage Strings
#define HARDWARE_CLASS							"Hardware|Adapters|Display"
#define	V_GRAPHICSADAPTER_NAME					"Name"
#define V_GRAPHICSADAPTER_CHIPSET				"Chipset"
#define V_GRAPHICSADAPTER_MEMORY				"Memory"
#define V_GRAPHICSADAPTER_RESOLUTION			"Resolution"
#define V_GRAPHICSADAPTER_REFRESH				"Screen Refresh Rate"
#define V_GRAPHICSADAPTER_DRIVERVERSION			"Driver Version"
#define V_GRAPHICSADAPTER_MONITORTYPE			"Monitor Type"

///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CGraphicsAdapter Class
//
CGraphicsAdapter::CGraphicsAdapter()
{
	// Initialize Data
	_adapterName = UNKNOWN;
	_chipset = UNKNOWN;
	_memory = UNKNOWN;
	_driverVersion = UNKNOWN;
}


///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CGraphicsAdaptersScanner Class
//
CGraphicsAdaptersScanner::CGraphicsAdaptersScanner()
{
	// Define the path to the items within this audited hardware data category
	m_strItemPath = HARDWARE_CLASS;
}

CGraphicsAdaptersScanner::~CGraphicsAdaptersScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool CGraphicsAdaptersScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CGraphicsAdaptersScanner::ScanWMI Start" ,true);

	// Ensure that the list is empty
	_listGraphicsAdapters.Empty();

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	try
	{
		CString		strBuffer;
		LPCTSTR		pDataValue = NULL;

		// We need to enumerate through the network adapters...
		// Note that Win32_GraphicsAdapterSetting actually combines the Win32_GraphicsAdapter and the
		// Win32_GraphicsAdapterConfiguration classes 
		if (wmiConnection.BeginEnumClassObject( _T( "Win32_VideoController")))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Create an empty graphics adapter object
				CGraphicsAdapter graphicsAdapter;

				// ...and fill it in - we start with the MAC address as if this is blank then we will skip
				// this display adapter
				strBuffer = wmiConnection.GetClassObjectStringValue("Description");
				graphicsAdapter.AdapterName(strBuffer);
				//
				strBuffer = wmiConnection.GetClassObjectStringValue("VideoProcessor");
				graphicsAdapter.Chipset(strBuffer);
				//
				DWORD memory = wmiConnection.GetClassObjectDwordValue("AdapterRAM");
				memory /= ONE_MEGABYTE;
				strBuffer.Format("%lu", memory);
				graphicsAdapter.Memory(strBuffer);
				//
				strBuffer = wmiConnection.GetClassObjectStringValue("DriverVersion");
				graphicsAdapter.DriverVersion(strBuffer);
				//
				graphicsAdapter.RefreshRate(wmiConnection.GetClassObjectDwordValue("CurrentRefreshRate"));
				//
				CString horizontalResolution = wmiConnection.GetClassObjectStringValue("CurrentHorizontalResolution");
				CString verticalResolution = wmiConnection.GetClassObjectStringValue("CurrentVerticalResolution");
				
				if (horizontalResolution == "" || verticalResolution == "")
				{
					graphicsAdapter.Resolution("");
				}
				else
				{
					strBuffer = horizontalResolution + "x" + verticalResolution;
					graphicsAdapter.Resolution(strBuffer);
				}

				if (wmiConnection.BeginEnumClassObject("Win32_DesktopMonitor"))
				{
					// Loop through - we only list details for the first processor but we do need to get
					// the count of processors in this system
					while (wmiConnection.MoveNextEnumClassObject())
					{				
						strBuffer = wmiConnection.GetClassObjectStringValue("MonitorType");
						graphicsAdapter.MonitorType(strBuffer);
					}
					wmiConnection.CloseEnumClassObject();
				}

				// add this to our list
				_listGraphicsAdapters.Add(graphicsAdapter);
			}
			wmiConnection.CloseEnumClassObject();
		}		
	}
	catch (CException *pEx)
	{
		return false;
	}

	log.Write("CGraphicsAdaptersScanner::ScanWMI End" ,true);
	return true;
}



//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows NT / 2000
//
bool	CGraphicsAdaptersScanner::ScanRegistryNT()
{
	HKEY		hKeyEnum;
	HKEY		hKeyGroup;
	HKEY		hKeyProperty;
	HKEY		hKeyObject;
	TCHAR		szBuffer[256];
	TCHAR		szGroupName[256];
	TCHAR		szDeviceName[256];
	TCHAR		szPropertyName[256];

	DWORD		dwLength;
	DWORD		dwType;
	DWORD		dwMemory;
	DWORD		dwIndexEnum = 0;
	DWORD		dwIndexGroup = 0;
	LONG		lResult;
	BOOL		bStore;

	// Data Collected by this function
	CString		strSubKeyName;
	CString		strAdapterName = UNKNOWN;
	CString		strChipset = UNKNOWN;
	CString		strMemory = UNKNOWN;
	CString		strDriverVersion = UNKNOWN;

	//
	CLogFile log;
	log.Write("CGraphicsAdaptersScanner::ScanRegistryNT Start" ,true);

	try
	{

		// Ensure that the list is empty
		_listGraphicsAdapters.Empty();

		// Open the main display key so that we can list the adapters contained therein
		if (RegOpenKeyEx(m_hKey, NT_ENUM_KEY, 0, KEY_READ, &hKeyEnum) == ERROR_SUCCESS)
		{
			// Now enumerate through the sub-keys looking for display adapters
			dwLength = 255;
			while ((lResult = RegEnumKeyEx(hKeyEnum, dwIndexEnum, szGroupName, &dwLength, NULL, NULL, NULL, NULL)) == ERROR_SUCCESS)
			{
				// For each group, enumerate device keys
				szGroupName[dwLength] = 0;
				strSubKeyName.Format("%s\\%s", NT_ENUM_KEY, szGroupName);
				if (RegOpenKeyEx( m_hKey, strSubKeyName, 0, KEY_READ, &hKeyGroup) == ERROR_SUCCESS)
				{
					dwLength = 255;
					dwIndexGroup = 0;
					while ((lResult = RegEnumKeyEx(hKeyGroup, dwIndexGroup, szDeviceName, &dwLength, NULL, NULL, NULL, NULL)) == ERROR_SUCCESS)
					{
						// For each device, get properties keys
						szDeviceName[dwLength] = 0;
						strSubKeyName.Format("%s\\%s\\%s", NT_ENUM_KEY, szGroupName, szDeviceName);
						if (RegOpenKeyEx(m_hKey, strSubKeyName, 0, KEY_READ, &hKeyObject) == ERROR_SUCCESS)
						{
							dwLength = 255;
							DWORD dwIndexProperties = 0;

							while ((lResult = RegEnumKeyEx(hKeyObject, dwIndexProperties, szPropertyName, &dwLength, NULL, NULL, NULL, NULL)) == ERROR_SUCCESS)
							{
								// If a display class key, read the associated service
								szPropertyName[dwLength] = 0;
								strSubKeyName.Format("%s\\%s\\%s\\%s", NT_ENUM_KEY, szGroupName, szDeviceName, szPropertyName);
								if (RegOpenKeyEx( m_hKey, strSubKeyName, 0, KEY_READ, &hKeyProperty) == ERROR_SUCCESS)
								{
									// Read the class
									dwLength = 255;
									if (RegQueryValueEx(hKeyProperty, NT_ENUM_CLASS_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) != ERROR_SUCCESS)
									{
										// Cannot read the class name so tidy up
										RegCloseKey(hKeyProperty);
										dwIndexProperties++;
										dwLength = 255;
										continue;
									}

									// Is the value a display service?
									szBuffer[dwLength] = 0;
									CString strClassName = szBuffer;
									if (strClassName.Compare(NT_ENUM_DISPLAY_VALUE) != 0)
									{
										// Not a display service
										dwIndexProperties++;
										dwLength = 255;
										continue;
									}


									// We have found the display service => get the service name
									dwLength = 255;
									if (RegQueryValueEx( hKeyProperty, NT_ENUM_SERVICE_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) != ERROR_SUCCESS)
									{
										// Cannot read the associated service name so ignore this entry
										RegCloseKey( hKeyProperty);
										dwIndexProperties ++;
										dwLength = 255;
										continue;
									}

									// We know the service name so get the Graphics Adapter information for this service
									szBuffer[dwLength] = 0;
									RegCloseKey(hKeyProperty);
									strSubKeyName.Format("%s\\%s\\%s", NT_SERVICES_KEY, szBuffer, NT_SERVICE_DEVICE_KEY);
									
									// Reset the display adapter detail fields
									strAdapterName = UNKNOWN;
									strChipset = UNKNOWN;
									strMemory = UNKNOWN;
									strDriverVersion = UNKNOWN;

									// ...and open the subkey based on the server name
									bStore = FALSE;
									if (RegOpenKeyEx(m_hKey, strSubKeyName, 0, KEY_READ, &hKeyProperty) == ERROR_SUCCESS)
									{
										dwLength = 255;

										// Get the Graphic Adapter name
										if (RegQueryValueEx( hKeyProperty, NT_ADAPTER_NAME_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) == ERROR_SUCCESS)
										{
											szBuffer[dwLength] = 0;
											strAdapterName.Format("%S" ,szBuffer);
											bStore = TRUE;
										}

										// Get the Graphic Adapter Chipset
										dwLength = 255;
										if (RegQueryValueEx(hKeyProperty, NT_ADAPTER_CHIP_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) == ERROR_SUCCESS)
										{
											szBuffer[dwLength] = 0;
											strChipset.Format("%S" ,szBuffer);
											bStore = TRUE;
										}

										// Get the Graphic Adapter Memory size
										dwLength = sizeof(DWORD);
										dwType = REG_DWORD;
										if (RegQueryValueEx(hKeyProperty, NT_ADAPTER_MEMORY_VALUE, 0, &dwType, (LPBYTE)&dwMemory, &dwLength) == ERROR_SUCCESS)
										{
											strMemory.Format("%s" ,dwMemory/ONE_MEGABYTE);
										}

										// If we have recovered an adapter then create an object and store it
										if (bStore)
										{
											CGraphicsAdapter adapter(strAdapterName , strChipset ,strMemory ,strDriverVersion);
											adapter.AdapterName(strAdapterName);
											_listGraphicsAdapters.Add(adapter);
										}

										// Close the properties key
										RegCloseKey( hKeyProperty);
									}
								} 

								// Increment the index and reset working variables
								dwIndexProperties++;
								dwLength = 255;
							} 

							// Close the Object key
							RegCloseKey( hKeyObject);
						}
						dwIndexGroup++;
						dwLength = 255;
					}
				}

				// Close this group and skip to the next
				RegCloseKey( hKeyGroup);
				dwIndexEnum++;
				dwLength = 255;
			}

			// close the main key now as we have finished with it
			RegCloseKey( hKeyEnum);
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CAuditDataScanner::Scan\n");
		log.Write("CGraphicsAdaptersScanner::ScanRegistryNT End" ,true);
		return FALSE;
	}

	// return
	log.Write("CGraphicsAdaptersScanner::ScanRegistryNT End" ,true);
	return TRUE;
}




//
//    ScanRegistryXP
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
bool	CGraphicsAdaptersScanner::ScanRegistryXP()
{
	HKEY		hKeyEnum;
	HKEY		hKeyGroup;
	HKEY		hKeyProperty;
	TCHAR		szBuffer[256];
	TCHAR		szGroupName[256];

	DWORD		dwLength;
	DWORD		dwType;
	DWORD		dwMemory;
	DWORD		dwIndexEnum = 0;
	DWORD		dwIndexGroup = 0;
	LONG		lResult;
	BOOL		bStore;

	// Data Collected by this function
	CString		strSubKeyName;
	CString		strAdapterName = UNKNOWN;
	CString		strChipset = UNKNOWN;
	CString		strMemory = UNKNOWN;
	CString		strDriverVersion = UNKNOWN;
	//
	CLogFile log;
	log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Start" ,true);

	try
	{
		// Ensure that the list is empty
		_listGraphicsAdapters.Empty();

		// Open the main display key so that we can list the adapters contained therein
		if (RegOpenKeyEx(m_hKey, XP_ENUM_KEY, 0, KEY_READ, &hKeyEnum) == ERROR_SUCCESS)
		{
			// Now enumerate through the sub-keys looking for display adapters
			dwLength = 255;
			while ((lResult = RegEnumKeyEx(hKeyEnum, dwIndexEnum, szGroupName, &dwLength, NULL, NULL, NULL, NULL)) == ERROR_SUCCESS)
			{
				// For each group, enumerate device keys
				szGroupName[dwLength] = 0;
				strSubKeyName.Format("%s\\%s", XP_ENUM_KEY, szGroupName);
				if (RegOpenKeyEx( m_hKey, strSubKeyName, 0, KEY_READ, &hKeyGroup) == ERROR_SUCCESS)
				{
					dwLength = 255;
					dwIndexGroup = 0;
					while ((lResult = RegEnumKeyEx(hKeyGroup, dwIndexGroup, szBuffer, &dwLength, NULL, NULL, NULL, NULL)) == ERROR_SUCCESS)
					{
						// For each device, get properties keys
						szBuffer[dwLength] = 0;
						strSubKeyName.Format("%s\\%s\\%s", XP_ENUM_KEY, szGroupName, szBuffer);
						if (RegOpenKeyEx(m_hKey, strSubKeyName, 0, KEY_READ, &hKeyProperty) == ERROR_SUCCESS)
						{
		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Step 6" ,true);
							dwLength = 255;
							bStore = FALSE;
		
							// Get the Display Adapter name
							if (RegQueryValueEx(hKeyProperty, XP_ADAPTER_NAME_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) == ERROR_SUCCESS)
							{
		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Step 7" ,true);
								szBuffer[dwLength] = 0;
								strAdapterName.Format("%S", szBuffer);
								bStore = TRUE;
							}
		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Step 8" ,true);

							// Get the Chipset for the Graphics Adapter
							dwLength = 255;
							if (RegQueryValueEx(hKeyProperty, XP_ADAPTER_CHIP_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) == ERROR_SUCCESS)
							{
		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Step 9" ,true);
								szBuffer[dwLength] = 0;
								strChipset.Format("%S", szBuffer);
								bStore = TRUE;
							}

		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Step 10" ,true);
							// Get the Graphic Adapter Memory size
							dwLength = sizeof(DWORD);
							dwType = REG_DWORD;
							if (RegQueryValueEx(hKeyProperty, XP_ADAPTER_MEMORY_VALUE, 0, &dwType, (LPBYTE)&dwMemory, &dwLength) == ERROR_SUCCESS)
							{
		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP Step 11" ,true);
								strMemory.Format("%s" ,dwMemory/ONE_MEGABYTE);
							}

							// If we have recovered an adapter then create an object and store it
							if (bStore)
							{
								CGraphicsAdapter adapter(strAdapterName , strChipset ,strMemory ,strDriverVersion);
								adapter.AdapterName(strAdapterName);
								_listGraphicsAdapters.Add(adapter);
							}

							// Close the property sub-key again
							RegCloseKey(hKeyProperty);
						}

						// Next group...
						dwIndexGroup++;
						dwLength = 255;
					} 			
				}

				// Close this group and skip to the next
				RegCloseKey( hKeyGroup);
				dwIndexEnum++;
				dwLength = 255;
			}

			// close the main key now as we have finished with it
			RegCloseKey( hKeyEnum);
		}
	}
	catch (CException *pEx)
	{
		log.Write("CGraphicsAdaptersScanner::ScanRegistryXP EXCEPTION HANDLER" ,true);
		throw pEx;
	}

	// return
	return TRUE;
}




//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows 9X
//
bool	CGraphicsAdaptersScanner::ScanRegistry9X()
{
	HKEY			hKeyEnum;
	HKEY			hKeyObject;
	HKEY			hKeyProperty;
	CString			strSubKey;
	TCHAR			szBuffer[256];
	DWORD			dwLength;
	DWORD			dwType;
	DWORD			dwMemory;
	DWORD			dwIndexEnum = 0;
	LONG			lResult;
	BOOL			bStore;

	// Data Collected by this function
	CString		strSubKeyName;
	CString		strAdapterName = UNKNOWN;
	CString		strChipset = UNKNOWN;
	CString		strMemory = UNKNOWN;
	CString		strDriverVersion = UNKNOWN;

	try
	{
		// Ensure that the list is empty
		_listGraphicsAdapters.Empty();

		// Windows 9X => Open the Display key
		if (RegOpenKeyEx(m_hKey, WIN_DISPLAY_KEY, 0, KEY_READ, &hKeyEnum) == ERROR_SUCCESS)
		{
			// Enum the devices subkeys to find devices (have an INFO value)
			dwLength = 255;
			while ((lResult = RegEnumKeyEx( hKeyEnum, dwIndexEnum, szBuffer, &dwLength, 0, NULL, 0, NULL)) == ERROR_SUCCESS)
			{
				// For each object, Try to open the device key
				szBuffer[dwLength] = 0;
				bStore = FALSE;
				strSubKey.Format("%s\\%s", WIN_DISPLAY_KEY, szBuffer);

				// Open the display key
				if (RegOpenKeyEx( m_hKey, strSubKey, 0, KEY_READ, &hKeyObject) == ERROR_SUCCESS)
				{
					// OK => Read the Graphic Adapter description
					dwLength = 255;
					if (RegQueryValueEx( hKeyObject, WIN_ADAPTER_NAME_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) == ERROR_SUCCESS)
					{
						szBuffer[dwLength]=0;
						strAdapterName.Format("%S", szBuffer);
						bStore = TRUE;
					}

					// Open the INFO key from Display Device
					strSubKey.Format("%s\\%s\\%s", WIN_DISPLAY_KEY, szBuffer, WIN_DEVICE_INFO_KEY);
					if (RegOpenKeyEx(m_hKey, strSubKey, 0, KEY_READ, &hKeyProperty) == ERROR_SUCCESS)
					{
						// Read the Graphic Adapter Chipset
						dwLength = 255;
						if (RegQueryValueEx( hKeyProperty, WIN_ADAPTER_CHIP_VALUE, 0, &dwType, (LPBYTE)szBuffer, &dwLength) == ERROR_SUCCESS)
						{
							szBuffer[dwLength] = 0;
							strChipset.Format("%S", szBuffer);
							bStore = TRUE;
						}

						// Read the Graphic Adapter Memory size
						dwLength = sizeof(DWORD);
						dwType = REG_DWORD;
						if (RegQueryValueEx(hKeyProperty, WIN_ADAPTER_MEMORY_VALUE, 0, &dwType, (LPBYTE) &dwMemory, &dwLength) == ERROR_SUCCESS)
							strMemory.Format("%s" ,dwMemory/ONE_MEGABYTE);

						// Close the adapter properties key
						RegCloseKey( hKeyProperty);
					}
					RegCloseKey( hKeyObject);
					
					// Add the device to the adapter list
					if (bStore)
					{
						CGraphicsAdapter adapter(strAdapterName , strChipset ,strMemory ,strDriverVersion);
						_listGraphicsAdapters.Add(adapter);
					}
				}

				// Enum the next device
				dwLength = 255;
				dwIndexEnum++;
			}
			RegCloseKey( hKeyEnum);
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	return true;
}


//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CGraphicsAdaptersScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CGraphicsAdaptersScanner::SaveData Start" ,true);
	CString categoryName;

	// OK on entry we will have started the Hardware Category so we do not need to worry about
	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listGraphicsAdapters.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		// and iterate through the audited graphics adapters - we create a CAuditDataFileCategory for each adapter
		// The items within the category are the values recovered for each adapter (name, chipset, memory)
		for (int isub=0; isub<(int)_listGraphicsAdapters.GetCount(); isub++)
		{
			// Format the hardware class name for this graphics adapter
			CGraphicsAdapter* pAdapter= &_listGraphicsAdapters[isub];
			categoryName.Format("%s|%s" ,HARDWARE_CLASS ,pAdapter->AdapterName());
			//categoryName.Format("%s|Display Adapter (%d)" ,HARDWARE_CLASS ,isub);

			// Each Adapter has its own category
			CAuditDataFileCategory category(categoryName);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem adapter(V_GRAPHICSADAPTER_NAME ,pAdapter->AdapterName());
			CAuditDataFileItem chipset(V_GRAPHICSADAPTER_CHIPSET ,pAdapter->Chipset());
			CAuditDataFileItem memory(V_GRAPHICSADAPTER_MEMORY ,pAdapter->Memory(), " MB");
			CAuditDataFileItem resolution(V_GRAPHICSADAPTER_RESOLUTION ,pAdapter->Resolution());
			CAuditDataFileItem refreshrate(V_GRAPHICSADAPTER_REFRESH ,pAdapter->RefreshRate(), " Hz");
			CAuditDataFileItem driverVersion(V_GRAPHICSADAPTER_DRIVERVERSION ,pAdapter->DriverVersion());
			CAuditDataFileItem monitorType(V_GRAPHICSADAPTER_MONITORTYPE ,pAdapter->MonitorType());

			// Add the items to the category
			category.AddItem(adapter);
			category.AddItem(chipset);
			category.AddItem(memory);
			category.AddItem(resolution);
			category.AddItem(refreshrate);
			category.AddItem(driverVersion);
			category.AddItem(monitorType);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CGraphicsAdaptersScanner::SaveData End" ,true);
	return true;
}
