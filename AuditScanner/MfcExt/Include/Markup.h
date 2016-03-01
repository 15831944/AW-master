// Markup.h: interface for the CMarkup class.
//
// CMarkup Release 6.5 Lite
// Copyright (C) 1999-2003 First Objective Software, Inc. All rights reserved
// This entire notice must be retained in this source code
// Redistributing this source code requires written permission
// This software is provided "as is", with no warranty.
// Latest fixes enhancements and documentation at www.firstobject.com
#pragma once
#include <afxtempl.h>

#ifdef _DEBUG
#define _DS(i) (i?&((LPCTSTR)m_csDoc)[m_aPos[i].nStartL]:0)
#define MARKUP_SETDEBUGSTATE m_pMainDS=_DS(m_iPos); m_pChildDS=_DS(m_iPosChild)
#else
#define MARKUP_SETDEBUGSTATE
#endif

class CMarkup  
{
public:
	CMarkup() { SetDoc( NULL ); };
	CMarkup( LPCTSTR szDoc ) { SetDoc( szDoc ); };
	CMarkup( const CMarkup& markup ) { *this = markup; };
	void operator=( const CMarkup& markup );
	virtual ~CMarkup() {};

	// Load, Save
	bool Load(LPCTSTR szDoc, CString& szContents);
	bool Save(LPCTSTR szDoc, CString szContents);

	// Create
	CString GetDoc() const { return m_csDoc; };
	//
	bool AddElem( LPCTSTR szName, LPCTSTR szData=NULL ) 
	{ return x_AddElem(szName,szData,false,false); };
	bool AddBoolElem( LPCTSTR szName, BOOL bStatus ) 
	{ if (bStatus) return x_AddElem(szName,"Yes" ,false,false); else return x_AddElem(szName,"No" ,false,false);};
	bool AddNumericElem( LPCTSTR szName, int value ) 
	{ char cValue[8]; _itoa(value ,cValue ,10); return x_AddElem(szName ,cValue ,false ,false);};
	//
	bool AddChildElem		(LPCTSTR szName, LPCTSTR szData=NULL )	
	{ return x_AddElem(szName,szData,false,true); };
	bool AddChildBoolElem	(LPCTSTR szName, BOOL bStatus )			
	{ if (bStatus) return x_AddElem(szName,"Yes" ,false,true); else return x_AddElem(szName,"No" ,false,true);};
	bool AddChildNumericElem(LPCTSTR szName, int value )			
	{ char cValue[8]; _itoa(value ,cValue ,10); return x_AddElem(szName ,cValue ,false ,true);};
	//
	bool AddAttrib( LPCTSTR szAttrib, LPCTSTR szValue ) 
	{ return x_SetAttrib(m_iPos,szAttrib,szValue); };
	bool AddChildAttrib( LPCTSTR szAttrib, LPCTSTR szValue ) 
	{ return x_SetAttrib(m_iPosChild,szAttrib,szValue); };
	bool SetAttrib( LPCTSTR szAttrib, LPCTSTR szValue ) 
	{ return x_SetAttrib(m_iPos,szAttrib,szValue); };
	bool SetChildAttrib( LPCTSTR szAttrib, LPCTSTR szValue ) 
	{ return x_SetAttrib(m_iPosChild,szAttrib,szValue); };

	// Navigate
	bool SetDoc( LPCTSTR szDoc );
	bool IsWellFormed();
	bool FindElem( LPCTSTR szName=NULL );
	bool FindChildElem( LPCTSTR szName=NULL );
	bool IntoElem();
	bool OutOfElem();
	void ResetChildPos() { x_SetPos(m_iPosParent,m_iPos,0); };
	void ResetMainPos() { x_SetPos(m_iPosParent,0,0); };
	void ResetPos() { x_SetPos(0,0,0); };
	CString GetTagName() const;
	CString GetChildTagName() const { return x_GetTagName(m_iPosChild); };
	CString GetData() const { return x_GetData(m_iPos); };
	bool	GetDataAsBoolean() const { return (x_GetData(m_iPos) == "true") ? true : false; };
	int		GetDataAsInt() const { return (atoi(x_GetData(m_iPos))); };
	CString GetChildData() const { return x_GetData(m_iPosChild); };
	bool	GetChildDataAsBoolean() const { return (x_GetData(m_iPosChild) == "true") ? true : false; };
	int		GetChildDataAsInt() const { return (atoi(x_GetData(m_iPosChild))); };
	CString GetAttrib( LPCTSTR szAttrib ) const { return x_GetAttrib(m_iPos,szAttrib); };
	CString GetChildAttrib( LPCTSTR szAttrib ) const { return x_GetAttrib(m_iPosChild,szAttrib); };
	CString GetError() const { return m_csError; };
	
