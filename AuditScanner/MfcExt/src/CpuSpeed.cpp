
// FILE:	CpuSpeed.cpp
// PURPOSE:	Class for detection of CPU speed
// AUTHOR:	JRF Thornley - copyright (C) InControl Software 2001
// HISTORY:	JRFT - 11.07.2001 - reworked from original MthBoard.cpp/h code (originally developed 1998)

#include "stdafx.h"

// Registry keys for obtaining cpu speed under Windows NT
#define RK_NT_CPU	"HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0"
#define RI_NT_MHZ	"~MHz"

/*
** Initialise static variable
*/
DWORD CCpuSpeed::m_dwSpeed = 0;

/*
** Constructor
*/
CCpuSpeed::CCpuSpeed()
{
	m_pLogFile = NULL;
}

/*
** Destructor
*/
CCpuSpeed::~CCpuSpeed()
{
}

/*
** Return the normalised result in MHz
*/
int CCpuSpeed::GetMHz()
{
	if (!m_dwSpeed)
		Detect();
	return NormaliseSpeed(m_dwSpeed / 1000);
}

/*
** Return the calculated result in KHz
*/
DWORD CCpuSpeed::GetTrueSpeed()
{
	if (!m_dwSpeed)
		Detect();
	return m_dwSpeed;
}

/*
** Enable diagnostic tracing
*/
void CCpuSpeed::EnableDiagnostics(CLogFile * pLog)
{
	m_pLogFile = pLog;
}

/*
** Run the detection process
*/
void CCpuSpeed::Detect()
{
	CLogFile log;
 	
	// What breed of operating system are we running ?
	COsInfo os;
	if (os.IsNT())
	{
		// Windows NT4 and 2000 store the speed in the registry
		m_dwSpeed = 1000 * CReg::GetItemInt(HKEY_LOCAL_MACHINE, RK_NT_CPU, RI_NT_MHZ);
	}
	
	else if (os.VersionMajor() >= 6 && os.VersionMinor() >= 2)
	{
		// We should not be getting the speed by dropping to asm for Winbdows 8 and above as this can cause a crash
		m_dwSpeed = CReg::GetItemInt(HKEY_LOCAL_MACHINE, RK_NT_CPU, RI_NT_MHZ);
	}

	else
	{
//		// other methods are processor critical, so grab some priority...
//		DWORD dwOriginalPriority = GetPriorityClass(GetCurrentProcess());
//		int nOriginalThreadPriority = GetThreadPriority(GetCurrentThread());
//		SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS);
//		SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST);

		// does the host cpu support the RDTSC instruction?
		try
		{
			// should jump to exception handler if not
			_asm
			{
				_emit	0x0F
				_emit	0x31
			}
			// RDTSC is supported so use the appropriate algorithm
			m_dwSpeed = DetectByRDTSC();
		}
		catch (...)
		{
			// RDTSC not supported
			m_dwSpeed = DetectByTimedLoop();
		}

		// restore original process priority
//		SetThreadPriority(GetCurrentThread(), nOriginalThreadPriority);
//		SetPriorityClass(GetCurrentProcess(), dwOriginalPriority);
	}
}

/*
** Parameters for RDTSC cpu speed detection
*/
#define TEST_DURATION		10		// duration of timed loop in milliseconds
#define TARGET_DEVIATION	500000	// deviation of 1/2 MHz
#define MIN_LOOPS			10		// minimum number of tests to perform
#define MAX_LOOPS			100		// maximum number of tests to perform

/*
** Detection of CPU speed using RDTSC instruction
*/
#pragma optimize ("", off)
DWORD CCpuSpeed::DetectByRDTSC ()
{
	LARGE_INTEGER	counterFreq;
	DWORD			new_freq, ave_freq = 0;
	int				dxFreq, deviation, temp = 0, count = 0;

	if (QueryPerformanceFrequency(&counterFreq))
	{
		// do one test and ditch the result to settle things down
		TestRDTSC(TEST_DURATION);
		// run again and store as the initial value
		ave_freq = TestRDTSC(TEST_DURATION);
		// now loop for a minimum of MIN_LOOPS, then until either MAX_LOOPS or deviation falls within spec
		do
		{
			new_freq = TestRDTSC(TEST_DURATION);
			ave_freq = (new_freq + ave_freq) / 2;

			// calculate deviation from average
			dxFreq = new_freq - ave_freq;
			if (dxFreq < 0)
				dxFreq *= -1;
			temp += dxFreq;
			deviation = temp / ++count;
		}
		while ((deviation > TARGET_DEVIATION || count < MIN_LOOPS) && count < MAX_LOOPS);
		// return result in KHz
		return ave_freq;
	}
	return 0;
}

