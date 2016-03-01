
// FILE:	CpuType.cpp
// PURPOSE:	Class for identification of host processor
// AUTHOR:	JRF Thornley - copyright (C) InControl Software 2001
// HISTORY:	JRFT - 11.07.2001 - reworked from original MthBoard.cpp/h code (originally developed 1998)
//			JRFT - 31.01.2002 - support added for Intel cpu identification via Brand Strings and Brand IDs (see Intel AP-485)

#include "stdafx.h"

///////////////////////////////////////////////////////////////////////////////
//
// Basic pseudo-singleton class for executing the CPUID instruction
//

/*
** Initialise statics
*/
int		CCpuID::m_nMfr = -1;
int		CCpuID::m_nMaxLevel = -1;

/*
** Constructor
*/
CCpuID::CCpuID()
{
	// has a previous object initialised the statics?
	if (-1 == m_nMfr)
	{
		// no - do it now, first check whether instruction can run at all
		BOOL bSupported = FALSE;
		__asm
		{
			pushfd					; get processor flags
			pop		eax
			mov		ecx, eax
			xor		eax, 200000h	; flip ID bit in EFLAGS - if this works then CPUID is supported
			push	eax
			popfd
			pushfd					; get the flags back
			pop		eax
			xor		eax, ecx		; has the bit changed
			jz		label			; no - CPUID not supported
			mov		bSupported, 1
label:		push	ecx				; restore original flags
			popfd
		}
		if (!bSupported)
		{
			m_nMfr = unknown;
		}
		else
		{
			// ok, run CPUID at level 0 to get max level, plus mfr string
			m_nMaxLevel = 0;
			DWORD dwLevel = 0;
			DWORD dwMfr1, dwMfr2, dwMfr3;
			RunCPUID(dwLevel, dwMfr1, dwMfr3, dwMfr2);
			// store returned level
			m_nMaxLevel = dwLevel;
			// check for Intels
			if (dwMfr1 == 'uneG' && dwMfr2 == 'Ieni' && dwMfr3 == 'letn')
				m_nMfr = intel;
			else if (dwMfr1 == 'htuA' && dwMfr2 == 'itne' && dwMfr3 == 'DMAc')
				m_nMfr = amd;
			else if (dwMfr1 == 'iryC' && dwMfr2 == 'snIx' && dwMfr3 == 'daet')
				m_nMfr = cyrix;
		}
	}
}

/*
** Execute the CPUID instruction and return results to caller
*/
void CCpuID::RunCPUID(DWORD & dwEAX, DWORD & dwEBX, DWORD & dwECX, DWORD & dwEDX)
{
	DWORD dw1, dw2, dw3, dw4;

	// check required level is available
	if ((int)dwEAX > m_nMaxLevel)
	{
		ASSERT(FALSE);	// Assertion failure means CPUID was attempted with EAX at a higher level than the cpu supports
		return;
	}

	dw1 = dwEAX;
	__asm
	{
		push	ebx;
		push	ecx;
		push	edx;
	
		mov		eax, dw1;
		_emit	00Fh
		_emit	0A2h

		mov		dw1, eax;
		mov		dw2, ebx;
		mov		dw3, ecx;
		mov		dw4, edx;

		pop		edx;
		pop		ecx;
		pop		ebx;
	}
	dwEAX = dw1;
	dwEBX = dw2;
	dwECX = dw3;
	dwEDX = dw4;
}

///////////////////////////////////////////////////////////////////////////////
//
//  G E N E R I C   C P U   T Y P E   D E T E C T I O N
//

/*
** Initialise Statics
*/
int		CCpuType::m_nLevel = 0;
CString	CCpuType::m_strMfr;
CString	CCpuType::m_strType;
DWORD	CCpuType::m_dwCpuID = 0;

