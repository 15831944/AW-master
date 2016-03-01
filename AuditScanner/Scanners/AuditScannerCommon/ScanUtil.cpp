
// FILE:	ScanUtil.cpp
// PURPOSE:	General Purpose utilities to support SoftScan library
// AUTHOR:	JRF Thornley - Copyright (C) PMD Technology Services Ltd 1998
// HISTORY:	JRFT - 03.04.2001 - memory allocation in FindVersionString now upped to prevent Win2000 "funny" - needs further investigation
// NOTES:	Whilst these routines may be implemented more fully elsewhere
//			these versions are specifically developed to minimise code size

#include "stdafx.h"

#include "scanutil.h"		// headers for these functions

#define new DEBUG_NEW

// characters not permissible in a file name
static unsigned char szIllegalChars[] = "\"*/:<>?\\|üìÄÅÉØ×";


///////////////////////////////////////////////////////////////////////////////
//  V E R S I O N   R E S O U R C E   R O U T I N E S

/*
** FOLLOWING ROUTINE ADAPTED FROM UTOPIA/MIKE O'HANLON CODE
** IT HAS NOT BEEN TESTED IN THIS CONFIGURATION SO USE WITH CARE!
*/
// Return DOS Exe file header description
CString ReadDosExeDescription (LPCSTR pszFileName)
{
#define DOS_EXE_SIG		"MZ"
#define OS2_EXE_SIG		"NE"
#define OS2_EXE_LE_SIG	"LE"
#define NT_EXE_SIG		"PE"

	typedef struct tagExeHeader {
		char szSignature[2];
		WORD wLastPageSize;
		WORD wFilePages;
		WORD wRelocationItems;
		WORD wHeaderParagraphs;
		WORD wMinParaAlloc;
		WORD wMaxParaAlloc;
		WORD wInitialSSValue;
		WORD wInitialSPValue;
		WORD wComplementedChecksum;
		WORD wInitialIPValue;
		WORD wInitialCSValue;
		WORD wRelocationTableOffset;
		WORD wOverlayNum;
		BYTE r1[32];
		DWORD dwNewExeOffset;
	} ExeHeader;

	typedef struct tagNewExeHeader {
		char szSignature[2];
		BYTE bLinkVersion;
		BYTE bLinkRevision;
		WORD wEntryTableOffset;
		WORD wEntryTableLength;
		DWORD dwChecksum;
		WORD wModuleFlag;
		WORD wSegmentNumber;
		WORD wHeapSize;
		WORD wStackSize;
		WORD wInitialIPValue;
		WORD wInitialCSValue;
		WORD wInitialSPValue;
		WORD wInitialSSValue;
		WORD wNumSegments;
		WORD wNumModuleRefs;
		WORD wNumNonResBytes;
		WORD wSegmentTableOffset;
		WORD wResourceTableOffset;
		WORD wResidentNamesTableOffset;
		WORD wModuleRefTableOffset;
		WORD wImportedNameTableOffset;
		DWORD dwNonResidNamesTableOffset;
		WORD wNumMovableEntryPoints;
		WORD wAlignmentShiftCount;
		WORD wNumResSegments;
		BYTE bTargetOS;
		BYTE bAdditionalInfo;
		WORD wFastLoadArea;
		WORD wFastLoadLength;
		WORD wr1;
		WORD wWinVersionNo;
	} NewExeHeader;
	// Variables
	ExeHeader exeHdr;
	NewExeHeader newExeHdr;
	BYTE	bDescLen;
	char	szBuffer[256];

	CString strResult;

	// try and open the file
	CFile file;
	if (!file.Open(pszFileName, CFile::modeRead))
		return strResult;

	// read the standard exe file header
	memset (&exeHdr, 0, sizeof(exeHdr));
	memset (&newExeHdr, 0, sizeof(exeHdr));
	if (sizeof(exeHdr) != file.Read(&exeHdr, sizeof(exeHdr)))
		return strResult;

	// read successfully - check for new executable format
	if ( strncmp(exeHdr.szSignature, DOS_EXE_SIG, 2) || (exeHdr.dwNewExeOffset < 0x40) )
		return strResult;

	// read the new header data
	if (!file.Seek(exeHdr.dwNewExeOffset, CFile::begin) || sizeof(NewExeHeader) != file.Read(&newExeHdr, sizeof(NewExeHeader)))
		return strResult;

	// read new header ok - do we recognise it ?
	if ( strncmp(OS2_EXE_SIG, newExeHdr.szSignature, 2))
		return strResult;
	
	// yes - seek and read size of Non-resident names table
	if (!file.Seek (newExeHdr.dwNonResidNamesTableOffset, CFile::begin) || (sizeof(BYTE) != file.Read(&bDescLen, sizeof(BYTE))))
		return strResult;

	// try and read the descriptor
	if (bDescLen > 0 && (bDescLen == file.Read(&szBuffer, bDescLen)))
	{
		szBuffer[bDescLen] = '\0';
		strResult = szBuffer;
	}
	return strResult;
}


///////////////////////////////////////////////////////////////////////////////
//   C R C   C A L C U L A T I N G   R O U T I N E S

static WORD wCrcTable [256] = {
	0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
	0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
	0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
	0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
	0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
	0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
	0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
	0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
	0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
	0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
	0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
	0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
	0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
	0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
	0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
	0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
	0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
	0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
	0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
	0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
	0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
	0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
	0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
	0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
	0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
	0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
	0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
	0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
	0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
	0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
	0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
	0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0 
};

