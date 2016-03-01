#include "stdafx.h"

// Include our base class header
#include "AuditScannerConfiguration.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"
#include "AuditDataScanner.h"
#include "LocaleScanner.h"

// Storage Strings
#define HARDWARE_CLASS				"System|Locale"
#define V_LOCALE_CODEPAGE			"Codepage"
#define V_LOCALE_CALENDARTYPE		"Calendar Type"
#define V_LOCALE_COUNTRY			"Country"
#define V_LOCALE_COUNTRYCODE		"Country Code"
#define V_LOCALE_CURRENCY			"Currency"
#define V_LOCALE_DATEFORMAT			"Date Format"
#define V_LOCALE_LANGUAGE			"Language"
#define V_LOCALE_LOCALLANGUAGE		"Local Language"
#define V_LOCALE_OEM_CODEPAGE		"OEM Codepage"
#define V_LOCALE_TIMEFORMAT			"Time Format"
#define V_LOCALE_TIMEFORMATSPECIFIER "Time Format Specifier"
#define V_LOCALE_TIMEZONE			"Time Zone"

CLocaleScanner::CLocaleScanner(void)
{
	m_strItemPath = HARDWARE_CLASS;
	//
	m_iCodePage = 0;
	m_strCalendarType = UNKNOWN;
	m_iOEMCodePage = 0;
	m_strLanguage = UNKNOWN;
	m_strDateFormat = UNKNOWN;
	m_strCountry = UNKNOWN;
	m_iCountryCode = 0;
	m_strTimeFormat = UNKNOWN;
	m_strCurrency = UNKNOWN;
	m_strTimeFormatSpecifier = UNKNOWN;
	m_strLocaleLocalLanguage = UNKNOWN;
	m_strLocaleTimeZone = UNKNOWN;
}

CLocaleScanner::~CLocaleScanner(void)
{
}


//
//    ScanWMI
//    =======
//
//    Over-ride of the base class to recover information using the WMI connection
//
bool	CLocaleScanner::ScanWMI(CWMIScanner *pScanner)
{
	// No WMI Solution for locale!
	return false;
}