// This is a list of the strings that we want to discard if found in a processor name
static const char* szDiscardStrings[] = { "CPU family","processor", "Processor" ,"(R)", "(r)", "(TM)", "(tm)", "CPU", "MHz" ,"GHz" };
static int nDiscardStrings = sizeof(szDiscardStrings) / sizeof(char *);

/*
** Constructor
*/
CCpuType::CCpuType()
{
}

/*
** Destructor
*/
CCpuType::~CCpuType()
{
}

/*
** Return the processor level (see the enum in the declaration)
*/
int CCpuType::GetLevel()
{
	if (m_nLevel == unknown)
		Detect();
	return m_nLevel;
}

/*
** Return processor manufacturer, may be empty
*/
const CString & CCpuType::GetMfr()
{
	if (m_nLevel == unknown)
		Detect();
	return m_strMfr;
}

/*
** Return processor type name
*/
const CString & CCpuType::GetType()
{
	if (m_nLevel == unknown)
		Detect();
	return m_strType;
}



//
//    RationalizeName
//    ===============
//
//    This function is intended to rationalize the processor name string passed into us according to the
//    rules laid down for the internal function GetRationalizedName
//
CString CCpuType::RationalizeName(CString& strProcessorName)
{
	CString strRationalizedName = strProcessorName;

	// Remove any leading and trailing spaces
	strRationalizedName.TrimLeft();
	strRationalizedName.TrimRight();

	// Rationalize white space in the string
	strRationalizedName = TrimSpaces(strRationalizedName);

	// Remove any 'speeds' from the processor name
	strRationalizedName = TrimSpeeds(strRationalizedName);

	// Strip off CPU if the name ends in it
	if (strRationalizedName.Right(3) == "CPU")
	{
		strRationalizedName = strRationalizedName.Left(strRationalizedName.GetLength() - 4);
	}

	// remove any copyright stuff etc
	for (int n = 0 ; n < nDiscardStrings ; n++)
	{
		while (-1 != strRationalizedName.Find(szDiscardStrings[n], 0))
			strRationalizedName.Delete(strRationalizedName.Find(szDiscardStrings[n], 0), strlen(szDiscardStrings[n]));
	}

	// Check to ensure that there is a space after Pentium as some ID strings do not include one
	int nPos = strRationalizedName.Find("Pentium");
	if (nPos != -1)
	{
		int nLength = strRationalizedName.GetLength();
		if ((nPos + 7) < nLength)
		{ 
			if (strRationalizedName[nPos + 7] != ' ')
				strRationalizedName = strRationalizedName.Mid(0, nPos + 7) + " " + strRationalizedName.Mid(nPos + 8);
		}
	}

	// Ensure that we add a manufacturer if there isn't one there already
	if ((strRationalizedName.Left(7) == "Pentium")
	||  (strRationalizedName.Left(4) == "pentium")
	||  (strRationalizedName.Left(4) == "Xeon")
	||  (strRationalizedName.Left(4) == "XEON")
	||	(strRationalizedName.Left(7) == "Celeron"))
	{
		strRationalizedName = "Intel " + strRationalizedName;
	}

	else if ((strRationalizedName.Left(6) == "Athlon")
	||  (strRationalizedName.Left(5) == "Duron"))
	{
		strRationalizedName = "AMD " + strRationalizedName;
	}

	// The above processes may have added spaces back in again so strip them again
	strRationalizedName.TrimRight();
	strRationalizedName = TrimSpaces(strRationalizedName);

	// Return this name
	return strRationalizedName;
}


CString CCpuType::TrimSpaces (CString& strInput)
{
	strInput.TrimLeft();
	strInput.TrimRight();

	CString strOutput = "";
	int pos = 0;
	while (pos < strInput.GetLength())
	{
		if (strInput[pos] == ' ')
		{
			// Skip over any additional spaces
			int skip = pos + 1;
			while ((skip<strInput.GetLength()) && (strInput[skip] == ' '))
			{
				skip++;
			}
			pos = skip - 1;
		}

		strOutput += strInput[pos++];
	}
	return strOutput;
}
	


