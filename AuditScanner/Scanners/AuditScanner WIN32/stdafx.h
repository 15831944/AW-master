// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#ifndef _STDAFX_DEF_
#define _STDAFX_DEF_

#define VC_EXTRALEAN					// Exclude rarely-used stuff from Windows headers

#include <afxwin.h>						// MFC core and standard components
#include <afxext.h>						// MFC extensions
#include <afxinet.h>					// FTP classes
#include <afxpriv.h>					// ATL stuff ??
#include <WinSvc.h>						// Windows Services
#include <winsock2.h>					// Support for Sockets

#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>						// MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

#include "../../MfcExt/Include/MfcExt.h"
#include "../../MfcExt/Include/ListCtrl.h"		// Extended List Control Classes
//
#include "../AuditScannerCommon/AuditDataFile.h"	// The output audit data file
#include "../AuditScannerCommon/WMIScanner.h"		// The output audit data file
#include "../AuditScannerCommon/ScanUtil.h"			// specific auditing stuff
#include "../AuditScannerCommon/TcpClient.h"		// TCP Client

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !ifndef _STDAFX_DEF_
