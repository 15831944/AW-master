#include "stdafx.h"
#include <iphlpapi.h>
#include "AuditDataFile.h"
#include "NetworkAdaptersScanner.h"


// Storage Strings
#define HARDWARE_CLASS					"Hardware|Adapters|Network Adapters"
#define V_NETWORKADAPTER_NAME			"Name"
#define V_NETWORKADAPTER_MANUFACTURER	"Manufacturer"
#define V_NETWORKADAPTER_MACADDRESS		"MAC Address"
#define V_NETWORKADAPTER_IPADDRESS		"IP Address"
#define V_NETWORKADAPTER_TYPE			"Type"				
#define V_NETWORKADAPTER_SPEED			"Speed"

///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CNetworkAdapter Class
//
CNetworkAdapter::CNetworkAdapter()
{
	_productName = UNKNOWN;
	_manufacturer = UNKNOWN;
	_macAddress = UNKNOWN;
	_ipAddress = UNKNOWN;
	_type = UNKNOWN;
	_speed = 0;
}


///////////////////////////////////////////////////////////////////////////////////////////////
//
//    CNetworkAdaptersScanner Class
//
CNetworkAdaptersScanner::CNetworkAdaptersScanner()
{
	m_strItemPath = HARDWARE_CLASS;
}

CNetworkAdaptersScanner::~CNetworkAdaptersScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool CNetworkAdaptersScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CNetworkAdaptersScanner::ScanWMI Start" ,true);

	bool returnStatus = true;

	try
	{
		// Get the WMI object itself
		CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

		// Ensure that the list is empty
		_listNetworkAdapters.Empty();
	
		CString		strBuffer;
		CString		strMacAddress;
		LPCTSTR		pDataValue = NULL;
		CLogFile log;

		// We need to enumerate through the network adapters...
		// Note that Win32_NetworkAdapterSetting actually combines the Win32_NetworkAdapter and the
		// Win32_NetworkAdapterConfiguration classes 
		if (wmiConnection.BeginEnumClassObject( _T( "Win32_NetworkAdapterSetting")))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				// Create an empty network adapter object
				CNetworkAdapter networkAdapter;

				// ...and fill it in - we start with the MAC address as if this is blank then we will skip
				// this network adapter
				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Setting", "MACAddress");
				if (pDataValue != NULL)
					networkAdapter.MacAddress((CString)pDataValue);
				if (networkAdapter.MacAddress() == "")
					continue;
			
				// Get the connection status as we only want connected devices - this should discard MOST 
				// devices which are not actual NIC cards.
				DWORD dwConnectionStatus = wmiConnection.GetRefElementClassObjectDwordValue("Element", "NetConnectionStatus");
				if (dwConnectionStatus != 2)
						continue;
								
				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Element", "AdapterType");
				if (pDataValue != NULL)
					networkAdapter.Type((CString)pDataValue);

				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Element", "ProductName");
				if (pDataValue != NULL)
					networkAdapter.ProductName((CString)pDataValue);

				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Element", "Manufacturer");
				if (pDataValue != NULL)
					networkAdapter.Manufacturer((CString)pDataValue);

				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Setting", "MACAddress");
				if (pDataValue != NULL)
					networkAdapter.MacAddress((CString)pDataValue);

				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Setting", "IPAddress");
				if (pDataValue != NULL)
				{
					// need to handle IPv6 here, as it will have come back in this format:
					// 192.168.0.122;fe80::cd54:2994:4023:1faa
					// we just want the first section before the semi-colon
					CString strIPAddress = (CString)pDataValue;	

					if (strIPAddress.Find( ';' ) > 0)
					{
						strIPAddress = strIPAddress.Left(strIPAddress.Find( ';' ));
					}
					networkAdapter.IPAddress(strIPAddress);
				}

				pDataValue = wmiConnection.GetRefElementClassObjectStringValue("Setting", "IPSubnet");
				if (pDataValue != NULL)
					networkAdapter.IPSubNet((CString)pDataValue);

				networkAdapter.DHCPEnabled(wmiConnection.GetRefElementClassObjectDwordValue("Setting", "IPSubnet") != 0);

				// add this to our list
				CString msg;
				msg.Format("WMI Detected Network Adapter %s" ,networkAdapter.ProductName());
				log.Write(msg);
				
				_listNetworkAdapters.Add(networkAdapter);
			}
			wmiConnection.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CNetworkAdaptersScanner::ScanWMI");
		return false;
	}

	log.Write("CNetworkAdaptersScanner::ScanWMI End" ,true);
	return returnStatus;
}



