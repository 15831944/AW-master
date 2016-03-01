#include "stdafx.h"
#include "AssetType.h"
static char PRODUCTOPTIONSKEY[]			= "SYSTEM\\CurrentControlSet\\Control";

// Flags for battery flags
#define BATTERY_HIGH	1
#define BATTERY_LOW		2
#define BATTERY_CRITICAL 4
#define BATTERY_CHARGING 8

CAssetType::CAssetType(void)
{
	CLogFile log;
	BOOL lServerType;

	_assetType = "";
	lServerType = FALSE;
	
	// We assume that this asset is a PC unless we detect otherwise...
	log.Format("CAssetType, trying to detect asset type\n");

	HKEY hKey;
	if (RegOpenKeyEx (HKEY_LOCAL_MACHINE, PRODUCTOPTIONSKEY, 0, KEY_READ, &hKey) == ERROR_SUCCESS) 
	{
		log.Format("Opened product options key\n");
		
		// Read the value of 'ProductType' to see whether we are a domain controller / server
		CString version = CReg::GetItemString(hKey ,"ProductOptions" ,"ProductType");
		if (version == "ServerNT")
		{
			_assetType = "Server";
			lServerType = TRUE;
		}
		else if (version == "LanmanNT")
		{
			_assetType = "Domain Controller";
			lServerType = TRUE;
		}

		log.Format("product options value is %s  Asset Type is %s\n", version ,_assetType);
	}
	else
	{
		log.Format("CAssetType Failed to open registry\n");	
	}

	// not a server so check for battery, likely to be laptop
	// JML TODO - this is not 100% reliable and needs more work
	if (!lServerType)
	{
		// If the OS indicates a server then we will skip the Laptop checks as this is unlikely
		COsInfo os;
		CString osName = os.GetName();
		osName.MakeUpper();
		bool isLaptop = false;
		
		// First check to see if this is a laptop
		SYSTEM_POWER_STATUS status;
		if (GetSystemPowerStatus(&status))
		{
			log.Format("GetSystemPowerStatus worked\n");
			log.Format("GetSystemPowerStatus Battery Flag is %d\n",status.BatteryFlag);
			log.Format("GetSystemPowerStatus ACLineStatus is %d\n",status.ACLineStatus);
			if ((status.BatteryFlag & (BATTERY_HIGH | BATTERY_LOW |BATTERY_CRITICAL | BATTERY_CHARGING)) != 0)
			{
				log.Format("GetSystemPowerStatus worked - battery found so we assume a laptop\n");
				isLaptop = true;
			}
			else if (status.ACLineStatus != 255)
			{
				log.Format("GetSystemPowerStatus worked - ACLineStatus <> 255 so we assume a laptop\n");
				isLaptop = true;
			}
		}

		if (isLaptop)
		{
			_assetType = "Laptop";
		}

	}

	if (_assetType == "")
	{
		// not found anything so set to PC
		_assetType = "PC";
	}

}

CAssetType::~CAssetType(void)
{
}
