#include "stdafx.h"
#include "AuditDataFile.h"
#include "WMIScanner.h"

// The various classes which we can collect
#include "SystemBiosScanner.h"
#include "SystemProcessorScanner.h"

// If we cannot recover the value of a field we set it to be 'n/a'
#define UNKNOWN "<not specified>"

CWMIScanner::CWMIScanner(void)
{
	// Show not connected
	m_bConnected = FALSE;
}


CWMIScanner::~CWMIScanner(void)
{
	// Just disconnect
	Disconnect();
}


BOOL CWMIScanner::Connect()
{
	CLogFile log;

	try
	{
		// Do not connect if already connected
		if (m_bConnected)
			return TRUE;

		// Connect to the root namespace
		CString	strCimRoot = "\\\\.\\root\\cimv2";			
		if (!m_WMI.Connect(strCimRoot))
		{
			// Unable to connect to WMI => no WMI support
			log.Format("Failed to connect to the WMI namespace. Reason: (0x%lX)\n", m_WMI.GetLastErrorWMI());
			return FALSE;
		}

		m_bConnected = TRUE;
		return TRUE;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		log.Write("Failed to connect to the WMI namespace. An unknown exception occured");
		return FALSE;
	}
}


//
//    Disconnect
//    ==========
//
//    Disconnect from WMI
//
BOOL CWMIScanner::Disconnect()
{
	//CLaytonWMI m_WMI;
	if (m_bConnected)
	{
		//m_WMI.Disconnect();
		m_bConnected = FALSE;
	}
	return TRUE;
}

//
//    IsConnected
//    ===========
//
//    Determine whether or not we are connected to WMI
//
BOOL CWMIScanner::IsConnected()
{
	return m_bConnected;
}


//
//    GetBiosInformation
//    ==================
//
//    This routine returns information about the system BIOS using WMI calls
//
BOOL CWMIScanner::GetBiosInformation(CSystemBiosScanner* pSystemBios)
{
	CLogFile log;

	// Temporary work data
	CString strManufacturer = "";					// CMD 8.3.4.1 (Used to be (UNKNOWN) - changed as causes errors I think)
	CString	strModel = "";							// CMD 8.3.4.1
	CString	strSerial = "";							// CMD 8.3.4.1
	//
	CString strBiosManufacturer = "";				// CMD 8.3.4.1
	CString strBiosModel = "";						// CMD 8.3.4.1
	CString strBiosVersion = "";					// CMD 8.3.4.1
	CString strBiosDate = "";						// CMD 8.3.4.1

	// If not WMI connected => cannot do this
	if (!m_bConnected)
		return FALSE;
	
	// First of all let's recover some general details about the Computer System such as it's make, model 
	// and serial number.  This information can be held in a few locations so we shall try to read a few 
	// different WMI namespaces to pickup as much data as possible
	try
	{
		if (m_WMI.BeginEnumClassObject("Win32_ComputerSystem"))
		{
			while (m_WMI.MoveNextEnumClassObject())
			{
				strManufacturer = m_WMI.GetClassObjectStringValue("Manufacturer");
				strModel		= m_WMI.GetClassObjectStringValue("Model");
			}
			m_WMI.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		pEx->Delete(); 
		log.Write("Failed to obtain system BIOS information from WMI because of an unknown exception accessing 'Win32_ComputerSystem'");
	}

		// Now we shall recover details of the BIOS itself...
	try
	{
		if (m_WMI.BeginEnumClassObject("Win32_Bios"))
		{
			while (m_WMI.MoveNextEnumClassObject())
			{
				strBiosManufacturer = m_WMI.GetClassObjectStringValue("Manufacturer");
				strBiosVersion		= m_WMI.GetClassObjectStringValue("BIOSVersion");

				// If BIOSVersion is not set then just read Version
				if (strBiosVersion.IsEmpty())
					strBiosVersion = m_WMI.GetClassObjectStringValue("Version");

				// Recover the BIOS serial number also - this is probably the same as that above
				//if (strSerial.IsEmpty() || strSerial == "None")
				
				strSerial = m_WMI.GetClassObjectStringValue("SerialNumber");
			}
		}
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		log.Write("Failed to obtain system BIOS information from WMI because of an unknown exception accessing 'Win32_Bios'");
		return FALSE;
	}

		// If we still have missing information then our last chance is to recover from the Win32_Baseboard
	if (strManufacturer.IsEmpty() || strModel.IsEmpty() || strSerial.IsEmpty())
	{
		try
		{
			if (m_WMI.BeginEnumClassObject("Win32_BaseBoard"))
			{
				while (m_WMI.MoveNextEnumClassObject())
				{
					if (strManufacturer.IsEmpty())
						strManufacturer = m_WMI.GetClassObjectStringValue("Manufacturer");
					//
					if (strModel.IsEmpty())
						strModel = m_WMI.GetClassObjectStringValue("Product");
					//
					if (strSerial.IsEmpty() || strSerial == "None")
						strSerial = m_WMI.GetClassObjectStringValue("SerialNumber");
				}
				m_WMI.CloseEnumClassObject();
			}
		}
		catch (CException *pEx)
		{
			pEx->Delete();
			log.Write("Failed to obtain system BIOS information from WMI because of an unknown exception accessing 'Win32_BaseBoard'");
		}
	}

	// Try to use system enclosure object to get System Manufacturer, Model, S/N and chassis type
	// noting that we will not over-write any values previously recovered above
	try
	{
		if (m_WMI.BeginEnumClassObject( _T( "Win32_SystemEnclosure")))
		{
			while (m_WMI.MoveNextEnumClassObject())
			{
				// If we do not already have the manufacturer then recover it here
				if (strManufacturer.IsEmpty())
					strManufacturer = m_WMI.GetClassObjectStringValue("Manufacturer");

				// ...and recove the model if we don't already have it
				if (strModel.IsEmpty())
					strModel = m_WMI.GetClassObjectStringValue("Model");

				// Recover serial number 
				strSerial = m_WMI.GetClassObjectStringValue("SerialNumber");
			}
			m_WMI.CloseEnumClassObject();
		}
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		log.Write("Failed to obtain system BIOS information from WMI because of an unknown exception accessing 'Win32_SystemEnclosure'");
	}

	// Now store the recovered information back into the supplied BIOS object
	pSystemBios->SystemManufacturer(strManufacturer);
	pSystemBios->SystemModel(strModel);
	pSystemBios->SystemSerialNumber(strSerial);
	pSystemBios->BiosManufacturer(strBiosManufacturer);
	pSystemBios->BiosDate(strBiosDate);
	pSystemBios->BiosVersion(strBiosVersion);

	// ...and return the result
	return TRUE;
}



//
//    GetDomainInformation
//    ====================
//
//    This function returns either the current Windows domain or Workgroup if the 
//    computer is not part of a domain
//
BOOL CWMIScanner::GetDomainInformation(CString &strDomain)
{
	CLogFile log;
	BOOL	bResult = FALSE;

	// If not WMI connected => cannot do this
	if (!m_bConnected)
		return FALSE;

	// Windows domain information is held within the Win32_ComputerSystem 
	try
	{
		if (m_WMI.BeginEnumClassObject("Win32_ComputerSystem"))
		{
			while (m_WMI.MoveNextEnumClassObject())
			{
				strDomain = m_WMI.GetClassObjectStringValue("Domain");
			}
			m_WMI.CloseEnumClassObject();
			bResult = TRUE;
		}
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		log.Write("Failed to obtain the Windows Domain / Workgroup information from WMI because of an unknown exception accessing 'Win32_Bios'");
		return FALSE;
	}
	return bResult;
}



