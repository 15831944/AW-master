#include "stdafx.h"			// for MFC compatibility

/****************************************************************************
*                                  CCheckSum::add
* Inputs:
*        DWORD d: word to add
* Result: void
* 
* Effect: 
*        Adds the bytes of the DWORD to the CCheckSum
****************************************************************************/

void CCheckSum::add(DWORD value)
{
 union { DWORD value; BYTE bytes[4]; } data;
 data.value = value;
 for(UINT i = 0; i < sizeof(data.bytes); i++)
    add(data.bytes[i]);
} // CCheckSum::add(DWORD)

/****************************************************************************
*                                 CCheckSum::add
* Inputs:
*        WORD value:
* Result: void
* 
* Effect: 
*        Adds the bytes of the WORD value to the CCheckSum
****************************************************************************/

void CCheckSum::add(WORD value)
{
 union { DWORD value; BYTE bytes[2]; } data;
 data.value = value;
 for(UINT i = 0; i < sizeof(data.bytes); i++)
   add(data.bytes[i]);
} // CCheckSum::add(WORD)

/****************************************************************************
*                                 CCheckSum::add
* Inputs:
*        BYTE value:
* Result: void
* 
* Effect: 
*        Adds the byte to the CCheckSum
****************************************************************************/

void CCheckSum::add(BYTE value)
{
 BYTE cipher = (value ^ (r >> 8));
 r = (cipher + r) * c1 + c2;
 sum += cipher;
} // CCheckSum::add(BYTE)

/****************************************************************************
*                                 CCheckSum::add
* Inputs:
*        const CString & s: String to add
* Result: void
* 
* Effect: 
*        Adds each character of the string to the CCheckSum
****************************************************************************/

void CCheckSum::add(const CString & s)
{
 for(int i = 0; i < s.GetLength(); i++)
    add((BYTE)s.GetAt(i));
} // CCheckSum::add(CString)

/****************************************************************************
*                                 CCheckSum::add
* Inputs:
*        LPBYTE b: pointer to byte array
*        UINT length: count
* Result: void
* 
* Effect: 
*        Adds the bytes to the CCheckSum
****************************************************************************/

void CCheckSum::add(LPBYTE b, UINT length)
{
for(UINT i = 0; i < length; i++)
   add(b[i]);
} // CCheckSum::add(LPBYTE, UINT)