/*
** Performs the actual RDTSC test
** Counts elapsed cpu cycles over a measured time interval to derive clock speed
*/
DWORD CCpuSpeed::TestRDTSC(DWORD dwTestDuration)
{
	LARGE_INTEGER qwFreq, qwDuration;
	LARGE_INTEGER qwStartTime, qwGoalTime, qwFinishTime, qwElapsedTime;
	LARGE_INTEGER qwStartTicks, qwFinishTicks, qwElapsedTicks;

	// calculate required ticks for requested duration
	QueryPerformanceFrequency(&qwFreq);
	qwDuration.QuadPart = (dwTestDuration * qwFreq.QuadPart) / 1000;

	// get initial cpu ticks and initial timer ticks
	QueryPerformanceCounter (&qwStartTime);
	qwStartTicks = GetTickCount();

	// wait for the specified test interval
	qwGoalTime.QuadPart = qwStartTime.QuadPart + qwDuration.QuadPart;
	do
	{
		QueryPerformanceCounter(&qwFinishTime);
	} while (qwFinishTime.QuadPart < qwGoalTime.QuadPart);
//	Sleep(dwTestDuration);

	// get final cpu ticks and final timer ticks
	QueryPerformanceCounter(&qwFinishTime);
	qwFinishTicks = GetTickCount();

	// calculate elapsed values
	qwElapsedTicks.QuadPart = qwFinishTicks.QuadPart - qwStartTicks.QuadPart;
	qwElapsedTime.QuadPart = qwFinishTime.QuadPart - qwStartTime.QuadPart;

	// calculate cpu frequency in KHz
//	QueryPerformanceFrequency(&qwFreq);
	DWORD dwResult = (DWORD)( (qwElapsedTicks.QuadPart * qwFreq.QuadPart) / (qwElapsedTime.QuadPart * 1000));

#ifdef _DEBUG
	LARGE_INTEGER qwInterval;
	// convert timer ticks into milliseconds
	qwInterval.QuadPart = (qwElapsedTime.QuadPart * 1000000) / qwFreq.QuadPart;
	TRACE ("MeasureSpeed() measured %d ticks in %d.%6.6d secs - calculated CPU speed : %d.%3.3dMHz\n", qwElapsedTicks.LowPart, 
		(qwInterval.LowPart / 1000000), (qwInterval.LowPart % 1000000),
		(dwResult / 1000), (dwResult % 1000));
#endif

//	CString strMessage;
//	strMessage.Format("calculated cpu speed %d.%3.3dMHz", (dwResult / 1000), (dwResult % 1000));
//	AfxMessageBox (strMessage, MB_ICONINFORMATION);

	return dwResult;
}

/*
** obtain CPU cycle count since startup by emitting an RDTSC instruction
*/
LARGE_INTEGER CCpuSpeed::GetTickCount()
{
	LARGE_INTEGER	count;
	
	_asm
	{
		cli
		xor		eax, eax
		push	ecx
		push	edx
		_emit	0x0f
		_emit	0xa2
		pop		edx
		pop		ecx
		_emit	0x0f
		_emit	0x31
		mov		count.HighPart, edx
		mov		count.LowPart, eax
		sti
	}

	return count;
}

/*
** Detect cpu speed by timing a known set of instructions
*/
DWORD CCpuSpeed::DetectByTimedLoop ()
{
	WORD	wResult;
	// run test and discard to settle things down
	TestLoop();
	WORD wLo = 0, wHi = 0;

	// then ten runs and take the average
	for (int n = 0 ; n < 10 ; n++)
	{
		// ten runs to improve accuracy
		TestLoop();
		__asm
		{
			add	wLo, ax
			adc wHi, 0
		}
	}
	
	// divide to get average
	__asm
	{
		mov	ax, wLo
		mov	dx, wHi
		add	ax, 5			// to prevent rounding error
		adc	dx, 0
		mov	cx, 10
		div	cx
		mov	wResult, ax		// store test result
	}

	// Get CPU class and use to correct the result for CPU efficiency
	CCpuType cpu;
	DWORD dwCpuLevel = cpu.GetLevel();

	// ...for 486 class
	DWORD dwFactor = 22500;
	// ...for 586 class
	if (dwCpuLevel == CCpuType::level5)
		dwFactor = 14000;
	// ...for 686 class
	else if (dwCpuLevel == CCpuType::level6)
		dwFactor = 5000;
	
	// divide test result into it to yield actual bus speed in MHz
	return (dwFactor / wResult) * 1000;
}

/*
** Measure tick count for a known loop of instructions
*/
WORD CCpuSpeed::TestLoop ()
{
	LARGE_INTEGER	start, finish;
	
	_asm	cli;

	// Get start time
	QueryPerformanceCounter (&start);

	// Perform the tight-timed loop - 1000 x AAD instruction
	__asm
	{						
		push	cx
		mov		cx, 1000
lp1:	aad
		dec		cx
		jnz		lp1
		pop		cx
	}

	// Get finish time
	QueryPerformanceCounter (&finish);

	_asm	sti;

	return (WORD)(finish.QuadPart - start.QuadPart);
}

#pragma optimize ("", on)

/*
** Round an input value in MHz to nearest known CPU speed
*/
int CCpuSpeed::NormaliseSpeed (int nOriginal)
{
	int nKnownFreqs[] = {   16,  25,  33,  50,  60,  66,  75,  90,
						   100, 116, 120, 133, 150, 166, 180, 200,
						   233, 266, 300, 333, 350, 366, 380, 400,
						   433, 450, 466, 475, 500, 533, 550, 566,
						   600, 633, 650, 667, 700, 733, 750, 766,
						   800, 850, 866, 900, 933,1000,1066,1133,
						  1200,1266,1300 };
	int nCount = sizeof(nKnownFreqs) / sizeof(int);
	int nLower, nUpper = 0;
	
	// If the speed is > 1300 then simply optimize to the nearest 100Mhz
	if (nOriginal > 1300)
	{
		nOriginal = ((nOriginal + 50) / 100) * 100;
		return nOriginal;
	}
	else
	{
		for (int n = 0 ; n < (nCount - 1) ; n++)
		{
			nLower = nUpper;
			// upper limit is halfway between values
			nUpper = (nKnownFreqs[n] + nKnownFreqs[n + 1]) / 2;
			// does the value fall within the limits ?
			if (nOriginal >= nLower && nOriginal <= nUpper)
				return nKnownFreqs[n];
		}
		
		// must be the last one
		return nKnownFreqs[nCount - 1];
	}
}