//
//    ScanXP
//    ======
//
//    Over-ride of the base class to recover information using a Windows XP onwards registry scan
//
bool	CLocaleScanner::ScanRegistryXP()
{
	try
	{
		ScanLocale();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    ScanRegistryNT
//    =============
//
//    Over-ride of the base class to recover information using a Windows NT/2000 Registry Scan
//
bool	CLocaleScanner::ScanRegistryNT()
{
	try
	{
		ScanLocale();
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
//    Over-ride of the base class to recover information using a Windows 9X registry scan
//
bool	CLocaleScanner::ScanRegistry9X()
{
	try
	{
		ScanLocale();
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	return true;
}


//
//    Save
//    ====
//
//    Save the information for this object to the AuditDataFile
//
bool CLocaleScanner::SaveData	(CAuditDataFile* pAuditDataFile)
{
	CLogFile log;
	log.Write("CLocaleScanner::SaveData Start" ,true);

	CString strValue;

	// Add the Category for memory
	CAuditDataFileCategory category(HARDWARE_CLASS);

	// Each audited item gets added an a CAuditDataFileItem to the category
	CAuditDataFileItem l1(V_LOCALE_CODEPAGE ,m_iCodePage);
	CAuditDataFileItem l2(V_LOCALE_CALENDARTYPE ,m_strCalendarType);
	CAuditDataFileItem l3(V_LOCALE_COUNTRY ,m_strCountry);
	CAuditDataFileItem l4(V_LOCALE_COUNTRYCODE ,m_iCountryCode);
	CAuditDataFileItem l5(V_LOCALE_CURRENCY ,m_strCurrency);
	CAuditDataFileItem l6(V_LOCALE_DATEFORMAT ,m_strDateFormat);
	CAuditDataFileItem l7(V_LOCALE_LANGUAGE ,m_strLanguage);
	CAuditDataFileItem l8(V_LOCALE_LOCALLANGUAGE ,m_strLocaleLocalLanguage);
	CAuditDataFileItem l9(V_LOCALE_OEM_CODEPAGE ,m_iOEMCodePage);
	CAuditDataFileItem l10(V_LOCALE_TIMEFORMAT ,m_strTimeFormat);
	CAuditDataFileItem l11(V_LOCALE_TIMEFORMATSPECIFIER ,m_strTimeFormatSpecifier);
	CAuditDataFileItem l12(V_LOCALE_TIMEZONE ,m_strLocaleTimeZone);

	// Add the items to the category
	category.AddItem(l1);
	category.AddItem(l2);
	category.AddItem(l3);
	category.AddItem(l4);
	category.AddItem(l5);
	category.AddItem(l6);
	category.AddItem(l7);
	category.AddItem(l8);
	category.AddItem(l9);
	category.AddItem(l10);
	category.AddItem(l11);
	category.AddItem(l12);

	// ...and add the category to the AuditDataFile
	pAuditDataFile->AddAuditDataFileItem(category);

	// we always need to get the default browser details so do here
	CAuditDataFileCategory browserCategory("Internet|Browsers|Default Browser", FALSE, TRUE);
	CAuditDataFileItem b1("Path", GetRegValue("HKEY_CLASSES_ROOT\\http\\shell\\open\\command", ""));
	browserCategory.AddItem(b1);

	pAuditDataFile->AddInternetItem(browserCategory);


	log.Write("CLocaleScanner::SaveData End" ,true);
	return true;
}

void CLocaleScanner::ScanLocale ()
{
	CLogFile log;
	log.Write("CLocaleScanner::ScanLocale Start" ,true);

	try
	{
		// Retirieve the information about the locale.
		TCHAR chPage[7];
		TCHAR szCurrency[7];
		TCHAR szVal[MAX_PATH];
		TCHAR szFormatString[100];

		// Get currently ised code page.
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_IDEFAULTANSICODEPAGE, chPage, 7);
		m_iCodePage = atoi (chPage);

		// Get default OEM code page.
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_IDEFAULTCODEPAGE, chPage, 7);
		m_iOEMCodePage = atoi (chPage);
	 
		// Get country name in english.
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_SENGCOUNTRY , szVal, MAX_PATH);
		m_strCountry = szVal;

		// Get country code
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_ICOUNTRY , chPage, 7);
		m_iCountryCode = atoi (chPage);

		// Get language name.
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_SENGLANGUAGE , szVal, MAX_PATH);
		m_strLanguage = szVal;

		// Get TimeFormat String
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_STIMEFORMAT , szFormatString, 100);
		m_strTimeFormat = szFormatString;
		
		// Get Date Format String
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_SLONGDATE  , szFormatString, 100);
		m_strDateFormat = szFormatString;

		// Get the string used for local currency.
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_SCURRENCY, szCurrency, 7);
		m_strCurrency = szCurrency;

		// Get time format specifier i.e. 12 hour (AM/PM) or 24 hour format
		// is used to indicate time.
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_ITIME, szVal, 3);
		if (atoi (szVal) == 0)
		{
			m_strTimeFormatSpecifier = "AM / PM 12-hour format";
		}
		else
		{
			m_strTimeFormatSpecifier = "24-hour format";
		}
		
		// Get calendar type
		::GetLocaleInfo (LOCALE_USER_DEFAULT, LOCALE_ICALENDARTYPE, szVal, 3);
		switch (atoi (szVal))
		{
			case 1:
				m_strCalendarType = ("Gregorian - Localized");
				break;
			case 2:
				m_strCalendarType = ("Gregorian - English strings always");
				break;
			case 3:
				m_strCalendarType = ("Year of the Emperor - Japan");
				break;
			case 4:
				m_strCalendarType = ("Year of Taiwan");
				break;
			case 5:
				m_strCalendarType = ("Tangun Era - Korea");
				break;
			case 6:
				m_strCalendarType = ("Hijri - Arabic lunar");
				break;
			case 7:
				m_strCalendarType = ("Thai");
				break;
			case 8:
				m_strCalendarType = ("Hebrew - Lunar");
				break;
			case 9:
				m_strCalendarType = ("Gregorian Middle East French");
				break;
			case 10:
				m_strCalendarType = ("Gregorian Arabic");
				break;
			case 11:
				m_strCalendarType = ("Gregorian Transliterated English");
				break;
			case 12:
				m_strCalendarType = ("Gregorian Transliterated French");
				break;
			default:
				m_strCalendarType = ("Unknown");
		}

		LANGID langId;
		// Get the ID of the language identifier.
		langId = ::GetSystemDefaultLangID ();

		DWORD dwSize, dwError, dwReturnedSize;
		char szLanguage[MAX_PATH];
		// Get the string for the language identifier.
		dwSize = MAX_PATH;
		dwReturnedSize = VerLanguageName (langId, szLanguage, dwSize);
		if (dwReturnedSize <= dwSize)
		{
			m_strLocaleLocalLanguage = szLanguage;
		}

		// Get time zone information.
		TIME_ZONE_INFORMATION info;
		TCHAR szTimezone[31];
		dwError = ::GetTimeZoneInformation (&info);
		if (TIME_ZONE_ID_INVALID != dwError)
		{
			wsprintf(szTimezone, "%S", info.StandardName);
			m_strLocaleTimeZone = szTimezone;
		}
		m_bDetected = TRUE;
	}
	catch (CException *pEx)
	{
		throw pEx;
	}
	log.Write("CLocaleScanner::ScanLocale End" ,true);
}

