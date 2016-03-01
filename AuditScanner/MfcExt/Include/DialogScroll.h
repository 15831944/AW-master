
// FILE:	DialogScroll.h
// PURPOSE:	Declaration of base class to implement scrolling dialogs
// AUTHOR:	JRF Thornley - copyright (C) Layton Technology Ltd 2006
// HISTORY:	JRFT - 24.01.2006 - written
// NOTES:	Derive from this class to implement a dialog box that automatically sprouts
//			scroll bars if it is resized below it's original design size.

#ifndef _SCROLL_DIALOG_DEF_
#define _SCROLL_DIALOG_DEF_

class CDialogScroll : public CDialog
{
	DECLARE_DYNAMIC(CDialogScroll);

public:
	CDialogScroll (UINT nIDTemplate, CWnd * pParentWnd = NULL);
	virtual ~CDialogScroll();

	// override the minimum size manually (otherwise is auto-set from the template)
	void SetMinSize (int cx, int cy);
	// return the minimum horizontal size before scrolling is invoked
	int GetMinSizeX () const
		{ return m_cx; }
	// return the minimum vertical size before scrolling is invoked
	int GetMinSizeY () const
		{ return m_cy; }

protected:
	int	m_cx;			// minimum size before scrolling
	int	m_cy;
	int	m_xScrollPos;	// current scroll positions
	int m_yScrollPos;

// Message handling
public:
	// Generated message map functions
	//{{AFX_MSG(CDialogScroll)
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

#endif //#ifndef _SCROLL_DIALOG_DEF_