//
//    TrimSpeeds
//    ==========
//
//    Remove any 'speeds' from a processor name
//
CString CCpuType::TrimSpeeds (CString& strInput)
{
	// On AMD chips it seems that processor speeds end with a + and are 4 digits long
	// so if we find a speed with 4 digits prior to it then we can strip them off
	int nDelimiter = strInput.Find("+");
	if (nDelimiter > 4)
	{
		CString strSpeed = strInput.Mid(nDelimiter - 4, 4);
		int speed = atoi(strSpeed);
		if (speed != 0)
			strInput = strInput.Left(strInput.GetLength() - (nDelimiter - 5) + 1) + strInput.Right(nDelimiter+1);

	}

	// On Intel Chips the speed appears to be introduced by an '@' symbol - we strip off everything after this
	nDelimiter = strInput.Find("@");
	if (nDelimiter != -1)
			strInput = strInput.Left(nDelimiter);

	// OK - now lets see if we can find either MHz or GHz and step back from here to the first character
	// that is NOT either a numeric or a period or a space 
	nDelimiter = strInput.Find("MHz");
	if (nDelimiter == -1)
		nDelimiter = strInput.Find("GHz");
	if (nDelimiter != -1)
	{
		for (int isub = nDelimiter - 1; isub>0; isub--)
		{

			char c = strInput[isub];
			if ((c != '.') && (c != ' ') && ((c < '0') || (c > '9')))
			{
				// End of string so strip out the speed
				CString strStart = strInput.Left(isub + 1);
				CString strEnd = strInput.Mid(nDelimiter+3);
				strInput =  strStart + strEnd;
				break;
			}
		}

	}

	return strInput;
}
		

//
//    GetRationalizedName
//    ===================
//
//    This function is intended to return a text string for the processor in a 'rationalized' fashion
//    that is all irrelevent characters are removed and we are left with a more baseline processor name
//    which hopefully will reduce the number of different processor names identified.
//
const CString CCpuType::GetRationalizedName(void)
{
	// Now build the full name of the processor from the Manufacturer and Type strings
	CString strRationalizedName = GetMfr();
	CString strType = GetType();
	if (strRationalizedName.GetLength())
		strRationalizedName += " ";
	strRationalizedName += strType;

	return RationalizeName(strRationalizedName);
}


/*
** Return detected cpu ID code (bit-packed family/model/stepping mode)
*/
DWORD CCpuType::GetCpuID()
{
	if (m_nLevel == unknown)
		Detect();
	return m_dwCpuID;
}

/*
** Detect the processor type
*/
void CCpuType::Detect()
{
	if (Is386())
	{
		// Yes - it's a 386
		m_strType = "80386";
		m_nLevel = level3;
	}
	else
	{
		CCpuID cpuid;

		// must be at least a 486 - does it support CPUID
		if (cpuid.GetMaxLevel() > -1)
		{
			// yes - run at level 1 to get basic processor ID
			m_dwCpuID = 1;
			DWORD dwDummy1, dwDummy2, dwDummy3;
			cpuid.RunCPUID(m_dwCpuID, dwDummy1, dwDummy2, dwDummy3);

			// rest of detection depends on Manufacturer
			switch (cpuid.GetMfr())
			{
				case CCpuID::intel:		
					m_strMfr = "Intel";
					DetectIntel();
					break;

				case CCpuID::cyrix:
					m_strMfr = "Cyrix";
					DetectCyrix();
					break;

				case CCpuID::amd:
					m_strMfr = "AMD";
					DetectAMD();
					break;
			}
		}
		else
		{
			// No CPUID Support - do specific Cyrix checks
			if (!IsCyrix())
			{
				// else assume its an unknown 486
				m_nLevel = level4;
				m_strType = "80486";
			}
		}
	}
}