//
//    GetRegValue
//    ===========
//
//    Recover a value from the system registry
//
CString CLocaleScanner::GetRegValue (LPCSTR pszRegKey, LPCSTR pszRegItem)
{
	CString strResult;

	// work out which "hive" to access
	CString strRegKey(pszRegKey);
	CString strHive = BreakString(strRegKey, '\\');
	HKEY hkHive = NULL, hkSubKey;
	if (strHive == "HKEY_CLASSES_ROOT")
		hkHive = HKEY_CLASSES_ROOT;
	if (strHive == "HKEY_CURRENT_CONFIG")
		hkHive = HKEY_CURRENT_CONFIG;
	if (strHive == "HKEY_CURRENT_USER")
		hkHive = HKEY_CURRENT_USER;
	if (strHive == "HKEY_LOCAL_MACHINE")
		hkHive = HKEY_LOCAL_MACHINE;
	if (strHive == "HKEY_USERS")
		hkHive = HKEY_USERS;

	if (hkHive)
	{
		int nStatus = RegOpenKeyEx(hkHive, strRegKey, 0, KEY_QUERY_VALUE, &hkSubKey);
		if (nStatus == ERROR_SUCCESS)
		{
			// key opened ok - look for matching item
			DWORD dwIndex = 0;
			unsigned char szThisRegValue[1024];
			DWORD dwType;
			DWORD dwRegValueLen = sizeof(szThisRegValue);

			int nStatus = RegQueryValueEx(hkSubKey ,pszRegItem ,NULL ,&dwType ,szThisRegValue ,&dwRegValueLen);
			if (nStatus == ERROR_SUCCESS)
			{
				// FOUND IT - sort out the type conversion
				switch (dwType)
				{
					case REG_BINARY:
						{
							// write as a sequence of hex values
							for (DWORD dw = 0 ; dw < dwRegValueLen ; dw++)
							{
								BYTE b = szThisRegValue[dw];
								CString strThisBit;
								strThisBit.Format("%2.2X ", b);
								strResult += strThisBit;
							}
							strResult.TrimRight();
						}
						break;

					case REG_DWORD:
//					case REG_DWORD_LITTLE_ENDIAN:
						strResult.Format("%d", *((LPDWORD)szThisRegValue));
						break;
						
					case REG_SZ:
						strResult = szThisRegValue;
						break;

					case REG_EXPAND_SZ:
						{
							char szBuffer[1024];
							ExpandEnvironmentStrings ((LPCSTR)szThisRegValue, szBuffer, sizeof(szBuffer));
							strResult = szBuffer;
						}
						break;

					case REG_MULTI_SZ:
						{
							for (char * p = (LPSTR)szThisRegValue ; *p != NULL ; p += strlen(p) + 1)
							{
								if (strResult.GetLength())
									strResult += ';';
								strResult += p;
							}
						}
						break;

					case REG_DWORD_BIG_ENDIAN:
					case REG_LINK:
					case REG_NONE:
//					case REG_QWORD:
//					case REG_QWORD_LITTLE_ENDIAN:
					case REG_RESOURCE_LIST:
					default:
						strResult.Format("Unsupported Registry Data Type %d", dwType);
						break;
				}
			}
		}
		RegCloseKey(hkSubKey);
	
	
	}
	return strResult;
}