#pragma once
#include "auditdatascanner.h"

////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of a Network Adapter
//
class CNetworkAdapter
{
public:
	CNetworkAdapter(void);
	CNetworkAdapter(CString& productName ,CString& manufacturer ,CString& macAddress ,CString& ipAddress ,CString& type ,DWORD speed ,bool dhcpEnabled)
	{ _productName = productName; _manufacturer = manufacturer; _macAddress = macAddress; _ipAddress = ipAddress; _type = type ,_speed = speed; _dhcpEnabled = dhcpEnabled;}
	~CNetworkAdapter(void)
	{}

// Data accessors
public: 
	CString&	ProductName (void)
	{ return _productName; }
	void		ProductName	(LPCSTR value)
	{ _productName = value; }
	//
	CString&	Manufacturer (void)
	{ return _manufacturer; }
	void		Manufacturer	(LPCSTR value)
	{ _manufacturer = value; }
	//
	CString&	MacAddress (void)
	{ return _macAddress; }
	void		MacAddress	(LPCSTR value)
	{ _macAddress = value; }
	//
	CString&	IPAddress (void)
	{ return _ipAddress; }
	void		IPAddress	(LPCSTR value)
	{ _ipAddress = value; }
	//
	CString&	IPSubNet (void)
	{ return _ipSubNet; }
	void		IPSubNet	(LPCSTR value)
	{ _ipSubNet = value; }
	//
	CString&	Type(void)
	{ return _type; }
	void		Type (LPCSTR value)
	{ _type = value; }
	//
	DWORD		Speed(void)
	{ return _speed; }
	void		Speed (DWORD value)
	{ _speed = value; }
	//
	bool		DHCPEnabled(void)
	{ return _dhcpEnabled; }
	void		DHCPEnabled (bool value)
	{ _dhcpEnabled = value; }


// Internal Data
private:
	CString		_productName;
	CString		_manufacturer;
	CString		_macAddress;
	CString		_ipAddress;
	CString		_ipSubNet;
	CString		_type;
	DWORD		_speed;
	bool		_dhcpEnabled;
	DWORD		_status;
};



////////////////////////////////////////////////////////////////////////////
//
//    Network Adapters Scanner
//
class CNetworkAdaptersScanner : public CAuditDataScanner
{
public:
	CNetworkAdaptersScanner();
	~CNetworkAdaptersScanner(void);


// Base class over-rides
public:
	bool	ScanWMI			(CWMIScanner* pScanner);
	bool	ScanRegistryXP	(void);
	bool	ScanRegistryNT	(void);
	bool	ScanRegistry9X	(void);
	bool	SaveData		(CAuditDataFile* pAuditDataFile);

// Public functions
public:
	CNetworkAdapter*	FindAdapter (CString& strIPAddress);

protected:
	// Recover network adapters if WMI has failed
	bool	GetNetworkAdapters	();

private:
	CDynaList<CNetworkAdapter>	_listNetworkAdapters;
};
