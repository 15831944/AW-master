#pragma once


#include <winsvc.h>
#include "Tlhelp32.h"
#include <Winbase.h>

//---------------------------------------------------------------------------
//
//                   typedefs for PSAPI.DLL functions 
//
//---------------------------------------------------------------------------
typedef BOOL (WINAPI * PFNENUMPROCESSES)(
    DWORD * lpidProcess,
    DWORD   cb,
    DWORD * cbNeeded
	);

typedef BOOL (WINAPI * PFNENUMPROCESSMODULES)(
    HANDLE hProcess,
    HMODULE *lphModule,
    DWORD cb,
    LPDWORD lpcbNeeded
	);

typedef DWORD (WINAPI * PFNGETMODULEFILENAMEEXA)(
    HANDLE hProcess,
    HMODULE hModule,
    LPSTR lpFilename,
    DWORD nSize
	);

// Function pointer for dynamic linking to Kernal32
typedef HANDLE (WINAPI* pCreateToolhelp32Snapshot)	(DWORD dwFlags, DWORD th32ProcessID);
typedef BOOL   (WINAPI* pProcess32First)			(HANDLE hSnapshot, LPPROCESSENTRY32 lppe);
typedef BOOL   (WINAPI* pProcess32Next)				(HANDLE hSnapshot, LPPROCESSENTRY32 lppe);
typedef BOOL   (WINAPI* pModule32First)				(HANDLE hSnapshot, LPMODULEENTRY32 lpme);


////////////////////////////////////////////////////////////////////////////
//
//    This class encapsulates a single instance of an active process
//
class CActiveProcess
{
public:
	CActiveProcess(void)
	{
		_name = UNKNOWN;
		_pid = 0;
	}
	
	CActiveProcess(LPCSTR executable ,LPCSTR name ,DWORD pid)
	{
		_executable = executable;
		_name = name;
		_pid = pid;
	}

public:
// Data Accessors
	CString	Executable (void)
	{ return _executable; }
	void	Executable (LPCSTR value)
	{ _executable = value; }
	//
	CString		Name (void)
	{ return _name; }
	void	Name (LPCSTR value)
	{ _name = value; }
	//
	DWORD	Pid (void)
	{ return _pid; }
	void	Pid (DWORD value)
	{ _pid = value; }

// Internal data
private:
	CString		_executable;
	CString		_name;
	DWORD		_pid;
};



class CActiveProcessScanner : public CAuditDataScanner
{
public:
	CActiveProcessScanner(void);
public:
	virtual ~CActiveProcessScanner(void);

// Base class over-rides
public:
	virtual bool	ScanWMI			(CWMIScanner* pScanner);
	virtual bool	ScanRegistryXP	(void);
	virtual bool	ScanRegistryNT	(void);
	virtual bool	ScanRegistry9X	(void);
	virtual bool	SaveData		(CAuditDataFile* pAuditDataFile);

protected:
	BOOL	ScanActiveProcesses		();
	CString GetProcessExecutable	(DWORD dwPID);

private:
	CDynaList<CActiveProcess>	_listActiveProcesses;

private:
	HINSTANCE	m_hKernel32;
	pCreateToolhelp32Snapshot m_pFnCreateToolHelpSnapshot;
	pProcess32First			m_pFnProcess32First;
	pProcess32Next			m_pFnProcess32Next;
	pModule32First			m_pFnModule32First;

};