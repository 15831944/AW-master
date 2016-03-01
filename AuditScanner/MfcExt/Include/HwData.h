
// FILE:	HwData.h
// PURPOSE:	Declaration of classes for handling hardware data - common between AudWiz scanner and manager
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology 2003
// HISTORY:	JRFT - 24.11.2003 - written

#ifndef _HWDATA_DEF_
#define _HWDATA_DEF_

// definitions borrowed from AudWiz database header file db.h
#define ICON_LOCATION		2
#define ICON_ASSET			3
#define ICON_ATTR			4
#define ICON_FOLDER			8
#define	ICON_EXE			9
#define ICON_APP			13
#define ICON_HW				15
#define ICON_OS				38
#define ICON_HW_MOTHERBOARD	45
#define ICON_HW_PERIPHERALS	46
#define ICON_HW_NETWORK		47
#define ICON_HW_DRIVES		48
#define ICON_HW_ADAPTERS	49
#define ICON_HW_PORTS		50
#define ICON_HW_SYSTEM		51
#define ICON_HW_KEY			52
#define ICON_LICENSETYPES	53
#define ICON_HW_SCSI		54

// hardware storage flags
#define HF_CAT				0x0001	// Item is a category (folder)
#define HF_VALUE			0x0002	// Item is a data value (see "units" below)
#define HF_SPARE_01			0x0004
#define HF_SPARE_02			0x0008
#define HF_ALL				(HF_CAT | HF_VALUE)

#define HF_TYPE_HW			0x0000	// Icon indexes
#define HF_TYPE_OS			0x0010
#define HF_TYPE_ADAPTER		0x0020
#define HF_TYPE_NETWORK		0x0030
#define HF_TYPE_VIDEO		0x0040
#define HF_TYPE_MULTIMEDIA	0x0050
#define HF_TYPE_CODEC		0x0060
#define HF_TYPE_DRIVES		0x0070
#define HF_TYPE_DRV_FIXED	0x0080
#define HF_TYPE_DRV_REMOVE	0x0090
#define HF_TYPE_DRV_CDROM	0x00A0
#define HF_TYPE_DRV_NETWORK	0x00B0
#define HF_TYPE_BIOS		0x00C0
#define HF_TYPE_DMI			0x00D0
#define HF_TYPE_CPU			0x00E0
#define HF_TYPE_MEMORY		0x00F0
#define HF_TYPE_PERIPH		0x0100
#define HF_TYPE_MODEM		0x0110
#define HF_TYPE_PRINTER		0x0120
#define HF_TYPE_SCANNER		0x0130
#define HF_TYPE_KEYBOARD	0x0140
#define HF_TYPE_MOUSE		0x0150
#define HF_TYPE_PORT		0x0160
#define HF_TYPE_SYSTEM		0x0170
#define HF_TYPE_ENVIR		0x0180
#define HF_TYPE_LOCALE		0x0190
#define HF_TYPE_SERVICE		0x01A0
#define HF_TYPE_USER		0x01B0
#define HF_TYPE_PROCESS		0x01C0
#define HF_TYPE_REGKEY		0x01D0
#define HF_TYPE_OS_PATCH	0x01E0
#define HF_TYPE_FIREWALL	0x01F0
#define HF_TYPE_WINDOWSUPDATE	0x0200
#define HF_TYPE_SECURITY	0x0210
#define HF_TYPE_ALL			0x0FF0	// bit mask to extract all of the above

#define HF_UNITS_NONE		0x0000	// No units (eg text string)
#define HF_UNITS_COUNT		0x1000	// Count of items
#define HF_UNITS_BYTE		0x2000	// Bytes
#define HF_UNITS_MB			0x3000	// MegaBytes
#define HF_UNITS_DATE		0x4000	// Date
#define HF_UNITS_IP			0x5000	// IP Address
#define HF_UNITS_MAC		0x6000	// MAC Address
#define HF_UNITS_MHZ		0x7000	// Clock Speed
#define HF_UNITS_ALL		0xF000	// bit mask to extract all of the above

#define HW_SEP ';'

extern const char gszMotherboard[];
extern const char gszPeripherals[];
extern const char gszNetwork[];
extern const char gszDrives[];
extern const char gszAdapters[];
extern const char gszPorts[];
extern const char gszSystem[];
extern const char gszDMI[];
extern const char gszScsi[];

/*
** Holds a single collected hardware data record
*/
class CHwData
{
public:
	// default constructor (used by list classes)
	CHwData()
		{}
	// explicit constructor
	CHwData(LPCSTR pszKey, LPCSTR pszVal, DWORD dwType) : m_strKey(pszKey), m_strVal(pszVal), m_dwType(dwType)
		{}
	// data access
	const CString & GetKey() const
		{ return m_strKey; }
	const CString & GetVal() const
		{ return m_strVal; }
	DWORD GetType() const
		{ return m_dwType; }
	void SetVal (LPCSTR pszVal)
		{ m_strVal = pszVal; }
	void SetType (DWORD dwType)
		{ m_dwType = dwType; }
protected:
	CString	m_strKey;
	CString	m_strVal;
	DWORD	m_dwType;
};

/*
** Collection class for hardware info
*/
class CHwDataList : public CDynaList<CHwData>
{
public:
	CHwDataList()
	{ m_strSection = "Hardware"; }

	static int GetIcon (LPCSTR pszCat);
public:
	// read all data from an INI file
	BOOL Load (CIniFile & file ,LPCSTR pszSection);
	// write to INI file
	BOOL Save (CIniFile & file);
	// list all sub-categories for a given parent
	DWORD EnumSubCats (LPCSTR pszCat, CSortList & cats);
	// list all keys stored under a fully-qualified category name
	DWORD EnumKeys (LPCSTR pszCat, CDynaList<DWORD> & keys);
	// return the fully-qualified name of the category
	CString GetCat (DWORD dwIndex, char chSep = HW_SEP);
	// return the name of a key
	CString GetKey (DWORD dwIndex);
	// return a stored value
	CString GetVal (DWORD dwIndex);
	// Set Base Section
	void	SetBaseSection (LPCSTR strBaseSection)
	{ m_strSection = strBaseSection; }
private:
	CString	m_strSection;
};

#endif//#ifndef _HWDATA_DEF_
