
// FILE:	ImageListEx.h
// PURPOSE:	Extended Image list to handle 256 colour bitmaps as icons
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology 2003
// HISTORY:	JRFT - 28.04.2003 - written

#include "stdafx.h"

#define new DEBUG_NEW

void CImageListEx::Create ()
{
	VERIFY(CImageList::Create(16, 16, ILC_COLOR8|ILC_MASK, 40, 0));
}

/*
** This function takes a bitmap resource ID value and adds it to the image list.
** The return value is the index of the image
*/
int CImageListEx::AddBitmap (UINT nBitmap)
{
	// Load bitmap from resource ID
	CBitmap bmp;
	VERIFY(bmp.LoadBitmap(MAKEINTRESOURCE(nBitmap)));

	// Add bitmap to image list and store index value
	int nIndex = Add(&bmp, RGB(255,0,255));

	// Delete GDI object so we don't get a resource leak
	bmp.DeleteObject();

	// return index of added bitmap
	return nIndex;
}

/*
** Override to replace a bitmap with a different one specified by a resource ID
*/
BOOL CImageListEx::ReplaceBitmap (int nIndex, UINT nBitmap)
{
	// load from resource ID
	CBitmap bm;
	VERIFY(bm.LoadBitmap(MAKEINTRESOURCE(nBitmap)));

	BOOL bResult = Replace (nIndex, &bm, NULL);

	bm.DeleteObject();

	return bResult;
}
