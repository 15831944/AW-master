
// FILE:	DialogScroll.cpp
// PURPOSE:	Implementation of base class to implement scrolling dialogs
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology Ltd 2006
// HISTORY:	JRFT - 24.01.2006 - written

#include "stdafx.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

IMPLEMENT_DYNAMIC(CDialogScroll, CDialog)

/*
**	Constructor
*/
CDialogScroll::CDialogScroll (UINT nIDTemplate, CWnd * pParentWnd/* = NULL*/) : CDialog(nIDTemplate, pParentWnd)
{
	m_cx = m_cy = m_xScrollPos = m_yScrollPos = 0;
}

/*
**	Destructor
*/
CDialogScroll::~CDialogScroll ()
{
}

BEGIN_MESSAGE_MAP(CDialogScroll, CDialog)
	//{{AFX_MSG_MAP(CDialogScroll)
	ON_WM_SIZE()
	ON_WM_HSCROLL()
	ON_WM_VSCROLL()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void CDialogScroll::OnSize(UINT nType, int cx, int cy) 
{
	CDialog::OnSize(nType, cx, cy);

	if (GetSafeHwnd())
	{
		// first lets sort scrolling - is this the initial call ?
		if (!m_cx)
		{
			// yes - store the intended minimum size
			m_cx = cx;
			m_cy = cy;
		}

		// wipe out any existing scroll offset
		ScrollWindow (m_xScrollPos, m_yScrollPos, NULL, NULL);
		m_xScrollPos = m_yScrollPos = 0;

		BOOL bChange;
		int cxScroll = 0, cyScroll = 0;

		// we do this iteratively to manage the tricky interaction between the two scroll bars
		do
		{
			bChange = FALSE;
			int cxVScrollBar = cyScroll ? ::GetSystemMetrics (SM_CXVSCROLL) : 0;
			int cyHScrollBar = cxScroll ? ::GetSystemMetrics (SM_CYHSCROLL) : 0;

			// do we need a horizontal scrollbar ?
			int cxNew = (m_cx > (cx - cxVScrollBar)) ? m_cx - cx/* + cxVScrollBar*/ : 0;
			if (cxNew != cxScroll)
			{
				cxScroll = cxNew;
				bChange = TRUE;
			}

			// repeat with vertical
			int cyNew = (m_cy > (cy - cyHScrollBar)) ? m_cy - cy/* + cyHScrollBar*/ : 0;
			if (cyNew != cyScroll)
			{
				cyScroll = cyNew;
				bChange = TRUE;
			}
		} while (bChange);

		SetScrollRange (SB_HORZ, 0, cxScroll, FALSE);
		SetScrollRange (SB_VERT, 0, cyScroll, FALSE);

		SetScrollPos (SB_HORZ, m_xScrollPos, TRUE);
		SetScrollPos (SB_VERT, m_yScrollPos, TRUE);
	}
}

void CDialogScroll::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	int cxLine = 1;
	int cxPage = m_cx;
	int nNewPos = 0, nMin, nMax;
	GetScrollRange (SB_HORZ, &nMin, &nMax);

	switch (nSBCode)
	{
		case SB_BOTTOM:
			nNewPos = nMax;
			break;
		case SB_LINEDOWN:
			nNewPos = m_xScrollPos + cxLine;
			break;
		case SB_LINEUP:
			nNewPos = m_xScrollPos - cxLine;
			break;
		case SB_PAGEDOWN:
			nNewPos = m_xScrollPos + cxPage;
			break;
		case SB_PAGEUP:
			nNewPos = m_xScrollPos - cxPage;
			break;
		case SB_THUMBPOSITION:
		case SB_THUMBTRACK:
			nNewPos = nPos;
			break;
		case SB_TOP:
			nNewPos = 0;
			break;
		case SB_ENDSCROLL:
			return;
	}

	// range check it
	nNewPos = max(nNewPos, 0);
	nNewPos = min(nNewPos, nMax);

	// scroll the window
	int cxToScroll = m_xScrollPos - nNewPos;
	if (cxToScroll)
	{
		ScrollWindow (cxToScroll, 0, NULL, NULL);
		SetScrollPos (SB_HORZ, nNewPos, TRUE);
		m_xScrollPos = nNewPos;
	}
	
	CDialog::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CDialogScroll::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* /*pScrollBar*/) 
{
	// what increment values (in pixels) do we want ?
	int cyLine = 1;
	int cyPage = m_cy;
	int nNewPos = 0, nMin, nMax;
	GetScrollRange (SB_VERT, &nMin, &nMax);

	switch (nSBCode)
	{
		case SB_BOTTOM:
			nNewPos = nMax;
			break;
		case SB_LINEDOWN:
			nNewPos = m_yScrollPos + cyLine;
			break;
		case SB_LINEUP:
			nNewPos = m_yScrollPos - cyLine;
			break;
		case SB_PAGEDOWN:
			nNewPos = m_yScrollPos + cyPage;
			break;
		case SB_PAGEUP:
			nNewPos = m_yScrollPos - cyPage;
			break;
		case SB_THUMBPOSITION:
		case SB_THUMBTRACK:
			nNewPos = nPos;
			break;
		case SB_TOP:
			nNewPos = 0;
			break;
		case SB_ENDSCROLL:
			return;
	}

	// range check it
	nNewPos = max(nNewPos, 0);
	nNewPos = min(nNewPos, nMax);

	// scroll the window
	int cyToScroll = m_yScrollPos - nNewPos;
	if (cyToScroll)
	{
		ScrollWindow (0, cyToScroll, NULL, NULL);
		SetScrollPos (SB_VERT, nNewPos, TRUE);
		m_yScrollPos = nNewPos;
	}
}

