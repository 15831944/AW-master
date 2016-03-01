
//========================================================================================
//
//    COperatingSystemScanner
//    =======================
//
//	Class to scan the Operating System - note this is not a derived class
//
//========================================================================================


#include "stdafx.h"
#include "AuditDataFile.h"
#include "OperatingSystemScanner.h"
#include "IEVersion.h"

const char gszDigitalProductID[]	= "DigitalProductID";
#define RK_95_VERSION	"SOFTWARE\\Microsoft\\Windows\\CurrentVersion"
#define RK_NT_VERSION	"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"
#define RI_OS_SERIAL	"ProductId"
#define AUDITWIZARD_REGKEY "SOFTWARE\\Layton Technology, Inc.\\AuditWizard_v8"

COperatingSystemScanner::COperatingSystemScanner() 
{
	// Initialize all values
	_osFamily		= UNKNOWN;		
	_osProductID	= UNKNOWN;			
	_osVersion		= UNKNOWN;			
	_osCDKey		= UNKNOWN;			
	_osIEVersion	= UNKNOWN;	
	_osIs64Bit		= false;
}

COperatingSystemScanner::~COperatingSystemScanner(void)
{
}


//
//    Scan
//    ====
//   
//    This is the main scanning function
//
bool COperatingSystemScanner::Scan()
{
	COsInfo os;
	CLogFile log;

	// additional Internet Explorer Information
	CIEVersion IEVersion;
	_osFamily = os.GetName();
	_osVersion = os.GetVersion();
	_osProductID = os.GetProductID();
	_osCDKey = os.GetCDKey();
	_osIEVersion = IEVersion.m_strVersion;
	_osIs64Bit = os.Is64BitWindows();														// 8.3.4 - CMD

	HKEY hk;
	DWORD dwDisp;
	LONG lResult;

	if (_osCDKey == "")
	{
		CByteArray arrayDigitalProductID;
		
		// try and load the sub-key
		if (ERROR_SUCCESS == RegOpenKeyEx (HKEY_LOCAL_MACHINE, RK_NT_VERSION, 0, KEY_READ | KEY_WOW64_64KEY, &hk))
		{
			DWORD dwBufferSize = 1024;
			LPBYTE lpbMemoryBuffer = (LPBYTE) ::malloc( dwBufferSize );

			// Now get the actual value from the registry
			DWORD dwDataType = REG_BINARY;
			lResult = RegQueryValueEx (hk, "DigitalProductId", NULL, NULL, lpbMemoryBuffer, &dwBufferSize);

			if (ERROR_SUCCESS == lResult)
			{
				arrayDigitalProductID.RemoveAll();
				DWORD dwIndex = 0;
				while( dwIndex < dwBufferSize )
				{
					arrayDigitalProductID.Add( lpbMemoryBuffer[dwIndex]);
					dwIndex++;
				}
			}

			// close the key
			RegCloseKey (hk);

			// Free the memory buffer allocated
			::free( lpbMemoryBuffer );

			_osCDKey = DecodeDigitalProductKey(arrayDigitalProductID);
		}
	}

	if (_osProductID == "")
	{
		if (RegOpenKeyEx( HKEY_LOCAL_MACHINE, RK_NT_VERSION, 0, KEY_READ | KEY_WOW64_64KEY, &hk ) == ERROR_SUCCESS)
		{
			char szProductId[100];
			DWORD ulSize = sizeof(szProductId);
			lResult = RegQueryValueEx( hk, "ProductId", NULL, NULL, (LPBYTE)szProductId, &ulSize);

			if (lResult == ERROR_SUCCESS)
			{	
				_osProductID = szProductId;
			}

			RegCloseKey(hk);
		}
	}

	return true;
}

//
bool COperatingSystemScanner::Save (CAuditDataFile& file)
{
	file.OSFamily(_osFamily);
	file.OSVersion(_osVersion);
	file.OSProductID(_osProductID);
	file.OSCDKey(_osCDKey);
	file.OSIEVersion(_osIEVersion);
	file.OSIs64Bit(_osIs64Bit);

	return true;
}
