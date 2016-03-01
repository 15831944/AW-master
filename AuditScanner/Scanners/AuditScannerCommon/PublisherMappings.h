#pragma once

class CPublisherMapping
{
public:
	CPublisherMapping ()
	{}

	CPublisherMapping (CString& stub, CString& alias)
	{ _stub = stub; _alias = alias; }

public:
	CString	Stub (void)
	{ return _stub; }
	void	Stub	(CString& value)
	{ _stub = value; }
	//
	CString	Alias (void)
	{ return _alias; }
	void	Alias	(CString& value)
	{ _alias = value; }

private:
	CString _stub;
	CString _alias;
};


class CPublisherMappings : public CDynaList<CPublisherMapping>
{
public:
	CPublisherMappings(void);
	~CPublisherMappings(void);

	CString RationalizePublisherName (CString& name);

};
