#pragma once


/////////////////////////////////////////////////////////////////////////////
//
//    CApplicationSerial Class
//
class CApplicationSerial
{
public:
	CApplicationSerial	(void);
	CApplicationSerial	(CString& name, CString& identifier, CString& productID, CString& cdKey);
	~CApplicationSerial	(void);

	// copy constructor
	CApplicationSerial(CApplicationSerial const & other)
	{ 
		_applicationName= other._applicationName; 
		_cdKey			= other._cdKey; 
		_identifier		= other._identifier; 
		_matched		= other._matched; 
		_productId		= other._productId;  
		_registryKey	= other._registryKey;
		_registryValue	= other._registryValue;
	}

// Data Accessors
public:
		CString ApplicationName ()
		{ return _applicationName; }
		void ApplicationName (CString& value)
		{ _applicationName = value; }

		CString CdKey ()
		{ return _cdKey; }
		void CdKey (CString& value)
		{ _cdKey = value; }

		CString Identifier ()
		{ return _identifier; }
		void Identifier (CString& value)
		{ _identifier = value; }

		BOOL Matched ()
		{ return _matched; }
		void Matched (BOOL value)
		{ _matched = value; }

		CString ProductId ()
		{ return _productId; }
		void ProductId (CString& value)
		{ _productId = value; }

		CString RegistryKey ()
		{ return _registryKey; }
		void RegistryKey (CString& value)
		{  _registryKey = (value.Left(18) == "HKEY_LOCAL_MACHINE") ? value.Mid(18) : value; }

		CString RegistryValue ()
		{ return _registryValue; }
		void RegistryValue (CString& value)
		{ _registryValue = value; }

// Methods
public:

protected:

private:
	CString		_applicationName;
	CString		_cdKey;
	CString		_identifier;
	BOOL		_matched;
	CString		_productId;
	CString		_registryKey;
	CString		_registryValue;
};



/////////////////////////////////////////////////////////////////////////////
//
//    CApplicationSerials Class
//
class CApplicationSerials
{
public:
	CApplicationSerials();
	~CApplicationSerials	(void);

public:
	// Return a reference to our internal list
	CDynaList<CApplicationSerial>* ApplicationSerials ()
	{ return &_listApplicationSerials; }

	// Find an application in the list
	CApplicationSerial* ContainsApplication	(CString& application);
	
	// Find an identifier in the list
	CApplicationSerial* ContainsIdentifier	(CString& identifier);

	// Find an entry which maps the specified registry key
	CApplicationSerial* ContainsRegistryKey	(CString& registryKey);

	// Detect possible product keys in the registry
	int Detect	(CAuditScannerConfiguration* pAuditScannerConfiguration);

	void Dump();

protected:
	void	FillGuidDictionary		();
	void	FindAllProductSerials	(HKEY hKey ,CString& fullKeyName);
	BOOL	IsSerialNumber			(CString& valueName);
	CString	GetDigitalProductId		(CString& keyName);
	void	ReadSerials				(CAuditScannerConfiguration* pAuditScannerConfiguration);
	CString	FindOfficeProductName	(CString& strKeyName ,CString& productGUID);

private:
	int _currentDepth;
	Dictionary<CString, CString> _guidMappings;
	int _keysScanned;
	int _valuesScanned;

	// The list of ApplicationSerial objects
	CDynaList<CApplicationSerial> _listApplicationSerials;

	// This is a map which stores the SKU and name of Microsoft Office XP variants
	CMap<CString, LPCSTR, CString, CString&> m_mapOfficeXP;

	// This is a map which stores the SKU and name of Microsoft Office 2003 variants
	CMap<CString, LPCSTR, CString, CString&> m_mapOffice2003;

	// This is a map which stores the SKU and name of Microsoft Office 2007 variants
	CMap<CString, LPCSTR, CString, CString&> m_mapOffice2007;
};