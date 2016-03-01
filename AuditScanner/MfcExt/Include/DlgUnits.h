
// FILE:	DlgUnits.h
// PURPOSE:	Class for conversion between dialog units and screen co-ordinates
// AUTHOR:	JRF Thornley - Copyright (C) InControl Desktop Systems Ltd 1999,2000
// HISTORY:	JRFT - 14.07.1999 - written
// NOTES:	Include this file in stdafx.h

#ifndef _DLGUNIT_DEF_
#define _DLGUNIT_DEF_

class CDlgUnit
{
public:
	CDlgUnit (CDialog * pDlg);
	int XToScreen (int xDlg);
	int YToScreen (int yDlg);
	int XFromScreen (int xScreen);
	int YFromScreen (int yScreen);
private:
	int	m_xBaseUnit;
	int	m_yBaseUnit;
};

#endif
