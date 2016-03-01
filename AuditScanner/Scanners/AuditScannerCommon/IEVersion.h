// IEVersion.h: interface for the CIEVersion class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

class CIEVersion  
{
public:
	CIEVersion();
	virtual ~CIEVersion();
	
	CString		m_strVersion;
	void	GetVersion	(void);

};