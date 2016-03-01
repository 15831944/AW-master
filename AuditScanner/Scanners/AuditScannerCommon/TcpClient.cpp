// TcpClient.cpp: implementation of the CTcpClient class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TcpClient.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#define SEND_BUFFER_SIZE    4096

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTcpClient::CTcpClient()
{
	m_hMainThread = NULL;
   m_winSock = NULL;
   m_bConnected = false;
   m_bConnecting = false;
}

CTcpClient::~CTcpClient()
{
	if (m_winSock != NULL)
   {
      closesocket(m_winSock);
      WSACleanup();
   }

	DWORD dwTemp;
	if (GetExitCodeThread(m_hMainThread,&dwTemp))
	{
		if (dwTemp == STILL_ACTIVE)
			TerminateThread(m_hMainThread,3);
	}
	if (m_hMainThread != NULL && m_hMainThread != INVALID_HANDLE_VALUE)
		CloseHandle(m_hMainThread);
}


bool CTcpClient::Connect(CString strServer, WORD wPort, void (*MsgCallBack)(CString buf, LPARAM lParam), LPARAM lParam)
{
   m_bConnected = false;
	m_strServer = strServer;
	m_wPort = wPort;
	m_lParam = lParam;
	m_MsgCallBack = MsgCallBack;

	if (MsgCallBack == NULL || lParam == NULL)
		return false;

	// Start the main thread
	m_hMainThread = CreateThread(
		NULL,
		0,
		(LPTHREAD_START_ROUTINE)ConnectThread,
		(LPVOID)this,
		0,
		NULL);

	if (m_hMainThread == NULL || m_hMainThread == INVALID_HANDLE_VALUE)
	{
      m_bConnecting = false;
		return false;
	}
	return true;
}

DWORD CTcpClient::ConnectThread(LPVOID lpParameter)
{
   CTcpClient* pTcp = (CTcpClient*)lpParameter;
	if (pTcp == NULL)
		return false;
   pTcp->m_bConnecting = true;
   
   // Initialize WinSock 2
	WSADATA wsaData;
   OutputDebugString("Initializing WinSock...");
	int wsaret = WSAStartup(0x101, &wsaData);
	if (wsaret)
   {
      pTcp->m_bConnecting = false;
      pTcp->m_MsgCallBack("<LOG>Unable to connect to TCP server: Unable to initialize Winsock.\n</LOG>", pTcp->m_lParam);
		return false;
   }

   // Create the TCP socket as a stream
	SOCKET winSock;
	winSock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (winSock == INVALID_SOCKET)
   {
      pTcp->m_bConnecting = false;
      pTcp->m_MsgCallBack("<LOG>Unable to connect to TCP server: Unable to create socket.\n</LOG>", pTcp->m_lParam);
		return false;
   }
   OutputDebugString("Created socket...");

   // Resolve the server host address
	hostent *hp;
	unsigned long addr;
   if (inet_addr(pTcp->m_strServer) == INADDR_NONE)
	{
		hp = gethostbyname(pTcp->m_strServer);
	}
	else
	{
		addr = inet_addr(pTcp->m_strServer);
		hp = gethostbyaddr((char*)&addr, sizeof(addr), AF_INET);
	}
	if (hp == NULL)
   {
      pTcp->m_bConnecting = false;
      pTcp->m_MsgCallBack("<LOG>Unable to connect to TCP server: Unable to resolve host.\n</LOG>", pTcp->m_lParam);
		return false;
   }
   char msgBuff[512];

   // Create server's socket struct
	struct sockaddr_in server;
   server.sin_addr.s_addr=*((unsigned long*)hp->h_addr);
	server.sin_family=AF_INET;
	server.sin_port=htons(pTcp->m_wPort);

   // Connect to the TCP server
	if (connect(winSock, (struct sockaddr*)&server, sizeof(server)) == SOCKET_ERROR)
	{
      int nError = WSAGetLastError();
      if (nError == WSAECONNREFUSED)
      {
         sprintf(msgBuff, "<LOG>Failed to connect to TCP server: Connection refused due to no server running.\n</LOG>");
      }
      else if (nError == WSAETIMEDOUT)
      {
         sprintf(msgBuff, "<LOG>Failed to connect to TCP server: Timeout trying to connect to server.\n</LOG>");
      }
      else
      {
         sprintf(msgBuff, "<LOG>Failed to connect to TCP server: Error %d\n</LOG>", nError);
      }
      pTcp->m_bConnecting = false;
		closesocket(winSock);
      pTcp->m_MsgCallBack(msgBuff, pTcp->m_lParam);
		return false;
	}
   // Successfully connected
   pTcp->m_winSock = winSock;
   pTcp->m_bConnected = true;
   pTcp->m_bConnecting = false;
   sprintf(msgBuff, "<CONNECTED>Connected to TCP server on %s:%d.\n</CONNECTED>", pTcp->m_strServer, pTcp->m_wPort);
   pTcp->m_MsgCallBack(msgBuff, pTcp->m_lParam);

   int nBytes;
   CString strMessage = "";

   do 
   {
      // Receive until the server closes the connection or there is an error
      nBytes = 0;
      char buff[SOCKET_BUFF_SIZE] = { 0 };

      nBytes = recv(pTcp->m_winSock, buff, SOCKET_BUFF_SIZE, 0);
      if (nBytes > 0)
      {
         // Got a packet...check if it is a full message
         if (buff[nBytes - 1] == '\0')
         {
            // Got a full message
            strMessage += buff;
	         pTcp->m_MsgCallBack(strMessage, pTcp->m_lParam);
            OutputDebugString("Got a message from recv...");
            strMessage = "";
         }
         else
         {
            // got a partial message
            OutputDebugString("Got a partial message from recv...");
            strMessage += buff;
         }
      }
      else if (nBytes == 0)
      {
         // close the connection
         pTcp->m_MsgCallBack("<LOG>Server has closed the connection.\n</LOG>", pTcp->m_lParam);

      }
      else
      {
         sprintf(msgBuff, "<LOG>TCP receive error: %d\n</LOG>", WSAGetLastError());
      }
   } 
   while (nBytes > 0);

   // cleanup
   closesocket(pTcp->m_winSock);
   WSACleanup();
   pTcp->m_winSock = NULL;
   pTcp->m_bConnected = false;
   OutputDebugString("Client has disconnected...");

   return 1;
}

