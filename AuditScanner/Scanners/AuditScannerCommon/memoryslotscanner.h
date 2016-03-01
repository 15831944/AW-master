#pragma once
#include "auditdatascanner.h"

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a Memory Slot
//
class CMemorySlot
{
public:
	CMemorySlot(void);
	~CMemorySlot(void)
	{}

// Data accessors
public: 
	CString& Name (void)
	{ return _name; }
	void	Name	(LPCSTR value)
	{ _name = value; }
	//
	int	FormFactor(void)
	{ return _formFactor; }
	void	FormFactor (int value)
	{ _formFactor = value; }
	//
	int	Type(void)
	{ return _type; }
	void	Type (int value)
	{ _type = value; }
	//
	DWORD		Speed(void)
	{ return _speed; }
	void	Speed (DWORD value)
	{ _speed = value; }
	//
	DWORD	Capacity (void)
	{ return _capacity; }
	void	Capacity (DWORD value)
	{ _capacity = value; }

	CString FormFactorAsString(void)
	{
		CString strFormFactor = UNKNOWN;
		switch (_formFactor)
		{
			case 0:
				return "Unknown";
			case 1:
				return "Other";
			case 2:
				return "SIP";
			case 3:
				return "DIP";
			case 4:
				return "ZIP";
			case 5:
				return "SOJ";
			case 6:
				return "Proprietary";
			case 7:
				return "SIMM";
			case 8:
				return "DIMM";
			case 9:
				return "TSOP";
			case 10:
				return "PGA";
			case 11:
				return "RIMM";
			case 12:
				return "SODIMM";
			case 13:
				return "SRIMM";
			case 14:
				return "SMD";
			case 15:
				return "SSMP";
			case 16:
				return "QFP";
			case 17:
				return "TQFP";
			case 18:
				return "SOIC";
			case 19:
				return "LCC";
			case 20:
				return "PLCC";
			case 21:
				return "BGA";
			case 22:
				return "FPGCA";
			case 23:
				return "LGA";
			default:
				CString strValue;
				strValue.Format("unknown form [%lu]", _formFactor);
				return strValue;
		}
	}

	CString TypeAsString(void)
	{
		switch (_type)
		{
			case 0:
				return "Unknown";
			case 1:
				return "Other";
			case 2:
				return "DRAM";
			case 3:
				return "Synchronous DRAM";
			case 4:
				return "Cache DRAM";
			case 5:
				return "EDO";
			case 6:
				return "EDRAM";
			case 7:
				return "VRAM";
			case 8:
				return "SRAM";
			case 9:
				return "RAM";
			case 10:
				return "ROM";
			case 11:
				return "Flash";
			case 12:
				return "EEPROM";
			case 13:
				return "FEPROM";
			case 14:
				return "EPROM";
			case 15:
				return "CDRAM";
			case 16:
				return "3DRAM";
			case 17:
				return "SDRAM";
			case 18:
				return "SGRAM";
			case 19:
				return "RDRAM";
			case 20:
				return "DDR";
			default:
				CString strValue;
				strValue.Format("unknown form [%lu]", _type);
				return strValue;
		}
	}

// Internal Data
private:
	DWORD		_capacity;
	DWORD		_speed;
	CString		_name;
	int			_formFactor;
	int			_type;
};



////////////////////////////////////////////////////////////////////////////
//
//    Memory Slot Scanner
//
class CMemorySlotScanner : public CAuditDataScanner
{
public:
	CMemorySlotScanner();
	~CMemorySlotScanner(void);


// Base class over-rides
public:
	bool	ScanWMI			(CWMIScanner* pScanner);
	bool	ScanRegistryXP	(void);
	bool	ScanRegistryNT	(void);
	bool	ScanRegistry9X	(void);
	bool	SaveData		(CAuditDataFile* pAuditDataFile);

private:
	CDynaList<CMemorySlot>	_listMemorySlots;
};
