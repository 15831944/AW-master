
// FILE:	InstalledPatch.cpp
// PURPOSE:	Implementation of code to recover MS installed patches from registry
// AUTHOR:	C.M.Drew - Copyright (C) Layton Technology 2003
// HISTORY:	CMD	- 03.12.2003 - written

#include "stdafx.h"
#include "InstalledPatches.h"
#include "QuickFixScanner.h"

static char Win2000Patches[] = "SOFTWARE\\Microsoft\\Updates";
static char WinNTPatches[] = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Hotfix";

/*
**	List installed patch in the form "Product|Description|ServicePack|Name|InstallDate|InstalledBy"
*/
CString	CInstalledPatch::List (char chSep/* = '|'*/) const
{
	CString strDetails;
	strDetails.Format ("%s%c%s%c%s%c%s%c%s%c%s", m_strProduct, chSep, m_strDescription, chSep, m_strServicePack, chSep, m_strName, chSep, m_strInstallDate, chSep, m_strInstaller);
	return strDetails;
}

/*
**	Scan for installed Microsoft Patches
*/
int CInstalledPatchList::Scan (CWMIScanner* pScanner)
{
	CLogFile log;

	// Ensure that the list is empty
	Empty();

	// ...and run the scan
	COsInfo os;
	if (os.GetClass() == COsInfo::winNT4)
	{
		ScanNT4 ();
		log.Format("Found %s OS and scanned \n", os.GetName());
	}
	else if (os.IsNT())
	{
		log.Format("Found %s OS and scanned \n", os.GetName());
		Scan2000 ();		
	}
	else
		log.Format("Found %s OS and not scanned yet \n", os.GetName());

	//Vista and above os 
	//This portion is added as a fix for getting the patches in vista and win7
	//Added by Sojan E John, KTS Infotech

	if( os.VersionMajor() >=6)
	{
		log.Format("Os version is vista or above scanning for Quick using WMI \n" );
		//scan using wmi here for installed patch if vista or higher
		CQuickFixScanner* objCQuickFixScanner= new CQuickFixScanner();
		//CAuditDataScanner* objSoftware=new CQuickFixScanner();
		CAuditDataScanner* pThisScanner= objCQuickFixScanner;
		pThisScanner->Scan(pScanner);

		CDynaList<CQuickFix> objlistQuickFix=objCQuickFixScanner->QuickFixList();
		if (objlistQuickFix.GetCount() != 0)
		{
			for (int isub=0; isub<(int)objlistQuickFix.GetCount(); isub++)
			{
				CQuickFix* pQuickFix= &objlistQuickFix[isub];
				
				CInstalledPatch Patch;
				Patch.Product ("Microsoft Windows");
				Patch.ServicePack (pQuickFix->ServicePack());
				Patch.Name (pQuickFix->HotFixID());
				Patch.Description(pQuickFix->Description());
				Patch.InstallDate(pQuickFix->InstalledON());
				Patch.Installer(pQuickFix->InstalledBy());

				// Add the patch (subject to duplicate rules)
				AddPatch(Patch);
			}
		}
		
		log.Format("finished scanning [%s]\n" ,pThisScanner->ItemName());
	}

	return GetCount ();
}

/*
**	Scan for installed patches on a Windows NT4 platform
*/
void CInstalledPatchList::ScanNT4 ()
{
	CReg regKey;
	CString	strKey, strProduct;
	CReg topkey, subkey;

	if (topkey.Open(HKEY_LOCAL_MACHINE, WinNTPatches))
	{
		CDynaList<CString> subKeys;
		topkey.EnumSubKeys(subKeys);
		for (DWORD dw = 0 ; dw < subKeys.GetCount() ; dw++)
		{
			strKey.Format("%s\\%s", WinNTPatches, subKeys[dw]);
			if (regKey.Open(HKEY_LOCAL_MACHINE, strKey))
			{
				CString strDescription = regKey.ReadValueStr("Fix Description");
				if (!strDescription.IsEmpty())
				{
					CInstalledPatch Patch;
					Patch.Product("");
					Patch.ServicePack("");
					Patch.Name(subKeys[dw]);
					Patch.Description (strDescription);
					Patch.InstallDate (regKey.ReadValueStr ("Installed On"));
					Patch.Installer (regKey.ReadValueStr ("Installed By"));

					// Add the patch (subject to duplicate rules)
					AddPatch(Patch);
				}
				regKey.Close();
			}
		}
		topkey.Close();
	}
}