/*
** Use CPUID instruction to detect Intel Processors
*/
void CCpuType::DetectIntel()
{
	// always store family number
	m_nLevel = m_dwCpuID >> 8;

	// first check for existence of a brand string
	CString strBuffer;
	if (GetBrandString(strBuffer))
	{
		ParseBrandString(strBuffer);
		m_strType = strBuffer;
		return;
	}

	// failing that check for existence of a brand ID
	DWORD dwEAX = 1, dwEBX, dwECX, dwEDX;
	CCpuID cpuid;
	cpuid.RunCPUID(dwEAX, dwEBX, dwECX, dwEDX);
	// if brand ID is supported it should report it in bits 0-7 of EBX
	DWORD dwBrandID = (dwEBX & 0xF);
	if (dwBrandID)
	{
		// ok, we have a brand ID - look it up
		switch (dwBrandID)
		{
			case 0x01:	m_strType = "Celeron";				break;
			case 0x02:	m_strType = "Pentium III";			break;
			case 0x03:
				if (m_dwCpuID == 0x000006B1)
					m_strType = "Celeron";
				else
					m_strType = "Pentium III Xeon";
				break;
			case 0x04:	m_strType = "Pentium III";			break;
			case 0x06:	m_strType = "Pentium III Mobile";	break;
			case 0x07:	m_strType = "Celeron";				break;
			case 0x08:	
			case 0x09:	m_strType = "Pentium 4";			break;
			case 0x0B:
			case 0x0E:	m_strType = "Xeon";					break;
			default:	m_strType = "Unknown";				break;
		}
		return;
	}

	// failing that just use straightforward CPUID check
	switch (m_dwCpuID >> 4)
	{
		// 486 family
		case 0x040:
		case 0x041:	m_strType = "80486DX";	break;
		case 0x042:	m_strType = "80486SX";	break;
		case 0x043:
		case 0x047:	m_strType = "80486DX2";	break;
		case 0x044:	m_strType = "80486SL";	break;
		case 0x045:	m_strType = "80486SX2";	break;
		case 0x048:
		case 0x049:	m_strType = "80486DX4";	break;
			
		// Pentiums
		case 0x050:	
		case 0x051:	m_strType = "Pentium P5";	break;
		case 0x052:
		case 0x057:	m_strType = "Pentium P54C";	break;
		case 0x053:	m_strType = "Pentium P24T";	break;
		case 0x054:
		case 0x058:	m_strType = "Pentium P55C";	break;

		// Pentium Pros etc
		case 0x060:
		case 0x061:	m_strType = "Pentium Pro";	break;
		case 0x063:
		case 0x065:
			{
				// Special Case - may be a Pentium II or a Celeron - check the cache size
				CCpuCache cache;
				int nInstCache, nDataCache, nL2Cache;
				if (cache.GetSize(&nInstCache, &nDataCache, &nL2Cache) && nL2Cache < 512)
					m_strType = "Celeron";
				else
					m_strType = "PentiumII";
			}
			break;
		case 0x066:	m_strType = "Celeron";		break;
		case 0x067:
		case 0x068:	m_strType = "PentiumIII";	break;
		
		default:
			{
				char szBuffer[24];
				wsprintf (szBuffer, "0x%X", m_dwCpuID);
				m_strType = szBuffer;
				m_nLevel = level6;
			}
			break;
	}
}

/*
** Perform Cyrix detection for chips with CPUID support
*/
void CCpuType::DetectCyrix()
{
	switch (m_dwCpuID >> 4)
	{
		case 0x044:	m_strType = "MediaGX";		m_nLevel = level4;	break;
		case 0x052:	m_strType = "6X86";			m_nLevel = level5;	break;
		case 0x054:	m_strType = "GXm";			m_nLevel = level5;	break;
		case 0x060:	m_strType = "6x86MX";		m_nLevel = level5;	break;
		default:
			break;
	}
}

