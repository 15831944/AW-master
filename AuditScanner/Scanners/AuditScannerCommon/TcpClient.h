// TcpClient.h: interface for the CTcpClient class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#define SOCKET_PORT      31730
#define SOCKET_BUFF_SIZE 4096

#ifndef _NODLL
class AFX_EXT_CLASS CTcpClient
#else
class CTcpClient
#endif
{
public:
	CTcpClient();
	virtual ~CTcpClient();

	// Connect to the TCP Server
	bool Connect	(CString strServer, WORD wPort, void (*MsgCallBack)(CString buf, LPARAM lParam), LPARAM lParam);
	
	// Send a message to the TCP Server
	bool Send		(CString strMessage);
	
	// Send the contents of the specified file to the TCP Server
	BOOL SendFile	(CString& fileName);
	
   bool CTcpClient::IsConnected() { return m_bConnected; };   // Determines if there is an open connection
   bool CTcpClient::IsConnecting() { return m_bConnecting; }; // Determines if an attempt is being made to connect

private:
   WORD m_wPort;
   bool m_bConnected;
   bool m_bConnecting;
   CString m_strServer;
	HANDLE m_hMainThread;
	SOCKET m_winSock;
   LPARAM m_lParam;
   void (*m_MsgCallBack)(CString buf, LPARAM lParam); // call back function

   static DWORD ConnectThread(LPVOID lpParameter);
};