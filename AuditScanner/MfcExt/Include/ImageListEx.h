
// FILE:	ImageListEx.h
// PURPOSE:	Extended Image list to handle 256 colour bitmaps as icons
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology 2003
// HISTORY:	JRFT - 28.04.2003 - written
#pragma once

class CImageListEx : public CImageList
{
public:
	void Create ();
	int AddBitmap (UINT nBitmapID);
	BOOL ReplaceBitmap (int nIndex, UINT nBitmapID);
};
