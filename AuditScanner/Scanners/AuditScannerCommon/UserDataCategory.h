#pragma once

#include "AuditDataFile.h"

//
//    CUserDataField
//    ==============
//
//    Encapsulate a user data field
//
class CUserDataField
{
public:
	enum eDataType { typeText, typeNumeric, typePicklist, typeEnvVar, typeRegKey, typeDate, typeCurrency };
	enum eInputCase { any, upper, lower, title };
	
public:
	CUserDataField();
	CUserDataField(DWORD dwID, LPCSTR pszBuffer);
	
	// ACCESS
	HWND GetHandle() const				{ return m_hCtl; }
	void SetHandle(HWND hCtl)			{ m_hCtl = hCtl; }
	BOOL operator== (const CUserDataField & other);
	BOOL operator== (LPCSTR pszLabel)	{ return _label == pszLabel; }


// Data Accessor Functions
public: 
	CString&	Label (void)
	{ return _label; }
	void Label (LPCSTR value)
	{ _label = value; }

	int	 DataType (void)
	{ return _type; }
	void DataType (int value)
	{ _type = value; }
	
	BOOL Mandatory()
	{ return _mandatory; }
	void Mandatory (BOOL value)
	{ _mandatory = value; }
	
	// Return the Maximum Length for a text field
	int Length(void)
	{ return _length; }
	void Length (int value)
	{ _length = value; }
	
	// Return the Minimum numeric value permitted, if not specified return 0;
	//int MinimumValue(void)
	//{ return GetNumericValue(_value1 ,0); }
	//void MinimumValue (int value)
	//{ _value1.Format("%d" ,value); }

	///// <summary>
	///// Return the Maximum numeric value permitted, if not specified return 0;
	///// </summary>
	//int MaximumValue(void)
	//{ return GetNumericValue(_value2 ,0); }
	//void MaximumValue (int value)
	//{ _value2.Format("%d" ,value); }

	// Return the Minimum numeric value permitted, if not specified return 0;
	int MinimumValue(void)
	{ return _minValue; }
	void MinimumValue (int value)
	{ _minValue = value; }

	/// <summary>
	/// Return the Maximum numeric value permitted, if not specified return 0;
	/// </summary>
	int MaximumValue(void)
	{ return _maxValue; }
	void MaximumValue (int value)
	{ _maxValue = value; }

	// Accessor for Picklist (stored in value1)
	CString& Picklist (void)
	{ return _value1; }
	void Picklist(LPCSTR value)
	{ _value1 = value; }

	// Accessor for Environment Variable (stored in value2)
	CString& EnvironmentVariable (void)
	{ return _value2; }
	void EnvironmentVariable(LPCSTR value)
	{ _value2 = value; }

	// Accessor for Registry Key (stored in value1)
	CString& RegistryKey (void)
	{ return _value1; }
	void RegistryKey(LPCSTR value)
	{ _value1 = value; }

	// Accessor for Value1
	CString& Value1 (void)
	{ return _value1; }
	void Value1(LPCSTR value)
	{ _value1 = value; }

	// Accessor for Value2
	CString& Value2 (void)
	{ return _value2; }
	void Value2(LPCSTR value)
	{ _value2 = value; }

	// Accessor for Registry Value (stored in value2)
	CString& RegistryValue (void)
	{ return _value2; }
	void RegistryValue(CString& value)
	{ _value2 = value; }

	// Convert textual datatype to our internal type
	void	DataType (CString& type)
	{
		if (type == "text")
			_type = typeText;
		else if (type == "numeric")
			_type = typeNumeric;
		else if (type == "environment")
			_type = typeEnvVar;
		else if (type == "registry")
			_type = typeRegKey;
		else if (type == "picklist")
			_type = typePicklist;
		else if (type == "date")
			_type = typeDate;
		else if (type == "currency")
			_type = typeCurrency;
		else
			_type = typeText;	
	}

	void InputCase (CString& value)
	{
		if (value == "any")
			_inputCase = CUserDataField::any;
		else if (value == "lower")
			_inputCase = CUserDataField::lower;
		else if (value == "title")
			_inputCase = CUserDataField::title;
		else if (value == "upper")
			_inputCase = CUserDataField::upper;
		else
			_inputCase = CUserDataField::any;	
	}
	
	eInputCase InputCase (void)
	{ return _inputCase; }
	
	// This function sets or recovers the current user defined value for this field
	CString&	CurrentValue (void)
	{ return _currentValue; }
	void		CurrentValue (LPCSTR value)
	{ _currentValue = value; }
	
protected:
	// Helper function to convert a string to a numeric handling defaults
	int GetNumericValue (CString& valueString ,int defaultValue)
	{
		int returnValue = defaultValue;
		try
		{
			if (valueString != "")
				returnValue = atoi(valueString);
		}
		catch (CException*)
		{ }
		return returnValue;
	}

protected:
	HWND		m_hCtl;						// control window handle
	DWORD		m_dwDatabaseID;				// database ID as defined by INI file
	CString		_label;						// field label
	//CString		_envVariable;				// environmental variables
	int			_type;						// data type
	int			_length;					// max size of text field
	int			_minValue;					// min value
	int			_maxValue;					// max value
	eInputCase	_inputCase;					// Input case for text fields
	BOOL		_mandatory;					// TRUE if data collection is mandatory
	CString		_value1;					// validation parameter 1
	CString		_value2;					// validation parameter 2
	//
	CString		_currentValue;				// The current value set for this field
};



//
//    CUserDataCategory
//    =================
//
//    Encapsulate a category of user data, this is essentially a list of user data fields
//
class CUserDataCategory
{
public:
	CUserDataCategory(void);
public:
	virtual ~CUserDataCategory(void);

	// Find a named field in the list
	CUserDataField*	FindField (LPCSTR fieldName);
	
	// return the list of fields
	CDynaList<CUserDataField>&	ListDataFields()
	{ return _listDataFields; }

	// Return count of fields
	int		GetCount()
	{ return _listDataFields.GetCount(); }
	
	// Add a new field
	int		Add(CUserDataField& userDataField)
	{	return _listDataFields.Add(userDataField); } 
	
	
public:
	CString&	Name(void)
	{ return _name; }
	void		Name(CString& value)
	{ _name = value; }
	
private:
	 CDynaList<CUserDataField>	_listDataFields;
	CString		_name;
};





//
//    CUserDataCategoryList
//    =====================
//
//    Encapsulate a list of user data categories
//
class CUserDataCategoryList
{
public:
	CUserDataCategoryList(void)
	{}
	virtual ~CUserDataCategoryList(void)
	{}
	
public:
	CDynaList<CUserDataCategory>&	ListCategories()
	{ return _listCategories; }

	// Find a category within this list
	CUserDataCategory*	FindCategory (LPCSTR categoryName);
	
	// Find a specific field within this list given its category and field names
	CUserDataField*		FindField (LPCSTR categoryName ,LPCSTR fieldName);
	
	// Return count 
	int		GetCount()
	{ return _listCategories.GetCount(); }
	
	// Add a new field
	int		Add(CUserDataCategory& category)
	{	return _listCategories.Add(category); }
	
private:
	// This is the internal list of UserDataCategories
	CDynaList<CUserDataCategory>	_listCategories;
};