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
	for (int index=0; index<_alertNotifications.GetCount(); index++)
	{
		delete _alertNotifications[index];
	}
	
	_alertNotifications.Empty();
	
	// Clean up the writer if allocated
	if (m_pWriter != NULL)
		delete m_pWriter;	
}



//
//    Write
//    =====
//
//    Save the alert notifications to the specified disk file
//
int CAlertNotificationFile::Write (CString& fileName)
{
	// Create the xmlFile output object
	m_pWriter = new CMarkup();
	m_pWriter->SetDoc("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" );

	// Now the Scanner Configuration Section
	m_pWriter->AddElem(S_ALERTNOTIFICATION_FILE);

	// Add the name of the asset to the top level section
	m_pWriter->AddChildElem(V_ASSET_NAME, _assetName);
	
	// Now add the AlertNotifications, one at a time
	for (int index=0; index<_alertNotifications.GetCount(); index++)
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
	m_pWriter->AddChildElem(V_ALERT_OLDVALUE, pNotification->OldValue());
	m_pWriter->AddChildElem(V_ALERT_NEWVALUE, pNotification->NewValue());
	//
	m_pWriter->OutOfElem();

	return;
}