//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows NT/2000 or higher
//
bool	CNetworkAdaptersScanner::ScanRegistryNT()
{
	// Network adapter information is not recovered from the registry so use a common routine
	return ScanRegistryXP();
}


//
//    ScanRegistryXP
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
bool	CNetworkAdaptersScanner::ScanRegistryXP()
{
	// Network adapter information is not recovered from the registry so use a common routine
	return GetNetworkAdapters();
}



//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows 9X
//
bool	CNetworkAdaptersScanner::ScanRegistry9X()
{
	// Network adapter information is not recovered from the registry so use a common routine
	return GetNetworkAdapters();
}


//
//    GetNetworkAdapters
//	  ==================
//
//    This routine is called when recovering the network adapters during a registry scan.  As
//    this information is not registry specific we recover using this common function
//
bool CNetworkAdaptersScanner::GetNetworkAdapters()
{
    HINSTANCE			hDll;
	DWORD				(WINAPI *lpfnGetAdaptersInfo)(PIP_ADAPTER_INFO myInfo, ULONG *pLength);
	DWORD				(WINAPI *lpfnGetIfTable)( PMIB_IFTABLE pIfTable, PULONG pdwSize, BOOL bOrder);
	PMIB_IFTABLE		pIfTable;
	PMIB_IFROW			pIfEntry;
	PIP_ADAPTER_INFO	pAdapterTable, pAdapterInfo;
	ULONG				ulLength = 0;
	UINT				uIndex = 0;
	PIP_ADDR_STRING		pAddressList;
	CString				strMACAddress;
	CString				strAddress;

	CLogFile log;
	log.Write("CNetworkAdaptersScanner::GetNetworkAdapters Start" ,true);

	try
	{
		// Ensure that the list is empty
		_listNetworkAdapters.Empty();

		// Load iphlpapi.dll and get the addresses of necessary functions
		if ((hDll = LoadLibrary("iphlpapi.dll")) < (HINSTANCE)HINSTANCE_ERROR) 
		{
			// Cannot load iphlpapi MIB
			hDll = NULL;
			log.Write("Cannot load 'iphlpapi.dll' module - exiting network adapter scan"  ,true);
			return false;
		}

		// Try and get the address of the GetIfTable function - if this does not exist then the iphlpapi dll
		// must be too old for us to use
		if ((*(FARPROC*)&lpfnGetIfTable = GetProcAddress( hDll, "GetIfTable")) == NULL)
		{
			log.Write("Cannot locate 'GetIfTable' function - exiting network adapter scan"  ,true);
			FreeLibrary( hDll);
			return false;
		}

		// Try and get the address of the GetAdaptersInfo function - if this does not exist then the iphlpapi dll
		// must be too old for us to use
		if ((*(FARPROC*)&lpfnGetAdaptersInfo = GetProcAddress( hDll, "GetAdaptersInfo")) == NULL)
		{
			log.Write("Cannot locate 'GetAdaptersInfo' function - exiting network adapter scan"  ,true);
			FreeLibrary( hDll);
			return false;
		}

		// Call GetIfTable to get memory size
		pIfTable = NULL;
		ulLength = 0;
		DWORD result = lpfnGetIfTable(pIfTable, &ulLength, TRUE);
		if (result != ERROR_BUFFER_OVERFLOW && result != ERROR_INSUFFICIENT_BUFFER)
		{
			FreeLibrary( hDll);
			return false;
		}
		
		// Allocate some memory to hold the interface table
		if ((pIfTable = (PMIB_IFTABLE) malloc( ulLength+1)) == NULL)
		{
			log.Write("Failed to allocate interface table memory - exiting network adapter scan"  ,true);
			FreeLibrary( hDll);
			return false;
		}
		// Read the Interface table into the allocated buffer now
		result = lpfnGetIfTable(pIfTable, &ulLength, TRUE);
		if (result != NO_ERROR)
		{
			log.Write("Failed to read interface table memory - exiting network adapter scan"  ,true);
			free(pIfTable);
			FreeLibrary( hDll);
			return false;
		}

		// Now do the same to get the Adapters table
		pAdapterTable = NULL;
		result = lpfnGetAdaptersInfo(pAdapterTable, &ulLength);
		if (result != ERROR_BUFFER_OVERFLOW && result != ERROR_INSUFFICIENT_BUFFER)
		{
			log.Write("Failed to read adapters table - exiting network adapter scan"  ,true);
			free(pIfTable);
			FreeLibrary( hDll);
			return false;
		}

		// ...and allocate memory to hold the adapters table given the size returned above
		if ((pAdapterTable = (PIP_ADAPTER_INFO) malloc( ulLength+1)) == NULL)
		{
			log.Write("Failed to allocate adapters table memory (2) - exiting network adapter scan"  ,true);
			free(pIfTable);
			FreeLibrary( hDll);
			return false;
		}

		// Read the Adapters table into the allocated buffer
		result = lpfnGetAdaptersInfo(pAdapterTable, &ulLength);
		if (result != NO_ERROR)
		{
			log.Write("CFailed to read adapters table (2) - exiting network adapter scan"  ,true);
			free( pIfTable);
			free( pAdapterTable);
			FreeLibrary( hDll);
			return false;
		}

		// Iterate through the interfaces identified and recover full details for each
		for (DWORD dwIndex = 0; dwIndex < pIfTable->dwNumEntries; dwIndex ++)
		{
			// If this is NOT a loopback adapter then process it
			if (pIfTable->table[dwIndex].dwType != 24)
			{
				// The network adapter object		
				CNetworkAdapter		networkAdapter;

				// Get this interface entry 
				pIfEntry = &(pIfTable->table[dwIndex]);

				// Get MAC Address 
				strMACAddress.Format("%02X:%02X:%02X:%02X:%02X:%02X",
									pIfEntry->bPhysAddr[0], pIfEntry->bPhysAddr[1],
									pIfEntry->bPhysAddr[2], pIfEntry->bPhysAddr[3],
									pIfEntry->bPhysAddr[4], pIfEntry->bPhysAddr[5]);
				networkAdapter.MacAddress(strMACAddress);

				// Get the Speed
				networkAdapter.Speed(pIfEntry->dwSpeed);
			
				// Now parse the Adapter information for additional details
				for (pAdapterInfo=pAdapterTable ; pAdapterInfo != NULL ; pAdapterInfo = pAdapterInfo->Next)
				{
					// Use the adapter index to find the adapter that we are looking at now
					if (pIfEntry->dwIndex == pAdapterInfo->Index)
					{
						// Adapter Name
						networkAdapter.ProductName(pAdapterInfo->AdapterName);

						// Get IP address and NetMasks
						pAddressList = &(pAdapterInfo->IpAddressList);

						// need to handle IPv6 here, as it will have come back in this format:
						// 192.168.0.122;fe80::cd54:2994:4023:1faa
						// we just want the first section before the semi-colon
						CString strIPAddress = pAddressList->IpAddress.String;	

						if (strIPAddress.Find( ';' ) > 0)
						{
							strIPAddress = strIPAddress.Left(strIPAddress.Find( ';' ));
						}
						networkAdapter.IPAddress(strIPAddress);

						networkAdapter.IPSubNet(pAddressList->IpMask.String);

						// DHCP Enabled
						bool dhcpEnabled = (pAdapterInfo->DhcpEnabled != 0);
						networkAdapter.DHCPEnabled(dhcpEnabled);

						// Adapter / Interface type
						switch (pAdapterInfo->Type) 
						{
							case MIB_IF_TYPE_OTHER:
								networkAdapter.Type("Other");
								break;
							case MIB_IF_TYPE_ETHERNET:
								networkAdapter.Type("Ethernet");
								break;
							case MIB_IF_TYPE_TOKENRING:
								networkAdapter.Type("Token Ring");
								break;
							case MIB_IF_TYPE_FDDI:
								networkAdapter.Type("FDDI");
								break;
							case MIB_IF_TYPE_PPP:
								networkAdapter.Type("PPP");
								break;
							case MIB_IF_TYPE_LOOPBACK:
								networkAdapter.Type("Lookback");
								break;
							case MIB_IF_TYPE_SLIP:
								networkAdapter.Type("Slip");
								break;
							default:
								CString strType;
								strType.Format("Unknown type %ld\n", pAdapterInfo->Type);
								networkAdapter.Type(strType);
								break;
						}
					}
				}

				// Add this adapter
				_listNetworkAdapters.Add(networkAdapter);
			}
		}

		// Free memory
		free(pIfTable);
		free(pAdapterTable);

		// Unload library
		FreeLibrary( hDll);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	log.Write("CNetworkAdaptersScanner::GetNetworkAdapters End" ,true);
	return true;
}



//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CNetworkAdaptersScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CString categoryName;
	CString speed;
	
	CLogFile log;
	log.Write("CNetworkAdaptersScanner::SaveData Start" ,true);

	// OK on entry we will have started the Hardware Category so we do not need to worry about
	// that however we do need to add our own Hardware-item section if we are going to write anything
	if (_listNetworkAdapters.GetCount() != 0)
	{
		// Write a placeholder item for the hardware class itself as this will ensure that the category can be displayed
		CAuditDataFileCategory mainCategory(HARDWARE_CLASS);
		pAuditDataFile->AddAuditDataFileItem(mainCategory);

		// and iterate through the audited applications
		for (int isub=0; isub<(int)_listNetworkAdapters.GetCount(); isub++)
		{
			// Format the hardware class name for this graphics adapter
			CNetworkAdapter* pAdapter = &_listNetworkAdapters[isub];
			categoryName.Format("%s|%s" ,HARDWARE_CLASS ,pAdapter->ProductName());
			//categoryName.Format("%s|Network Adapter (%d)" ,HARDWARE_CLASS ,isub);

			// Each Adapter has its own category
			CAuditDataFileCategory category(categoryName);

			// Each audited item gets added an a CAuditDataFileItem to the category
			CAuditDataFileItem adapter(V_NETWORKADAPTER_NAME ,pAdapter->ProductName());
			CAuditDataFileItem mfr(V_NETWORKADAPTER_MANUFACTURER ,pAdapter->Manufacturer());
			CAuditDataFileItem macaddress(V_NETWORKADAPTER_MACADDRESS ,pAdapter->MacAddress());
			CAuditDataFileItem ipaddress(V_NETWORKADAPTER_IPADDRESS ,pAdapter->IPAddress());

			// Add the items to the category
			category.AddItem(adapter);
			category.AddItem(mfr);
			category.AddItem(macaddress);
			category.AddItem(ipaddress);

			// ...and add the category to the AuditDataFile
			pAuditDataFile->AddAuditDataFileItem(category);
		}
	}

	log.Write("CNetworkAdaptersScanner::SaveData End" ,true);
	return false;
}


//
//    FindAdapter
//    ===========
//
//    Return the network adapter (if any) which has the specified IP address
//
CNetworkAdapter* CNetworkAdaptersScanner::FindAdapter (CString& strIPAddress)
{
	CNetworkAdapter* pFoundAdapter = NULL;
	CLogFile log;

	try
	{

		// Iterate through the adapters
		for (int index=0; index<(int)_listNetworkAdapters.GetCount(); index++)
		{
			CNetworkAdapter* pNetworkAdapter = &_listNetworkAdapters[index];

			if (pNetworkAdapter->IPAddress() == strIPAddress)
			{
				pFoundAdapter = pNetworkAdapter;
				break;
			}
		}
	}
	catch (CException *pEx)
	{
		LogException(pEx, "FindAdapter");
		//log.Write("CNetworkAdaptersScanner::SaveData End" ,true);
		throw pEx;
	}

	return pFoundAdapter;

}