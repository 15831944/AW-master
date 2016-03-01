#pragma once

class CWindowsSocket  
{
public: // Methods
	// contructor/destructor
	CWindowsSocket();
	virtual ~CWindowsSocket();

	CString&	GetIPAddress();

protected: // Attributes
	CString		m_strIPAddress;
};

