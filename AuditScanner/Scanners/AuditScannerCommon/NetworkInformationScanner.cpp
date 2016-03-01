#include "stdafx.h"
#include "AuditDataFile.h"
#include "NetworkInformationScanner.h"
#include "NetworkAdaptersScanner.h"

//////////////////////////////////////////////////////////////////////////////
//
// Windows Domain / Workgroup registry keys and values
//
// Windows 9X
#define WIN_DOMAIN_KEY							"Security\\Provider"
#define WIN_DOMAIN_VALUE						"Container"
#define WIN_WORKGROUP_KEY						"SYSTEM\\CurrentControlSet\\Services\\VxD\\VNETSUP"
#define WIN_WORKGROUP_VALUE						"Workgroup"
#define WIN_NETCLIENT_KEY						"Network\\Logon"
#define WIN_NETCLIENT_VALUE						"PrimaryProvider"

// Windows NT On
#define NT_DOMAIN_KEY							"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon"
#define NT_DOMAIN_VALUE							"DefaultDomainName"
#define NT_WORKGROUP_KEY						"SYSTEM\\CurrentControlSet\\Services\\VxD\\VNETSUP"
#define NT_WORKGROUP_VALUE						"Workgroup"

// Storage Strings
#define HARDWARE_CLASS					"Hardware|Network"
#define V_NETWORK_COMPUTERNAME			"Computer Name"
#define V_NETWORK_DOMAINNAME			"Domain Name"
#define V_NETWORK_LOGONCLIENT			"Logon Client"
#define V_NETWORK_USERNAME				"Username"
#define V_NETWORK_MACADDRESS			"MAC Address"
#define V_NETWORK_IPADDRESS				"IP Address"


// Constructor
CNetworkInformationScanner::CNetworkInformationScanner() 
{
	m_strItemPath = HARDWARE_CLASS;
}

CNetworkInformationScanner::~CNetworkInformationScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool CNetworkInformationScanner::ScanWMI(CWMIScanner *pScanner)
{
	CLogFile log;
	log.Write("CNetworkInformationScanner::ScanWMI Start" ,true);

	// Temporary work data
	CString strDomain = "";					// CMD 8.3.4.1
	CString	strComputerName = "";			// CMD 8.3.4.1
	CString	strUsername = "";				// CMD 8.3.4.1
	CString	strLogonClient = "";			// CMD 8.3.4.1

	// Get the WMI object itself
	CLaytonWMI& wmiConnection = pScanner->GetWMIConnection();

	// Windows domain information is held within the Win32_ComputerSystem 
	try
	{
		if (wmiConnection.BeginEnumClassObject("Win32_ComputerSystem"))
		{
			while (wmiConnection.MoveNextEnumClassObject())
			{
				strDomain = wmiConnection.GetClassObjectStringValue("Domain");
				strComputerName = wmiConnection.GetClassObjectStringValue("Name");
				strUsername = wmiConnection.GetClassObjectStringValue("UserName");
			}
			wmiConnection.CloseEnumClassObject();

			// Save data
			ComputerName(strComputerName);
			DomainName(strDomain);
			UserName(strUsername);
			LogonClient(strLogonClient);
		}
	}

	catch (CException *pEx)
	{
		LogException(pEx, "CNetworkInformationScanner::ScanWMI (Enumerating Win32_ComputerSystem)");
		return false;
	}


	// To obtain the MAC address we actually need to make use of the CNetworkAdaptersScanner code as we essentially pull
	// all of the installed adapters and recover the MAc address for the one which has the 'active' IP address
	//
	// Of course we don't have the IP address either at this stage so need to recover it as well!
	try
	{
		CWindowsSocket windowsSocket;
		m_strIPAddress = windowsSocket.GetIPAddress();
	}
	catch (CException *pEx)
	{
		LogException(pEx, "CNetworkInformationScanner::ScanWMI (Getting IP address)");
		return false;
	}
	
	//
	CNetworkAdaptersScanner networkAdapters;
	networkAdapters.Scan(pScanner);
	log.Write("after networkAdapters.Scan");
	CNetworkAdapter* pPrimaryAdapter = networkAdapters.FindAdapter(m_strIPAddress);
	log.Write("after networkAdapters.FindAdapter");
	if (pPrimaryAdapter != NULL)
		m_strMACAddress = pPrimaryAdapter->MacAddress();
	else
		m_strMACAddress = UNKNOWN;

	log.Write("CNetworkInformationScanner::SaveData End" ,true);
	return true;
}


//
//    ScanRegistryNT
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows NT or higher
//
bool	CNetworkInformationScanner::ScanRegistryNT()
{
	return ScanRegistryXP();
}


