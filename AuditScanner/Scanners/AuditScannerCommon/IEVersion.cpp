// IEVersion.cpp: implementation of the CIEVersion class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "IEVersion.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CIEVersion::CIEVersion()
{
	GetVersion();
}

CIEVersion::~CIEVersion()
{

}

//
//    GetVersion
//    ===========
//
//    Read the version from the registry
//
void CIEVersion::GetVersion	(void)
{
	HKEY hCU; 
	m_strVersion = "Internet Explorer ";

//
// let's open the Registry key for IE
//
	if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Internet Explorer", 0 ,KEY_QUERY_VALUE, &hCU) == ERROR_SUCCESS)
	{
		char szIEVersion[100];
		int nMajor, nMinor, nBuild, nSubBuild;

		DWORD ulSize = sizeof(szIEVersion);

// For IE 11 we have a differnt key
		if (RegQueryValueEx( hCU, "svcVersion", NULL, NULL, (LPBYTE)szIEVersion, &ulSize) != ERROR_SUCCESS)
		{
			if (RegQueryValueEx( hCU, "Version", NULL, NULL, (LPBYTE)szIEVersion, &ulSize) != ERROR_SUCCESS)
				RegQueryValueEx(hCU, "IVer", NULL, NULL, (LPBYTE)szIEVersion, &ulSize);
		}

//
//    Split up the version into its components parts
//
		sscanf(szIEVersion, "%d.%d.%d.%d", &nMajor, &nMinor, &nBuild, &nSubBuild);

		// trace to logfile if active
		CLogFile log;
		log.Format("Detected IE Version %s = %d %d %d %d\n",szIEVersion, nMajor ,nMinor ,nBuild ,nSubBuild);

//
//    Now lets format a textual representation.
//
		switch (nMajor)
		{
		case 4:

			switch (nMinor)
			{
			case 40:					// Version 1 & 2 Derivatives
				switch (nBuild)
				{
				case 308:
					m_strVersion += "1.0 Plus!";
					break;
				case 520:
					m_strVersion += "2.0";
					break;
				default:
					m_strVersion += szIEVersion;
					break;
				}
				break;

			case 70:					// Version 3 Derivatives
				m_strVersion += "3.0";

				switch (nBuild)
				{
				case 1155:
					break;
				case 1158:
					m_strVersion += " (OSR2)";
					break;
				case 1215:
					m_strVersion += "1";
					break;
				case 1300:
					m_strVersion += "2";
					break;
				default:
					break;
				}
				break;

			case 71:
				m_strVersion += "4.0";
				break;

			case 72:
				m_strVersion += "4.0";

				switch (nBuild)
				{
				case 2106:
					m_strVersion += "1";
					break;
				case 3110:
					m_strVersion += "1 (SP1)";
					break;
				case 3612:
					m_strVersion += "1 (SP2)";
					break;
				default:
					break;
				}
				break;

			default:
				m_strVersion += "4.0 (unrecognized sub-version)";
				break;
			}
			break;

//
//    Version 5 Derivatives
//
		case 5:							
			m_strVersion += "5.0";

			switch (nMinor)
			{
			case 0:
				switch (nBuild)
				{
				case 518:
					m_strVersion += " (BETA 1)";
					break;
		
				case 910:
					m_strVersion += " (BETA 2)";
					break;
		
				case 2014:
				case 2314:
				case 2614:
					break;

				case 2516:
				case 2919:
				case 2920:
					m_strVersion += "1";
					break;

				case 3103:
				case 3105:
					m_strVersion += "1 SP1";
					break;

				case 3314:
				case 3315:
					m_strVersion += "1 SP2";
					break;
	
				default:
					break;
				}
				break;

//
//	IE 5.5 Derivatives
//
			case 50:		
				m_strVersion += "5.5";

				switch (nBuild)
				{
				case 4308:
					m_strVersion += " Advanced Security Privacy Beta";
					break;

				case 4522:
					m_strVersion += " SP1";
					break;
	
				case 4807:
					m_strVersion += " SP2";
					break;

				default:
					break;
				}
				break;

			default:
				m_strVersion += "5.0 (unrecognized sub-version)";
				break;

			}
			break;

//
// Version 6 derivatives
//
		case 6:
			m_strVersion += "6.0";
			break;

		default:
			m_strVersion.Format("%d.%d" ,nMajor ,nMinor);
			break;
		}			
			
		RegCloseKey(hCU);					
	}
	else
	{
		m_strVersion = "Not Installed";
	}
}



