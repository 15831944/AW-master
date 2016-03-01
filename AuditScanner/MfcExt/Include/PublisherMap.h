// PublisherMap.h: interface for the CPublisherMap class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_PUBLISHERMAP_H__FEA27D0E_97EC_4EFB_8923_80821500336A__INCLUDED_)
#define AFX_PUBLISHERMAP_H__FEA27D0E_97EC_4EFB_8923_80821500336A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CPublisherMap  
{
public:
	CPublisherMap();
	virtual ~CPublisherMap();

	static BOOL Rationalize (CString& strPublisher);

};

#endif // !defined(AFX_PUBLISHERMAP_H__FEA27D0E_97EC_4EFB_8923_80821500336A__INCLUDED_)