//
//    ScanRegistryXP
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows XP or higher
//
bool	CNetworkInformationScanner::ScanRegistryXP()
{
	HKEY	hKey = NULL;
	unsigned char szValue[256];
	LONG	lResult;
	DWORD	dwType(REG_SZ);
	DWORD	dwSize(255);

	CLogFile log;
	log.Write("CNetworkInformationScanner::ScanRegistryXP Start" ,true);

	try
	{	
		// Temporary work data
		CString strDomain = "";						// CMD 8.3.4.1
		CString	strComputerName = "";				// CMD 8.3.4.1
		CString	strUsername = "";					// CMD 8.3.4.1
		CString	strLogonClient = "";				// CMD 8.3.4.1

		// Open the registry key
		lResult = RegOpenKeyEx(m_hKey, NT_DOMAIN_KEY, 0, KEY_READ, &hKey);
		if (lResult != ERROR_SUCCESS)
			return false;

		// Get domain name or workgroup.
		dwSize = 255;
		lResult = RegQueryValueEx(hKey, NT_DOMAIN_VALUE, NULL, &dwType, szValue, &dwSize);
		RegCloseKey( hKey);
		if (lResult == ERROR_SUCCESS)
		{
			// Save the domain
			CString strDomain = szValue;
			DomainName(strDomain);
		}

		// Get the computer name
		ComputerName(GetComputerName());

		// Logged on user
		UserName(GetActiveUser());

		// Logon Client
		LogonClient("Client for Microsoft Networks");
		log.Write("CNetworkInformationScanner::ScanRegistryXP End" ,true);
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	return true;
}



//
//    ScanRegistry9X
//    ==============
//
//    Over-ride of the base class to recover information using the System Registry object 
//	  when we are running under Windows 9X
//
bool	CNetworkInformationScanner::ScanRegistry9X()
{
	CLogFile log;
	HKEY	hKey = NULL;
	unsigned char szValue[256];
	LONG	lResult;
	DWORD	dwType(REG_SZ);
	DWORD	dwSize(255);
	
	// Temporary work data
	CString strDomain = "";								// CMD 8.3.4.1
	CString	strComputerName = "";						// CMD 8.3.4.1
	CString	strUsername = "";							// CMD 8.3.4.1
	CString	strLogonClient = "";						// CMD 8.3.4.1

	try
	{
		// Open the registry key
		lResult = RegOpenKeyEx(m_hKey, WIN_DOMAIN_KEY, 0, KEY_READ, &hKey);
		if (lResult == ERROR_SUCCESS)
		{
			// Get domain name or workgroup.
			dwSize = 255;
			lResult = RegQueryValueEx(hKey, WIN_DOMAIN_VALUE, NULL, &dwType, szValue, &dwSize);
			RegCloseKey( hKey);
			if (lResult == ERROR_SUCCESS)
				DomainName((CString)szValue);
		}

		// Get the computer name
		ComputerName(GetComputerName());

		// Logged on user
		char szUserName[UNLEN + 1];
		DWORD dwLen = UNLEN;
		if (::GetUserName(szUserName, &dwLen))
			UserName((CString)szUserName);

		// Logon Client
		lResult = RegOpenKeyEx(m_hKey, WIN_NETCLIENT_KEY, 0, KEY_READ, &hKey);
		if (lResult == ERROR_SUCCESS)
		{
			// Get logon client
			dwSize = 255;
			lResult = RegQueryValueEx(hKey, WIN_NETCLIENT_VALUE, NULL, &dwType, szValue, &dwSize);
			RegCloseKey( hKey);
			if (lResult == ERROR_SUCCESS)
				LogonClient((CString)szValue);
		}
	}
	catch (CException *pEx)
	{
		throw pEx;
	}

	return true;
}


//
//    ScanExceptions
//    ==============
//
//    We over-ride the base class as we need to recover the primary IP address and MAC address
//    and we cannot do that using WMI even if the rest of the scan has been done using WMI
//
bool CNetworkInformationScanner::ScanExceptions()
{
	// Get the primary IP address for this computer
	CWindowsSocket windowsSocket;
	m_strIPAddress = windowsSocket.GetIPAddress();
	return true;
}



//
//    SaveData
//    ========
//
//    Save the information for this object to the AuditDataFile
//
bool CNetworkInformationScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CNetworkInformationScanner::SaveData Start" ,true);

	CString strValue;

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	// Each audited item gets added an a CAuditDataFileItem to the category
	CAuditDataFileItem n1(V_NETWORK_COMPUTERNAME ,m_strComputerName);
	CAuditDataFileItem n2(V_NETWORK_DOMAINNAME ,m_strDomainName);
	//CAuditDataFileItem n3(V_NETWORK_LOGONCLIENT ,m_strLogonClient);
	CAuditDataFileItem n4(V_NETWORK_USERNAME ,m_strUserName);
	CAuditDataFileItem n5(V_NETWORK_IPADDRESS ,m_strIPAddress);
	CAuditDataFileItem n6(V_NETWORK_MACADDRESS ,m_strMACAddress);

	// Add the items to the category
	category.AddItem(n1);
	category.AddItem(n2);
	//category.AddItem(n3);
	category.AddItem(n4);
	category.AddItem(n5);
	category.AddItem(n6);

	// ...and add the category to the AuditDataFile
	pAuditDataFile->AddAuditDataFileItem(category);

	log.Write("CNetworkInformationScanner::SaveData End" ,true);
	return true;
}