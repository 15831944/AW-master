#include "stdafx.h"
#include "PublisherMappings.h"

CPublisherMappings::CPublisherMappings(void)
{
}

CPublisherMappings::~CPublisherMappings(void)
{
}

CString CPublisherMappings::RationalizePublisherName (CString& name)
{
	CString rationalizedName = name;

	// All base names are uppercase for ease of comparison so convert the supplied name
	CString upcPublisherName = name.MakeUpper();

	// The publisher mappings dictionary contains stubs which we need to match against the 
	// start of the passed in value.
	for (DWORD dw=0; dw<GetCount(); dw++)
	{
		CPublisherMapping* pMapping = &m_pData[dw];
		CString stub = pMapping->Stub();
		if (name.Left(stub.GetLength()) == stub)
		{
			rationalizedName = pMapping->Alias();
			break;
		}
	}
	return rationalizedName;
}
