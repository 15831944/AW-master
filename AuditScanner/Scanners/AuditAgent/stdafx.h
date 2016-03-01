#include <afxwin.h>
#include <afxdtctl.h>

#include "NTService.h"

#define _SQL_SERVER_
#define _WINDOWS_SERVICE_
#define _NO_SCAN16_

#include <afxcmn.h>
#include <afxinet.h>		// FTP classes
#include <winsock2.h>					// Support for Sockets

#define _AFXEXT


#include "../../MfcExt/Include/MfcExt.h"
//#include "../../MfcExt/Include/ListCtrl.h"		// Extended List Control Classes
//
#include "../AuditScannerCommon/AuditDataFile.h"	// The output audit data file
#include "../AuditScannerCommon/WMIScanner.h"		// The output audit data file
#include "../AuditScannerCommon/ScanUtil.h"			// specific auditing stuff
#include "../AuditScannerCommon/TcpClient.h"		// TCP Client
