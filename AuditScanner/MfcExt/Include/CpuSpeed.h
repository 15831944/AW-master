
// FILE:	CpuSpeed.h
// PURPOSE:	Class for detection of CPU speed
// AUTHOR:	JRF Thornley - copyright (C) InControl Software 2001
// HISTORY:	JRFT - 11.07.2001 - reworked from original MthBoard.cpp/h code (originally developed 1998)
// NOTES:	Include this file in stdafx.h

#ifndef _CPUSPEED_DEF_
#define _CPUSPEED_DEF_

class CCpuSpeed
{
public:
	// constructor
	CCpuSpeed ();
	// destructor
	~CCpuSpeed ();
	// return result in MHz normalised to nearest known frequency
	int GetMHz();
	// Get actual result in KHz
	DWORD GetTrueSpeed ();
	// set diagnostic mode (sends tracing to pLog)
	void EnableDiagnostics (CLogFile * pLog);
	// round value to nearest known speed
	int NormaliseSpeed (int nOriginal);
protected:
	// the main detection routine
	void Detect ();
	// detects using RDTSC instruction to count clock ticks (preferred)
	DWORD DetectByRDTSC ();
	// performs the actual RDTSC loop
	DWORD TestRDTSC (DWORD dwTestDuration);
	// returns elapsed CPU cycles since startup
	LARGE_INTEGER GetTickCount();
	// detects using a tight-timed loop if RDTSC not available
	DWORD DetectByTimedLoop ();
	// run the actual timed loop test
	WORD TestLoop ();
protected:
	static DWORD	m_dwSpeed;		// exact calculated speed in KHz
	CLogFile *		m_pLogFile;		// logfile for diagnostic mode
};

#endif //#define _CPUSPEED_DEF_