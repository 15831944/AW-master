
// FILE:	NamedPipes.cpp
// PURPOSE:	Implementation for the Named Pipes communation class, CNamedPipes
// AUTHOR:	Andrew Cooper - copyright (C) Layton Technology, Inc. 2008
// HISTORY:	ABC - June 12, 2008 - written

#include "stdafx.h"
#include "NamedPipe.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CNamedPipe::CNamedPipe()
{
	HANDLE m_hServerPipe = NULL;
	HANDLE m_hListner = NULL;
}

CNamedPipe::~CNamedPipe()
{
	if (m_hServerPipe != NULL && m_hServerPipe != INVALID_HANDLE_VALUE)
		CloseHandle(m_hServerPipe);

	DWORD dwTemp;
	if (GetExitCodeThread(m_hListner,&dwTemp))
	{
		if (dwTemp == STILL_ACTIVE)
			TerminateThread(m_hListner,3);
	}
	if (m_hListner != NULL && m_hListner != INVALID_HANDLE_VALUE)
		CloseHandle(m_hListner);
}

// The main loop creates an instance of the named pipe and 
// then waits for a client to connect to it. When the client 
// connects, a thread is created to handle communications 
// with that client, and the loop is repeated. 
bool CNamedPipe::CreateServer(CString strPipeName, void (*MsgCallBack)(CString buf, LPARAM lParam), LPARAM lParam)
{
	m_strServerPipeName = "\\\\.\\PIPE\\";
	m_strServerPipeName += strPipeName;
	
	if (MsgCallBack == NULL)
		return false;

	m_hServerPipe = CreateNamedPipe( 
		m_strServerPipeName,      // pipe name 
		PIPE_ACCESS_DUPLEX,       // read/write access 
		PIPE_TYPE_MESSAGE |       // message type pipe 
		PIPE_READMODE_MESSAGE |   // message-read mode 
		PIPE_WAIT,                // blocking mode 
		PIPE_UNLIMITED_INSTANCES, // max. instances  
		PIPE_BUF_SIZE,            // output buffer size 
		PIPE_BUF_SIZE,            // input buffer size 
		PIPE_TIMEOUT,             // client time-out 
		NULL);                    // default security attribute 

	if (m_hServerPipe == NULL || m_hServerPipe == INVALID_HANDLE_VALUE) 
	{
		return false;
	}

	// Create a thread for this client. 
	ListnerParam* pLP = new ListnerParam;
	pLP->MsgCallBack = MsgCallBack;
	pLP->hPipe = m_hServerPipe;
	pLP->lParam = lParam;

	m_hListner = CreateThread(
		NULL,
		0,
		(LPTHREAD_START_ROUTINE)ListnerProc,
		(LPVOID)pLP,
		0,
		&m_dwListnerThreadId);

	if (m_hListner == NULL || m_hListner == INVALID_HANDLE_VALUE)
	{
		return false;
	}
	return true;
}

/*static*/
DWORD CNamedPipe::ListnerProc(LPVOID lpParameter)
{
	char buf[PIPE_BUF_SIZE];
	DWORD dwRetVal, dwRead;
	CString szMsg;
	BOOL bOK;
	ListnerParam* pLP = (ListnerParam*)lpParameter;
	if (pLP == NULL)
		return 1;

	dwRetVal = 0;
	while(dwRetVal == 0)
	{
		// Read from the pipe
		bOK = ReadFile(
			pLP->hPipe,     // pipe to read from
			buf,            // buffer
			PIPE_BUF_SIZE,  // buffer size 
			&dwRead,        // # of bytes read
			NULL);          // not overlapped I/O

		if (dwRead == 0)
			continue;
		if (!bOK)
			dwRetVal = 2;
		
		szMsg = buf;
		pLP->MsgCallBack(szMsg, pLP->lParam);
		//cbReplyBytes  = lstrlen(szMsg) + 1;

		//// Write the reply to the pipe. 
		//WriteFile( 
		//	pLP->hPipe,   // handle to pipe 
		//	szMsg,        // buffer to write from 
		//	cbReplyBytes, // number of bytes to write 
		//	&cbWritten,   // number of bytes written 
		//	NULL);        // not overlapped I/O 
	}

	// Clean up
	FlushFileBuffers(pLP->hPipe); 
	DisconnectNamedPipe(pLP->hPipe); 
	CloseHandle(pLP->hPipe); 
	delete pLP;

	return dwRetVal;
}

bool CNamedPipe::SendPipeMessage(CString strServer, CString strPipeName, CString strMessage)
{
	CLogFile log;
	HANDLE hPipe; 
	BOOL fSuccess; 
	DWORD cbWritten, dwMode; 
	CString strFullPipeName = "\\\\" + strServer + "\\pipe\\" + strPipeName; 
	log.Format("Pipe name is %s\n" ,strFullPipeName);

	// Create the connection to the remote pipe
	while (1) 
	{ 
		hPipe = CreateFile( strFullPipeName,	// pipe name 
							GENERIC_READ |		// read and write access 
							GENERIC_WRITE, 
							0,					// no sharing 
							NULL,				// default security attributes
							OPEN_EXISTING,		// opens existing pipe 
							0,					// default attributes 
							NULL);				// no template file 
 
		// Break if the pipe handle is valid. 
		if (hPipe != INVALID_HANDLE_VALUE)
			break; 
 
		// Exit if an error other than ERROR_PIPE_BUSY occurs. 
		if (GetLastError() != ERROR_PIPE_BUSY) 
		{
			log.Write("Failed to open the pipe");
			return false;
		}
 
		// All pipe instances are busy, so wait for 10 seconds for it to become free
		if (!WaitNamedPipe(strFullPipeName, 10000)) 
		{ 
			log.Write("Could not open pipe within the allowed time period");
			return false;
		} 
	}

	// The pipe connected; change to message-read mode. 
	dwMode = PIPE_READMODE_MESSAGE; 
	fSuccess = SetNamedPipeHandleState( hPipe,    // pipe handle 
										&dwMode,  // new pipe mode 
										NULL,     // don't set maximum bytes 
										NULL);    // don't set maximum time 
	if (!fSuccess) 
	{
		log.Write("Failed to change the pipe to message-read mode");
		return false;
	}
 
	// Send a message to the pipe server. 
	fSuccess = WriteFile( hPipe,                  // pipe handle 
						  strMessage,             // message 
						  lstrlen(strMessage) + 1,// message length 
						  &cbWritten,             // bytes written 
						  NULL);                  // not overlapped 
	if (!fSuccess) 
	{
		log.Write("Failed to write the specified message to the pipe");
		return false;
	}
 
	CloseHandle(hPipe); 
    return true; 
}
