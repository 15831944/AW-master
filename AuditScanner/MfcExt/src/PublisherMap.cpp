// PublisherMap.cpp: implementation of the CPublisherMap class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

// CString array for converting Publisher Names
static CString arrayPublishers[30][2] = {"3COM"			,"3Com Corporation, Inc." 
								,"ADOBE"				,"Adobe Systems, Inc."
								,"APPLE"				,"Apple Computer, Inc."
								,"ATI"					,"ATI Technologies, Inc."
								,"BELKIN"				,"Belkin, Inc."
								,"CANON"				,"Canon Inc."
								,"CISCO"				,"Cisco Systems, Inc."
								,"CITRIX"				,"Citrix Systems, Inc."
								,"COMPUTER ASSOCIATES"	,"Computer Associates International, Inc."
								,"COREL"				,"Corel Corporation, Inc."
								,"DELL"					,"Dell Computer Corporation, Inc."
								,"HEWLETT"				,"Hewlett-Packard Company"
								,"IBM"					,"IBM Corporation, Inc."
								,"INTEL"				,"Intel Corporation"
								,"INSTALLSHIELD"		,"Installshield Corporation, Inc."
								,"IOMEGA"				,"Iomega Corporation, Inc."
								,"JASC"					,"Jasc Software, Inc."
								,"LAYTON"				,"Layton Technology, Inc."
								,"LOTUS DEVELOPMENT"	,"Lotus Development, Inc."
								,"MACROVISION"			,"Macrovision Corporation, Inc."
								,"MACROMEDIA"			,"Macromedia Corporation, Inc."
								,"MICROSOFT"			,"Microsoft Corporation, Inc."
								,"ROXIO"				,"Roxio, Inc."
								,"SYMANTEC"				,"Symantec Corporation, Inc."
								,"ULEAD"				,"ULead Systems, Inc."
								,"VERITAS"				,"Veritas Software, Inc."
								,"WINZIP"				,"WinZip Computing, Inc."
								};
static int sPublisherCount = 27;

CPublisherMap::CPublisherMap()
{
}

CPublisherMap::~CPublisherMap()
{
}


//
//    Rationalize
//    ===========
//
//    Rationalize the publisher by finding if it matches any of the entries in our list and
//    if so returning the rationalized publisher
//
BOOL CPublisherMap::Rationalize (CString& strPublisher)
{
	// Upper case it first as we ignore case
	CString strUPCPublisher = strPublisher;
	strUPCPublisher.MakeUpper();

	for (int isub=0; isub<sPublisherCount; isub++)
	{
		int nLen = arrayPublishers[isub][0].GetLength();
		if (strUPCPublisher.Left(nLen) == arrayPublishers[isub][0])
		{
			strPublisher = arrayPublishers[isub][1];
			return TRUE;
		}
	}

	return FALSE;
}