/*
** Detects AMD processors with CPUID support
*/
void CCpuType::DetectAMD()
{
	// does it support a brand string via extended CPUID
	CString strTemp;
	if (GetBrandString(strTemp))
	{
		// clean it up, maybe recognising the named cpu
		ParseBrandString(strTemp);
		// store the final string, plus the family
		m_strType = strTemp;
		m_nLevel = m_dwCpuID >> 8;
		return;
	}

	// otherwise do a straight lookup on the CPUID signature
	switch (m_dwCpuID >> 4)
	{
		case 0x043:
		case 0x047:	m_strType = "80486DX2";		m_nLevel = level4;	break;
		case 0x048:
		case 0x049:	m_strType = "80486DX4";		m_nLevel = level4;	break;
		case 0x04E:
		case 0x04F:	m_strType = "5x86";			m_nLevel = level5;	break;
		case 0x050:
		case 0x051:
		case 0x052:
		case 0x053:	m_strType = "K5";			m_nLevel = level5;	break;
		case 0x056:
		case 0x057:
		case 0x058:
		case 0x059:	m_strType = "K6";			m_nLevel = level6;	break;
		default:
			break;
	}
}

/*
** Return the brand string obtained via extended CPUID support - currently AMD & Intel chips support this
*/
BOOL CCpuType::GetBrandString (CString & string)
{
	CCpuID cpuid;
	DWORD dwEAX = 0x80000000, dwEBX, dwECX, dwEDX;
	cpuid.RunCPUID(dwEAX, dwEBX, dwECX, dwEDX);
	if (dwEAX >= 0x80000004)
	{
		// brand string is supported - loop to get the 3 possible values
		for (DWORD dwFn = 0x80000002 ; dwFn <= 0x80000004 ; dwFn++)
		{
			char szBuffer[17];
			dwEAX = dwFn;
			cpuid.RunCPUID(dwEAX, dwEBX, dwECX, dwEDX);
			// convert into a text string and terminate
			LPDWORD p = (LPDWORD)szBuffer;
			*p++ = dwEAX;
			*p++ = dwEBX;
			*p++ = dwECX;
			*p = dwEDX;
			szBuffer[16] = '\0';
			// append to caller's string
			string += szBuffer;
		}
		return TRUE;
	}
	return FALSE;
}

/*
** Clean up a returned "Brand String" and attempt to normalise known cpu types
*/
void CCpuType::ParseBrandString(CString & string)
{
	int n;

	// first remove any copyright stuff, manufacturer names etc
	char * szToRemove[] = { "Intel", "AMD", "(R)", "(tm)", "(TM)" ,"processor" };
	int nCount = sizeof(szToRemove) / sizeof(char *);
	for (n = 0 ; n < nCount ; n++)
	{
		while (-1 != string.Find(szToRemove[n], 0))
			string.Delete(string.Find(szToRemove[n], 0), strlen(szToRemove[n]));
	}

	// now remove any leading or trailing spaces
	string.TrimLeft();
	string.TrimRight();
	
	char* szKnownCpus[] = {"Pentium 4"
						  ,"Mobile Pentium III" ,"Pentium III Xeon" ,"Pentium III Mobile" ,"Pentium III"
						  ,"Mobile Pentium II" ,"Pentium II Xeon" ,"Pentium II Mobile" ,"Pentium II"
						  ,"Pentium Pro" ,"Pentium" 
						  ,"XEON" 
						  ,"Celeron Mobile" ,"Mobile Celeron" ,"Celeron" };
//	char * szKnownCpus[] = { "Pentium 4" };
	nCount = sizeof(szKnownCpus) / sizeof(char*);
	for (n = 0 ; n < nCount ; n++)
	{
		if (-1 != string.Find(szKnownCpus[n], 0))
		{
			string = szKnownCpus[n];
			return;
		}
	}
}


				
/*
** Specific check for 386 class processor
*/
BOOL CCpuType::Is386()
{
	BOOL bRetValue = 1;
	// This algorithm attempts to set the AC flag. On a 486 or newer this
	// operation will succeed and the newly set flag can be read back. If
	// the processor is a 386 the flag will not be set and will stay zero
	__asm
	{
		pushfd
		pop		eax
		mov		ecx, eax
		xor		eax, 40000h			; set the AC bit in flags
		push	eax
		popfd
		pushfd
		pop		eax
		xor		eax, ecx			; has it been set ?
		jz		label
		mov		bRetValue, 0		; Yes - it's a 486 or better
label:	push	ecx					; restore original flags
		popfd
	}
/*
	if (bRetValue) {
		// Possible future enhancement - check for SX or DX chip here
	}
*/
	return bRetValue;
}

