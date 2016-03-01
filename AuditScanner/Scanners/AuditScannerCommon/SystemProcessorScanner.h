#pragma once

// Include our base class header
#include "AuditDataScanner.h"

//
class CSystemProcessorScanner : public CAuditDataScanner
{
public:
	CSystemProcessorScanner();
	~CSystemProcessorScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	ScanRegistryForNameSpeed (void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

// Data Accessor Functions
public:
	CString&	Name ()
	{ return m_strName; }
	void		Name (CString& value)
	{ m_strName = value; }
	//
	int			Speed()
	{ return m_nSpeed; }
	void		Speed(int value)
	{ m_nSpeed = value; }
	//
	CString&	Socket ()
	{ return m_strSocket; }
	void		Socket (CString& value)
	{ m_strSocket = value; }
	//
	int			Voltage()
	{ return m_nVoltage; }
	void		Voltage(int value)
	{ m_nVoltage = value; }
	//
	int			Processors()
	{ return m_nProcessors; }
	void		Processors(int value)
	{ m_nProcessors = value; }
	//
	int			Cores()
	{ return m_nCores; }
	void		Cores(int value)
	{ m_nCores = value; }
	//
	CString&	BusType ()
	{ return m_strBusType; }
	void		BusType (CString& value)
	{ m_strBusType = value; }
	
	// 8.3.4 - CMD
	CString&	Architecture()
	{ return m_strArchitecture; }
	void		Architecture (CString& value)
	{ m_strArchitecture = value; }
	
	// 8.3.4 - CMD
	CString&	SystemType()
	{ return m_strSystemType; }
	void		SystemType (CString& value)
	{ m_strSystemType = value; }

private:
	// Internal class data
	CString		m_strName;
	int			m_nSpeed;
	CString		m_strSocket;
	int			m_nVoltage;
	int			m_nProcessors;
	int			m_nCores;
	CString		m_strBusType;
	CString		m_strArchitecture;
	CString		m_strSystemType;
};
