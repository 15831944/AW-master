#pragma once

//
//    CPicklist
//    =========
//
//    Encapsulate a picklist
//
class CPicklist : public CDynaList<CString>
{
public:
	CPicklist()
	{}

// Data Accessor Functions
public: 
	CString&	Name (void)
	{ return _name; }
	void Name (CString& value)
	{ _name = value; }
	
protected:
	CString		_name;			// field label
};