/*
**	Scan for installed patches on a Windows 2000 onwards platform
*/
void CInstalledPatchList::Scan2000 ()
{
	CReg topkey, subkey;
	COsInfo os;
	CString strProduct;

	if (topkey.Open(HKEY_LOCAL_MACHINE, Win2000Patches))
	{
		CDynaList<CString> subKeys;
		topkey.EnumSubKeys(subKeys);
		
		for (DWORD dw = 0 ; dw < subKeys.GetCount() ; dw++)
		{
			ScanProduct (subKeys[dw]);
		}
		topkey.Close();
	}
}

/*
**	Helper to scan a specific registry key for patch details
*/
void CInstalledPatchList::ScanProduct (LPCSTR strProduct)
{
	CReg regKey;
	CString	strKey;
	strKey.Format ("%s\\%s", Win2000Patches, strProduct);

	// emumerate keys beneath the product key
	if (regKey.Open (HKEY_LOCAL_MACHINE, strKey))
	{
		CDynaList<CString> subKeys;
		regKey.EnumSubKeys (subKeys);
		for (DWORD dw = 0 ; dw < subKeys.GetCount() ; dw++)
		{
			// Is this a service pack? If so then we must enumerate it also to get to the patches
			if (subKeys[dw].Left(2) == "SP")
				ScanServicePack (strProduct, subKeys[dw]);
			else
				GetPatchDetails (strProduct, "", subKeys[dw]);
		}
		regKey.Close();
	}
}



/*
**	Scan patches beneath a specific service pack
*/
void CInstalledPatchList::ScanServicePack (LPCSTR strProduct, LPCSTR strServicePack)
{
	CLogFile log;
	CReg regKey;
	CString	strKey;
	strKey.Format ("%s\\%s\\%s", Win2000Patches, strProduct, strServicePack);			

	// emumerate keys beneath the service pack key - these are the patches themselves
	if (regKey.Open(HKEY_LOCAL_MACHINE, strKey))
	{
		CDynaList<CString> subKeys;
		regKey.EnumSubKeys(subKeys);
		for (DWORD dw = 0 ; dw < subKeys.GetCount() ; dw++)
		{
			// Format the key name
			GetPatchDetails (strProduct, strServicePack, subKeys[dw]);
		}
		regKey.Close();
	}
	
}

/*
**	Get the details of a specific patch
*/
void CInstalledPatchList::GetPatchDetails (LPCSTR strProduct, LPCSTR strServicePack, LPCSTR strPatch)
{
	CReg regKey;
	CString	strKey;
	CString strSP = strServicePack;

	if (strSP.IsEmpty())
		strKey.Format ("%s\\%s\\%s", Win2000Patches, strProduct, strPatch);
	else
		strKey.Format ("%s\\%s\\%s\\%s", Win2000Patches, strProduct, strServicePack, strPatch);

	// Open the patch key
	if (regKey.Open(HKEY_LOCAL_MACHINE, strKey))
	{
		CInstalledPatch Patch;
		Patch.Product (strProduct);
		Patch.ServicePack (strServicePack);
		Patch.Name (strPatch);
		Patch.Description (regKey.ReadValueStr ("Description"));
		Patch.InstallDate (regKey.ReadValueStr ("InstalledDate"));
		Patch.Installer (regKey.ReadValueStr ("InstalledBy"));

		// Add the patch (subject to duplicate rules)
		AddPatch(Patch);
		
		// Close this registry key
		regKey.Close ();
	}
}


		
void CInstalledPatchList::AddPatch (CInstalledPatch& newPatch)
{
	// Add this patch to our list noting that we need to ensure that we do not add the same named patch twice 
	// as this will cause us problems later on when we come to upload. We check for a duplicate product and patch
	// name.  Usually this is caused by a patch being listed individually and as part of a SP so we can delete the
	// duplicate (actually we ensure that we retain the Service Pack entry)
	BOOL matchFound = FALSE;
	for (int index =0; index < (int)GetCount(); index++)
	{
		CInstalledPatch* thisPatch = &(m_pData[index]);
		if ((thisPatch->Product() == newPatch.Product())
		&&  (thisPatch->Name() == newPatch.Name()))
		{
			if ((thisPatch->ServicePack() == "") && (newPatch.ServicePack() != ""))
				thisPatch->ServicePack(newPatch.ServicePack());
			matchFound = TRUE;
			break;
		}
	}
		
	if (!matchFound)
		Add (newPatch);
}