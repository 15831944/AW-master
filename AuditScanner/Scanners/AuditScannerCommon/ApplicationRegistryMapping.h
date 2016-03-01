#pragma once

///////////////////////////////////////////////////////////////////
//
//    CRegistryMapping
//    ================
//
//    This class represents a registry key/value pair
//
class CRegistryMapping
{
public:
	CRegistryMapping(void)
	{
		_registryKey = "";
		_valuename = "";
	}

	CRegistryMapping(CString& keyname ,CString& valuename)
	{
		_registryKey = keyname;
		_valuename = valuename;
	}

// Data Accessor Functions
public:
	CString RegistryKey ()
	{ return _registryKey; }
	void	RegistryKey (CString& value)
	{ _registryKey = value; }

	CString ValueName ()
	{ return _valuename; }
	void ValueName (CString& value)
	{ _valuename = value; }

private:
	CString _registryKey;
	CString _valuename;
};



///////////////////////////////////////////////////////////////////
//
//    CApplicationRegistryMapping
//    ===========================
//
//    This class represents a mapping between a named application and
//    one or more registry key/value pairs which define the location in
//    the windows registry where a product key for this application MAY 
//    be located.
//
class CApplicationRegistryMapping
{
public:
	CApplicationRegistryMapping(void);
	CApplicationRegistryMapping(CString& applicationName);

// Data Accessor functions
public:
	CString&	ApplicationName ()
	{ return _applicationName; }
	void		ApplicationName (CString& value)
	{ _applicationName = value; }

// Methods
public:
	CDynaList<CRegistryMapping>& GetMappings ()
	{ return _registryMappings; }

	// Add a new mapping to our internal list
	void AddMapping(CString& mappingString);
	
	// Add a new mapping to our internal list
	void AddMapping(CString& registryKey ,CString& registryValue);


// Data 
private:
	CString		_applicationName;

	// The internal list of registry mappings
	CDynaList<CRegistryMapping> _registryMappings;
};
