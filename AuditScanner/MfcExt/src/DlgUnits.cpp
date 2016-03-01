
// FILE:	DlgUnits.cpp
// PURPOSE:	Class for conversion between dialog units and screen pixels
// AUTHOR:	JRF Thornley - Copyright (C) InControl Desktop Systems Ltd 1999,2000
// HISTORY:	JRFT - 14.07.1999 - Written
//			JRFT - 24.03.2001 - m_yBaseUnits NOT now rounded - check this on Win95???
// NOTES:	Ensure that DlgUnits.h is included in the project stdafx.h file

#include "stdafx.h"

#define new DEBUG_NEW

// Constructor
CDlgUnit::CDlgUnit (CDialog * pDlg)
{
	// get window handle of dialog
	HWND hDlg = pDlg->GetSafeHwnd();
	// get the font in use
	HFONT hF = (HFONT)::SendMessage (hDlg, WM_GETFONT, 0, 0L);
	// create a device context for this window & font
	HDC hdc = GetDC (hDlg);
	HFONT hOldFont = (HFONT)SelectObject(hdc, hF);
	// get the font dimensions
	TEXTMETRIC tm;
	GetTextMetrics (hdc, &tm);
	// release the dc
	SelectObject (hdc, hOldFont);
	ReleaseDC (hDlg, hdc);
	// calculate x and y base units
	m_xBaseUnit = tm.tmAveCharWidth;
	m_yBaseUnit = tm.tmHeight;
	// ensure base units are even - round up if necessary
	if (m_xBaseUnit & 1)
		m_xBaseUnit++;
//	if (m_yBaseUnit & 1)
//		m_yBaseUnit++;
}

// Conversion from dialog units to screen co-ordinates
int CDlgUnit::XToScreen(int xDlgUnit)
{
	return (m_xBaseUnit * xDlgUnit) / 4;
}

int CDlgUnit::YToScreen (int yDlgUnit)
{
	return (m_yBaseUnit * yDlgUnit) / 8;
}

int CDlgUnit::XFromScreen (int xScreen)
{
	return (4 * xScreen) / m_xBaseUnit;
}

int CDlgUnit::YFromScreen (int yScreen)
{
	return (8 * yScreen) / m_yBaseUnit;
}