/*
** Detection for Cyrix processors that don't support CPUID
*/
BOOL CCpuType::IsCyrix ()
{
	BOOL bFound = FALSE;
	BYTE DIR0, DIR1;

	// this algorithm taken from www.cyrix.com
	// The "Device Id Registers" are accessed at port offsets FE and FF
	// To read a value the port offset is written to port 22h, then the
	// value is read back from port 23h. Two registers exist which are
	// DIR0 at FE and DIR1 at FF. For further details see www.cyrix.com
	__asm 
	{
		push	ax
		xor		ax, ax		; clear acc
		sahf				; clear flags
		mov		ax, 5		; perform a simple division
		mov		bx, 5
		div		bl
		lahf				; see whether flags have changed
		cmp		ah, 2		; bit 1 is always set on a cyrix
		jne		no_cy
		mov		bFound, 1	; found a Cyrix - set flag
		mov		al, 0FEh	; read DIR0 reg from port FE
		out		022h, al
		in		al, 023h
		mov		DIR0, al
		mov		al, 0FFh	; read DIR1 reg from port FF
		out		022h, al
		in		al, 023h
		mov		DIR1, al
no_cy:	pop		ax			; put further identification in here
	}

	// was a cyrix identified ?
	if (bFound)
	{
		// yes - store the name
		m_strMfr = "Cyrix";
		
		// now identify the exact processor
		switch (DIR0)
		{
			case 0x00:	// Cx486SLC
			case 0x01:	// Cx486DLC
			case 0x02:	// Cx486SLC2
			case 0x03:	// Cx486DLG2
			case 0x04:	// Cx486SRx
			case 0x05:	// Cx486DRx
			case 0x06:	// Cx486SRx2
			case 0x07:	// Cx486DRx2
			case 0x10:	// Cx486S(B)
			case 0x11:	// Cx486S2(B)
			case 0x12:	// Cx486Se(B)
			case 0x13:	// Cx486S2(B)
			case 0x1A:	m_strType = "Cx486";		m_nLevel = level4;	break;
			case 0x1B:	m_strType = "Cx486DX2";		m_nLevel = level4;	break;
			case 0x1C:	m_strType = "Cx486DX4";		m_nLevel = level4;	break;

			case 0x28:
			case 0x2A:	m_strType = "5x86(x1)";		m_nLevel = level5;	break;
			case 0x29:
			case 0x2B:	m_strType = "5x86(x2)";		m_nLevel = level5;	break;
			case 0x2D:
			case 0x2F:	m_strType = "5x86(x3)";		m_nLevel = level5;	break;
			case 0x2C:
			case 0x2E:	m_strType = "5x86(x4)";		m_nLevel = level5;	break;

			case 0x30:	m_strType = "6x86(x1)";		m_nLevel = level5;	break;
			case 0x31:	m_strType = "6x86(x2)";		m_nLevel = level5;	break;
			case 0x35:	m_strType = "6x86(x3)";		m_nLevel = level5;	break;
			case 0x34:	m_strType = "6x86(x4)";		m_nLevel = level5;	break;

			case 0x40:
			case 0x41:
			case 0x42:
			case 0x43:
			case 0x44:
			case 0x45:
			case 0x46:
			case 0x47:	m_strType = "MediaGX";		m_nLevel = level5;	break;

			case 0x50:
			case 0x58:	m_strType = "6x86MX(x1)";	m_nLevel = level5;	break;
			case 0x51:
			case 0x59:	m_strType = "6x86MX(x2)";	m_nLevel = level5;	break;
			case 0x52:
			case 0x5A:	m_strType = "6x86MX(x2.5)";	m_nLevel = level5;	break;
			case 0x53:			   
			case 0x5B:	m_strType = "6x86MX(x3)";	m_nLevel = level5;	break;
			case 0x54:
			case 0x5C:	m_strType = "6x86MX(x3.5)";	m_nLevel = level5;	break;
			case 0x55:
			case 0x5D:	m_strType = "6x86MX(x4)";	m_nLevel = level5;	break;
			case 0x56:
			case 0x5E:	m_strType = "6x86MX(x4.5)";	m_nLevel = level5;	break;
			case 0x57:
			case 0x5F:	m_strType = "6x86MX(x5)";	m_nLevel = level5;	break;

			default:	
				break;
		}
		return TRUE;
	}
 	return FALSE;
}