// This assumes that file open will not fail!
#pragma warning(disable:4244)
WORD CalculateCRC (const char * lpszFileName)
{
	BYTE Buffer[1024];
	DWORD dwBytesRead;

	// open the file
	HANDLE hFile = CreateFile (lpszFileName, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, /*FILE_ATTRIBUTE_NORMAL*/FILE_FLAG_SEQUENTIAL_SCAN, NULL);
	if (INVALID_HANDLE_VALUE == hFile)
	{
		TRACE ("\nCould not open %s", lpszFileName);
		TRACE ("\nError code was 0x%X", GetLastError());
		return 0;
	}
	
	WORD wCRC = 0;
	do
	{
		ReadFile (hFile, Buffer, sizeof(Buffer), &dwBytesRead, NULL);
		ASSERT (dwBytesRead != HFILE_ERROR);
		if (dwBytesRead)
			for (DWORD dw = 0 ; dw < dwBytesRead ; dw++)
				wCRC = wCrcTable[(wCRC >> 8) ^ Buffer[dw]] ^ (wCRC << 8);
	}
	while (dwBytesRead == sizeof(Buffer));

	// close file
	CloseHandle (hFile);

	// return CRC value
	return wCRC;
}
#pragma warning(default:4244)

/*
** Mangles a string into a 2 byte word. Done using the sum of
** the ordinal value if each letter multiplied by it's position
** in the string. Typically overflows after about 35 chars
** (ORIGINALLY WRITTEN IN PASCAL BY MIKE O'HANLON)
*/
#pragma warning(disable:4244)
WORD MangleWord (LPCSTR pStr)
{
	WORD wSum = 0;
	for (size_t n = 0 ; n < strlen(pStr) ; n++)
		wSum += ((n + 1) * (unsigned char)pStr[n]);
	return wSum;
}
#pragma warning(default:4244)

/*
** Attempts to recover Version Information from a given file. Note
** that this is only available for 32 bit windows executable files
*/
CString FindVersionString (const char * lpszFileName, const char * pszSearch)
{
	CString strResult;
	char szFileName[_MAX_PATH];
	strcpy (szFileName, lpszFileName);

	DWORD dwZero;
	DWORD dwSize = GetFileVersionInfoSize (szFileName, &dwZero);
	if (dwSize) 
	{
		char * pBuffer = new char [dwSize + 1 + 128];
		if ( GetFileVersionInfo	( szFileName, dwZero, dwSize, pBuffer)) 
		{
			// yes - loaded version info ok - get language information
			char szTranslationSubBlock[] = "\\VarFileInfo\\Translation";
			LPVOID	lpBuf;
			UINT	nLen;
			if (VerQueryValue (pBuffer, szTranslationSubBlock, &lpBuf, &nLen))
			{
				char szCharSet[9], szSubBlock[256];
				// returned language info ok - build the language string in HEX
				WORD word0 = ((LPWORD)lpBuf)[0];
				WORD word1 = ((LPWORD)lpBuf)[1];
				wsprintf (szCharSet, "%4.4X%4.4X", word0, word1);
				// now make the required search string
				wsprintf (szSubBlock, "\\StringFileInfo\\%s\\%s", szCharSet, pszSearch);
				// finally try and find it
				if (VerQueryValue (pBuffer, szSubBlock, &lpBuf, &nLen))
				{
					strResult = (LPCSTR)lpBuf;
					
					// (((CAN DO ANY CHECKING / STRIPPING HERE )))
					// Check for non-printing characters at the end of the string
					if (!strResult.IsEmpty())
					{
						char lastChar = strResult[strResult.GetLength() - 1];
						int charIdx = lastChar;
						if (charIdx <= 32)
							strResult.SetAt(strResult.GetLength() - 1 ,'\0');
					}
				}
			}
		}
		delete [] pBuffer;
	}
	return strResult;
}


//
//    MakeUniqueFileName
//    ==================
//
//    Make a unique filename by adding a numeric suffix to the base name supplied until
//    the resultant filename is unique within the specified folder.
//
CString MakeUniqueFileName (LPCSTR szFolder ,LPCSTR szBaseName ,LPCSTR szExtension)
{
	CString baseName = szBaseName;
	CString folder = szFolder;
	CString extension = szExtension;

	// parse to check for illegal filename characters
	for (int n = 0 ; n < baseName.GetLength() ; n++)
	{
		unsigned char ch = baseName[n];
		if (ch < 32 || strchr((const char*)szIllegalChars, ch))
		{
			baseName.Delete(n);
			n--;
		}
	}

	// Construct the base file name which consists of the path and file name (no extension)
	CString baseFileName;
	EndWithBackslash(folder);
	baseFileName = folder + baseName;

	// is there an audit of this name already ?
	int nFileNumber = 0;
	CString outputFileName;
	CFile file;

	// Try a non-suffixed name at first by adding the extension and trying to open the file
	outputFileName = baseFileName + "." + extension;
	BOOL bStatus = file.Open(outputFileName, CFile::modeRead);
	while (bStatus) 
	{
		file.Close();

		// OK File existed - reformat the name adding the numeric suffix and incrementing the suffix
		// just in case a file of this name exists also.
		outputFileName.Format("%s%d.%s" ,baseFileName ,++nFileNumber ,extension); 
		bStatus = file.Open(outputFileName, CFile::modeRead);
	}

	// Return the name of the new unique file
	return outputFileName;
}