	bool	GetChildAttribAsBoolean(LPCTSTR szAttrib ,bool bDefault) const 
	{ 
		CString value = x_GetAttrib(m_iPosChild,szAttrib);
		if (value == "")
			return bDefault;
		else
			return (value == "true") ? true : false; 
	};
	
	int		GetChildAttribAsInt(LPCTSTR szAttrib ,int nDefault) const 
	{ 
		CString value = x_GetAttrib(m_iPosChild,szAttrib);
		if (value == "")
			return nDefault;
		else
			return (atoi(x_GetAttrib(m_iPosChild,szAttrib))); 
	};

	enum MarkupNodeType
	{
		MNT_ELEMENT					= 1,  // 0x01
		MNT_TEXT					= 2,  // 0x02
		MNT_WHITESPACE				= 4,  // 0x04
		MNT_CDATA_SECTION			= 8,  // 0x08
		MNT_PROCESSING_INSTRUCTION	= 16, // 0x10
		MNT_COMMENT					= 32, // 0x20
		MNT_DOCUMENT_TYPE			= 64, // 0x40
		MNT_EXCLUDE_WHITESPACE		= 123,// 0x7b
	};

protected:

#ifdef _DEBUG
	LPCTSTR m_pMainDS;
	LPCTSTR m_pChildDS;
#endif

	CString m_csDoc;
	CString m_csError;

	struct ElemPos
	{
		ElemPos() { Clear(); };
		ElemPos( const ElemPos& pos ) { *this = pos; };
		bool IsEmptyElement() const { return (nStartR == nEndL + 1); };
		void Clear()
		{
			nStartL=0; nStartR=0; nEndL=0; nEndR=0; nReserved=0;
			iElemParent=0; iElemChild=0; iElemNext=0;
		};
		void AdjustStart( int n ) { nStartL+=n; nStartR+=n; };
		void AdjustEnd( int n ) { nEndL+=n; nEndR+=n; };
		int nStartL;
		int nStartR;
		int nEndL;
		int nEndR;
		int nReserved;
		int iElemParent;
		int iElemChild;
		int iElemNext;
	};

	CArray< ElemPos, ElemPos& > m_aPos;
	int m_iPosParent;
	int m_iPos;
	int m_iPosChild;
	int m_iPosFree;
	int m_nNodeType;

	struct TokenPos
	{
		TokenPos( LPCTSTR sz ) { Clear(); szDoc = sz; };
		bool IsValid() const { return (nL <= nR); };
		void Clear() { nL=0; nR=-1; nNext=0; bIsString=false; };
		bool Match( LPCTSTR szName )
		{
			int nLen = nR - nL + 1;
			return ( (_tcsnccmp( &szDoc[nL], szName, nLen ) == 0)
				&& ( szName[nLen] == _T('\0') || _tcschr(_T(" =/["),szName[nLen]) ) );
		};
		int nL;
		int nR;
		int nNext;
		LPCTSTR szDoc;
		bool bIsString;
	};

	void x_SetPos( int iPosParent, int iPos, int iPosChild )
	{
		m_iPosParent = iPosParent;
		m_iPos = iPos;
		m_iPosChild = iPosChild;
		m_nNodeType = iPos?MNT_ELEMENT:0;
		MARKUP_SETDEBUGSTATE;
	};

	int x_GetFreePos();
	int x_ReleasePos();
	int x_ParseElem( int iPos );
	int x_ParseError( LPCTSTR szError, LPCTSTR szName = NULL );
	static bool x_FindChar( LPCTSTR szDoc, int& nChar, _TCHAR c );
	static bool x_FindAny( LPCTSTR szDoc, int& nChar );
	static bool x_FindToken( TokenPos& token );
	CString x_GetToken( const TokenPos& token ) const;
	int x_FindElem( int iPosParent, int iPos, LPCTSTR szPath );
	CString x_GetTagName( int iPos ) const;
	CString x_GetData( int iPos ) const;
	CString x_GetAttrib( int iPos, LPCTSTR szAttrib ) const;
	bool x_AddElem( LPCTSTR szName, LPCTSTR szValue, bool bInsert, bool bAddChild );
	bool x_FindAttrib( TokenPos& token, LPCTSTR szAttrib=NULL ) const;
	bool x_SetAttrib( int iPos, LPCTSTR szAttrib, LPCTSTR szValue );
	void x_LocateNew( int iPosParent, int& iPosRel, int& nOffset, int nLength, int nFlags );
	int x_ParseNode( TokenPos& token );
	void x_DocChange( int nLeft, int nReplace, const CString& csInsert );
	void x_Adjust( int iPos, int nShift, bool bAfterPos = false );
	CString x_TextToDoc( LPCTSTR szText, bool bAttrib = false ) const;
	CString x_TextFromDoc( int nLeft, int nRight ) const;
};
