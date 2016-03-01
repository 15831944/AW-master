// File:      NamedPipe.h: Interface for the CNamedPipe class.
// Author:    Andrew Cooper
// Created:   June 12, 2008
// Copyright: Layton Technology, Inc 2008

#pragma once

#define PIPE_BUF_SIZE 4096 // 4 KB
#define PIPE_TIMEOUT  (120*1000) /*120 seconds*/

class CNamedPipe  
{
public:
	CNamedPipe();
	virtual ~CNamedPipe();

	//bool Initialize(bool bAsServer, CString szStopListnenCmd, void (*MsgCallBack)(CString buf, LPARAM lParam), LPARAM lParam);
	bool CreateServer(CString strPipeName, void (*MsgCallBack)(CString buf, LPARAM lParam), LPARAM lParam);

	static DWORD ListnerProc(LPVOID lpParameter);
	struct ListnerParam
	{
		HANDLE hPipe;                            // Handle to pipe to listen to...
		LPARAM lParam;
		void (*MsgCallBack)(CString buf, LPARAM lParam); // Call this function for every successful read operation.
	};
protected:
	CString m_strServerPipeName;
	HANDLE m_hServerPipe;
	HANDLE m_hListner;
	DWORD m_dwListnerThreadId;

public:
	CString GetServerPipeName() { return m_strServerPipeName; }
	bool SendPipeMessage(CString strServer, CString strPipeName, CString strMessage);
};