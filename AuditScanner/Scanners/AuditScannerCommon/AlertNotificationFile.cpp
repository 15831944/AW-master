#include "stdafx.h"
#include "AlertNotificationFile.h"



//////////////////////////////////////////////////////////////////////////////////////////
//
//    CAlertNotificationFile
//    ======================
//
//    Constructor for the CAlertNotificationFile class
//
CAlertNotificationFile::CAlertNotificationFile(void)
{
	m_pWriter = NULL;
}

CAlertNotificationFile::~CAlertNotificationFile(void)
{
	// Free up any allocated Alert Notification objects dynamically allocated
	for (int index=0; index < (int)_alertNotifications.GetCount(); index++)
	{
		delete _alertNotifications[index];
	}
	
	_alertNotifications.Empty();
	
	// Clean up the writer if allocated
	if (m_pWriter != NULL)
		delete m_pWriter;	
}




//
//	Load
//  ====
//
//  Load the specified XML file into memory
//
//	Inputs:
//		pszPath - Fully qualified path to the XML file to be loaded
//
//  Returns:
//		-1 - File does not exist or failed to open file
//		0  - Success
//		1  - File exists but is in an invalid format
//
int CAlertNotificationFile::Load(LPCSTR pszPath)
{
	//_assetName = GetAssetName();
	CString fileName = _assetName + ".anf";

	fileName = MakePathName(pszPath ,fileName);

	// Now read the file
	CString csText;
	CFile file;
	if (!file.Open(fileName, CFile::modeRead))
		return -1;
	int nFileLen = (int)file.GetLength();

	// Allocate buffer for binary file data
	unsigned char* pBuffer = new unsigned char[nFileLen + 2];

	// ...and read the file into this buffer
	nFileLen = file.Read( pBuffer, nFileLen );
	file.Close();

	// Terminate the buffer
	pBuffer[nFileLen] = '\0';
	pBuffer[nFileLen+1] = '\0'; // in case 2-byte encoded
	csText = (LPCSTR)pBuffer;
	delete [] pBuffer;

	// Load the XML buffer into an internal XML object
	CMarkup xmlFile;
	if (!xmlFile.SetDoc(csText))
		return -1;

	// Ok the file has now been read - let's process it
	xmlFile.ResetPos();

	// We must have the main alert monitor section otherwise this is an invalid file
	if (!xmlFile.FindElem(S_ALERTNOTIFICATION_FILE))
		return -1;

	// Process the file as we can be confident that it is an Alert Notification File
	ProcessElementRead(xmlFile);
	return 0;
}



// 
//    ProcessElementRead
//    ==================
//
//    We have parsed the 'ScannerConfiguration' element so know that the XML file is an AuditWizard
//    Scanner Configuration file and can now continue to parse the items within this section noting 
//    that we terminate parsing when we reach the end of the section.
//
void CAlertNotificationFile::ProcessElementRead(CMarkup xmlFile)
{
	// Get the list of elements in this section
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == S_ALERT)
			ProcessAlertNotification(xmlFile);

		else if (elementName == V_ASSET_NAME)
			_assetName = xmlFile.GetChildData();
	}
}




// 
//    ProcessAlertNotification
//    ========================
//
//    We have parsed the 'Alert' element
//
void CAlertNotificationFile::ProcessAlertNotification(CMarkup xmlFile)
{
	// Step into the 'Alert Definitions' section
	xmlFile.IntoElem();

	CAlertNotification* pAlertNotification = new CAlertNotification();
	
	// Get the list of elements in this section - should be a list of <Alert Definition> 
	while (xmlFile.FindChildElem(""))
	{
		CString elementName = xmlFile.GetChildTagName();
		//
		if (elementName == V_ALERT_NAME)
			pAlertNotification->AlertName(xmlFile.GetChildData());
			
		else if (elementName == V_ALERT_TYPE)
			pAlertNotification->Type(xmlFile.GetChildData());
			
		else if (elementName == V_ALERT_CATEGORY)
			pAlertNotification->Category(xmlFile.GetChildData());
			
		else if (elementName == V_ALERT_KEY)
			pAlertNotification->Key(xmlFile.GetChildData());
			
		else if (elementName == V_ALERT_OLDVALUE)
			pAlertNotification->OldValue(xmlFile.GetChildData());
			
		else if (elementName == V_ALERT_NEWVALUE)
			pAlertNotification->NewValue(xmlFile.GetChildData());
	}

	_alertNotifications.Add(pAlertNotification);
	
	xmlFile.OutOfElem();
}


//
//    Write
//    =====
//
//    Save the alert notifications to the specified disk file
//
int CAlertNotificationFile::Write (CString& path)
{
	CString fileName = _assetName + ".anf";
	fileName = MakePathName(path ,fileName);
	
	// Create the xmlFile output object
	m_pWriter = new CMarkup();
	m_pWriter->SetDoc("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" );

	// Now the Scanner Configuration Section
	m_pWriter->AddElem(S_ALERTNOTIFICATION_FILE);

	// Add the name of the asset to the top level section
	m_pWriter->AddChildElem(V_ASSET_NAME, _assetName);
	
	// Now add the AlertNotifications, one at a time
	for (int index=0; index < (int)_alertNotifications.GetCount(); index++)
	{
		WriteAlert(_alertNotifications[index]);
	}
	
	// ...force the CMarkup object to actually write the data to disk
	CString data = m_pWriter->GetDoc();
	m_pWriter->Save(fileName ,data);

	// Delete the m_pWriter-> again
	delete m_pWriter;
	m_pWriter = NULL;

	// return success
	return 0;
}



//
//    WriteAlert
//    ==========
//
//    Write the 'Alert Notification' to the file
//
void CAlertNotificationFile::WriteAlert(CAlertNotification* pNotification)
{
	m_pWriter->AddChildElem(S_ALERT);		// ...add a 'General' section
	m_pWriter->IntoElem();
	//
	m_pWriter->AddChildElem(V_ALERT_NAME, pNotification->AlertName());
	m_pWriter->AddChildElem(V_ALERT_TYPE, pNotification->NotificationTypeAsString());
	m_pWriter->AddChildElem(V_ALERT_CATEGORY, pNotification->Category());
	m_pWriter->AddChildElem(V_ALERT_KEY, pNotification->Key());
	m_pWriter->AddChildElem(V_ALERT_OLDVALUE, pNotification->OldValue());
	m_pWriter->AddChildElem(V_ALERT_NEWVALUE, pNotification->NewValue());
	//
	m_pWriter->OutOfElem();

	return;
}

