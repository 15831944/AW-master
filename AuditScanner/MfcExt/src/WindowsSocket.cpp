#include "stdafx.h"
#include <Winsock.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

// Unknown value
#define UNKNOWN "N/A"

// Winsock functions declaration for getting local IP Address
int (__stdcall *lpfnWSAStartup) (WORD wVersionRequested, WSADATA *wsaData);
int (__stdcall *lpfnGetHostName) (LPSTR lpstrName, int nMaxSize);
struct hostent * (__stdcall *lpfnGetHostByName) (LPCTSTR lpstrName);
LPCTSTR (__stdcall *lpfnInet_ntoa) (struct in_addr pInetAddr);
int (__stdcall *lpfnWSACleanup) (void);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CWindowsSocket::CWindowsSocket()
{
	m_strIPAddress = UNKNOWN;
}

CWindowsSocket::~CWindowsSocket()
{
}

CString& CWindowsSocket::GetIPAddress()
{
	struct hostent	*pHostEnt;
	struct in_addr	pInetAddr;
	WORD			wVersionRequested;
	WSADATA			wsaData;
	HINSTANCE	    hDll;
	TCHAR			szHostName[256];

	// Load the Winsock 32 bit DLL
	if ((hDll = LoadLibrary( _T( "wsock32.dll"))) == NULL)
		return m_strIPAddress;

	// Load the WSAStartup function
	if ((*(FARPROC*)&lpfnWSAStartup = GetProcAddress( hDll, _T( "WSAStartup"))) == NULL)
	{
		FreeLibrary( hDll);
		return m_strIPAddress;
	}

	// Load the gethostname function
	if ((*(FARPROC*)&lpfnGetHostName = GetProcAddress( hDll, _T( "gethostname"))) == NULL)
	{
		FreeLibrary( hDll);
		return m_strIPAddress;
	}

	// Load the gethostbyname function
	if ((*(FARPROC*)&lpfnGetHostByName = GetProcAddress( hDll, _T( "gethostbyname"))) == NULL)
	{
		FreeLibrary( hDll);
		return m_strIPAddress;
	}

	// Load the inet_ntoa function
	if ((*(FARPROC*)&lpfnInet_ntoa = GetProcAddress( hDll, _T( "inet_ntoa"))) == NULL)
	{
		FreeLibrary( hDll);
		return m_strIPAddress;
	}

	// Load the WSACleanup function
	if ((*(FARPROC*)&lpfnWSACleanup = GetProcAddress( hDll, _T( "WSACleanup"))) == NULL)
	{
		FreeLibrary( hDll);
		return m_strIPAddress;
	}

	wVersionRequested = MAKEWORD( 1, 1 );
 	if (lpfnWSAStartup( wVersionRequested, &wsaData ) != 0)
	{
		FreeLibrary( hDll);
		return m_strIPAddress;
	}

	// Confirm that the WinSock DLL supports 1.1.
	// Note that if the DLL supports versions greater than 1.1 in addition to 1.1, it will still return 
	// 1.1 in wVersion since that is the version we requested.                                        
	if (LOBYTE(wsaData.wVersion) != 1 || HIBYTE( wsaData.wVersion) != 1) 
	{
		lpfnWSACleanup();
		FreeLibrary( hDll);
		return m_strIPAddress; 
	}

	if (lpfnGetHostName(szHostName, 256) == SOCKET_ERROR)
	{
		lpfnWSACleanup();
		FreeLibrary( hDll);
		return m_strIPAddress; 
	}

	if ((pHostEnt = lpfnGetHostByName( szHostName)) == NULL)
	{
		lpfnWSACleanup( );
		FreeLibrary( hDll);
		return m_strIPAddress; 
	}

	if (pHostEnt->h_length != 4)
	{
		// Not 32 bits IP adresses
		lpfnWSACleanup( );
		FreeLibrary( hDll);
		return m_strIPAddress; 
	}
	else
	{
		// Computer as an IP Address => get the first one
		memcpy(&pInetAddr, pHostEnt->h_addr_list[0], pHostEnt->h_length);
		m_strIPAddress = lpfnInet_ntoa(pInetAddr);
	}

	// Unload the Winsock
	lpfnWSACleanup( );
	
	// Unload the Winsock Dll
	FreeLibrary( hDll);
	return m_strIPAddress;
}