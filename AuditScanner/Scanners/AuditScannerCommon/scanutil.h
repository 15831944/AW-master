
// FILE:	ScanUtil.h
// PURPOSE:	Declaration General Purpose utilities to support SoftScan library
// AUTHOR:	JRF Thornley - Copyright (C) PMD Technology Services Ltd 1998
// NOTES:	Used by Audit32.exe, Scanner.exe

#ifndef _SCANUTIL_DEF_
#define _SCANUTIL_DEF_

// Calculate CRC using original Utopia algorithm
WORD CalculateCRC (const char * lpszFileName);
// Get 16 bit EXE File Description
CString ReadDosExeDescription (LPCSTR pszFileName);
// Mangles a string into a single word
WORD MangleWord (LPCSTR pStr);
// Load version information from Windows exe/dll
CString FindVersionString (const char * lpszFileName, const char * pszSearch);

// Construct the name of a unique file given the components
CString MakeUniqueFileName (LPCSTR szFolder ,LPCSTR szBaseName ,LPCSTR szExtension);

#endif

