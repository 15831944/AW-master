#pragma once

#include "AuditScannerConfiguration.h"
#include "ApplicationSerial.h"

class CApplicationInstance
{
public:
	CApplicationInstance(void);

	// Fields
protected:
	CString		_guid;
	int			_applicationID;
	CString		_name;
	CString		_publisher;
	CApplicationSerial _serial;
	CString		_source;
	CString		_version;

public:
	// Properties
	// This is the product GUID if any held in the registry for this application
	CString Guid ()
	{ return _guid; }
	void	Guid (CString& value)
	{ _guid = value; }

	//Name of the application of which this is an instance
	CString Name ()
	{ return _name; }
	void Name (CString value)
	{ _name = value; }

	//Name of the publisher of this application
	CString Publisher ()
	{ return _publisher; }
	void Publisher (CString value)
	{ _publisher = value; }

	//Serial number object for this instance
	CApplicationSerial Serial ()
	{ return _serial; }
	void Serial (CApplicationSerial value)
	{ _serial = value; }

	//Where this instance was identified
	CString Source ()
	{ return _source; }
	void Source (CString value)
	{ _source = value; }

	//Specific version of the application of which this is an instance
	CString Version ()
	{ return _version; }
	void Version (CString value)
	{ _version = value; }
};


/////////////////////////////////////////////////////////////////////////
//
//    CApplicationInstanceList Class
//    ==============================
//
//    This class implements a list of CApplicationInstance objects
//
class CApplicationInstanceList
{
public:
	// Add an instance of an application to the list
	void AddApplicationInstance					(CString name, CString publisher, CString guid, CString serialNumber, CString version);

	// Return an instance for the specified application
	CApplicationInstance* ContainsApplication	(CString& value);

	// Return an instance for the specified GUID
	CApplicationInstance* ContainsGuid			(CString& value);

	// Set the GUID value for the specified application
	CApplicationInstance* SetGuid				(CString& application, CString& value);

	// Set the Serial value for the specified application
	CApplicationInstance* SetSerial				(CString& application, CApplicationSerial* pSerial);

	// Set the version for the specified application
	CApplicationInstance* SetVersion			(CString& application, CString& value);

	// Return a count of the total number of instances found
	int	Count()
	{ return _listApplicationInstances.GetCount(); }

	// Return the application instance with the specified index
	CApplicationInstance*	GetAt				(int index)
	{
		if (index < 0 || index > (int)_listApplicationInstances.GetCount() - 1)
			return NULL;
		else
			return &_listApplicationInstances[index];
	}

	// Override the array Operator to make returning elements easier
	CApplicationInstance* operator[] (int index)
	{ return GetAt(index); }

	// Detection Routines
	int Detect	(CAuditScannerConfiguration* pAuditScannerConfiguration);

protected:
	BOOL		IsInstalledApplication	(HKEY hKey ,CString& subKeyName);
	CString		GetApplicationName		(HKEY hKey ,CString& subKeyName ,CString& valueName);

	// This function allows us to rationalize an application publisher name
	CString	RationalizePublisher (CString& thePublisher);
	//
	void		ScanUninstallKey		(HKEY rootKey);
	void		ScanWindowsInstaller	(HKEY rootKey);
	void		ScanExceptions			();

protected:
	// Fields
	CDynaList<CApplicationInstance>	_listApplicationInstances;
	CApplicationSerials				_applicationSerials;
	Dictionary<CString, CString>	_publisherMappings;
	CAuditScannerConfiguration*		_pAuditScannerConfiguration;
};