bool CTcpClient::Send(CString strMessage)
{
   if (m_winSock == NULL || m_bConnected == false)
   {
      m_bConnected = false;
      m_MsgCallBack("<LOG>Unable to send when a connection has not been established.\n</LOG>", m_lParam);
      return false;
   }

   char buff[SOCKET_BUFF_SIZE];
   sprintf(buff, "%s\r\n\r\n", strMessage);
   OutputDebugString(buff);
   int nResult = send(m_winSock, buff, strlen(buff), 0);
   if (nResult == SOCKET_ERROR)
   { 
      char debugBuff[512];
      sprintf(debugBuff, "<LOG>Failed to send message to TCP server: Error %d\n</LOG>", WSAGetLastError());
      m_MsgCallBack(debugBuff, m_lParam);
      return false;
   }
   OutputDebugString("Send succeeded...");
   return true;
}



//
// Send a file via the TCP connection
//
BOOL CTcpClient::SendFile(CString& fileName)
{
	CFile sourceFile;
	CFileException fe;
	BOOL bFileIsOpen = FALSE;
    BOOL bRet = TRUE;
    CString strMessage;
    
	if (!(bFileIsOpen = sourceFile.Open(fileName, CFile::modeRead | CFile::typeBinary, &fe)))
    {
        TCHAR strCause[256];
        fe.GetErrorMessage( strCause, 255 );
		strMessage.Format("<LOG>Failed to write the audit results to the TCP Server\n\tFile name = %s\n\tCause = %s\n\tm_cause = %d\n\tm_IOsError = %d\n</LOG>",
						  fe.m_strFileName, strCause, fe.m_cause, fe.m_lOsError);
		m_MsgCallBack(strMessage, m_lParam);
		bRet = FALSE;
		goto PreReturnCleanup;
    }

    // first send length of file
	int fileLength = (int)sourceFile.GetLength();
	fileLength = htonl(fileLength);

    // pointer to buffer for sending data
    // (memory is allocated after sending file size)
    char* sendData = NULL;

	// First we need to send the size of the file to the server
    fileLength = (int)sourceFile.GetLength();
    fileLength = htonl(fileLength);

	// How much is left to send?
    int nBytesLeftToSend = sizeof(fileLength);
        
    // now send the file's data - there is a maximuym sebnd buffer size so we probably won't be able to send
    // the entire file in one chunk 
    sendData = new char[SEND_BUFFER_SIZE]; 
    nBytesLeftToSend = (int)sourceFile.GetLength();
    
	int sendThisTime, doneSoFar, buffOffset;        
    do
    {
		// read next chunk of SEND_BUFFER_SIZE bytes from the input file
		sendThisTime = sourceFile.Read(sendData, SEND_BUFFER_SIZE - 4);
        buffOffset = 0;
        
        // ...and send this via the socket noting again that we may not be able to send it all in one chunk
        do
        {
            doneSoFar = send(m_winSock
						   , sendData + buffOffset					// Buffer being sent
						   , sendThisTime							// Size of data to send
						   ,0);										// flags
            
			// test for errors and get out if they occurred
			if (doneSoFar == SOCKET_ERROR)
			{
				int iErr = ::GetLastError();
				strMessage.Format("Failed sending the file chunk to the server, the error was %d\n" ,iErr);
				m_MsgCallBack(strMessage, m_lParam);
				bRet = FALSE;
			}
            
            // data was successfully sent, so account for it with already-sent data
			buffOffset += doneSoFar;
			sendThisTime -= doneSoFar;
			nBytesLeftToSend -= doneSoFar;
		}
		while (sendThisTime > 0); 
	}
	while (nBytesLeftToSend > 0);
	
	// OK - we have now sent the contents of the ADF file however we need to add the terminator to the message
	// to inform the server that the end of message has been reached.
	strcpy(sendData ,"***EOF#123***");
	nBytesLeftToSend = strlen(sendData);
	//
    buffOffset = 0;

	// ...and send this via the socket noting again that we may not be able to send it all in one chunk
    do
    {
		doneSoFar = send(m_winSock
					   , sendData + buffOffset					// Buffer being sent
					   , nBytesLeftToSend						// Size of data to send
					   ,0);										// flags
            
		// test for errors and get out if they occurred
		if (doneSoFar == SOCKET_ERROR)
		{
			int iErr = ::GetLastError();
			strMessage.Format("Failed sending the terminating file chunk to the server, the error was %d\n" ,iErr);
			m_MsgCallBack(strMessage, m_lParam);
			bRet = FALSE;
		}
            
		// data was successfully sent, so account for it with already-sent data
		buffOffset += doneSoFar;
		sendThisTime -= doneSoFar;
		nBytesLeftToSend -= doneSoFar;
	}
	while (nBytesLeftToSend > 0); 
	
    
// Bad form maybe but having this label allows us to handle errors in one place tidying up resources    
PreReturnCleanup: 
	// Delete allocated buffer
    delete[] sendData;
    
    // If the source file is open then close it
    if (bFileIsOpen)
        sourceFile.Close();

    return bRet;
} 