///////////////////////////////////////////////////////////////////////////////
//
//  C P U   C A C H E   S I Z E   D E T E C T I O N
//

// Initialize Statics
int CCpuCache::m_nL2Cache = -1;
int CCpuCache::m_nInstructionCache = -1;
int CCpuCache::m_nDataCache = -1;

/*
** Construction
*/
CCpuCache::CCpuCache()
{
}

/*
** Destruction
*/
CCpuCache::~CCpuCache()
{
}


/*
** Obtain L2 Cache Size if available
*/
BOOL CCpuCache::GetSize (LPINT pInstructionCache, LPINT pDataCache, LPINT pL2Cache)
{
	if (-1 == m_nL2Cache)
	{
		// obtain information if possible
		if (!Detect())
			return FALSE;
	}
	*pInstructionCache = m_nInstructionCache;
	*pDataCache = m_nDataCache;
	*pL2Cache = m_nL2Cache;
	return TRUE;
}


/*
** Does the actual detection work
*/
BOOL CCpuCache::Detect ()
{
	// ignore if already done
	if (-1 != m_nL2Cache)
		return TRUE;

	// ok, two checks - first is CPUID available at level 2?
	CCpuID cpuid;
	if (cpuid.GetMaxLevel() < 2)
		return FALSE;
	// also, is the chip an Intel ?
	if (cpuid.GetMfr() != CCpuID::intel)
		return FALSE;

	// ok, now do the actual detection...
	DWORD dwEAX = 2, dwEBX = 0, dwECX = 0, dwEDX = 0;
	cpuid.RunCPUID(dwEAX, dwEBX, dwECX, dwEDX);

//	ProcessFlags (dwEAX);
	ProcessFlags (dwEBX);
	ProcessFlags (dwECX);
	ProcessFlags (dwEDX);

	return TRUE;
}

/*
** Interpret info retrieved from level 2 run of CPUID into relevant cache sizes
*/
void CCpuCache::ProcessFlags (DWORD dwRegister)
{
	BOOL bUpperBitSet = (0 != (dwRegister >> 31));
	if  (!bUpperBitSet)
	{
		for (int n = 0 ; n < 4 ; n++)
		{
			BYTE b = (BYTE)(dwRegister & 0xFF);
			switch (b)
			{
				case 0x40:	m_nL2Cache = 0;		break;
				case 0x41:	m_nL2Cache = 128;	break;
				case 0x42:	m_nL2Cache = 256;	break;
				case 0x43:	m_nL2Cache = 512;	break;
				case 0x44:	m_nL2Cache = 1024;	break;
				case 0x45:	m_nL2Cache = 2048;	break;

				case 0x06:	m_nInstructionCache = 8;	break;
				case 0x08:	m_nInstructionCache = 16;	break;

				case 0x0A:	m_nDataCache = 8;	break;
				case 0x0C:	m_nDataCache = 16;	break;
			}
			dwRegister >>= 8;
		}
	}
}