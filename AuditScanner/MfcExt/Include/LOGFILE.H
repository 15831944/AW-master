
// FILE:	LogFile.h
// PURPOSE:	Declaration of easy-to-use application log
// AUTHOR:	JRF Thornley - Copyright (C) PMD Technology Services Ltd 1999, InControl Desktop Systems Ltd 2000,2001
// HISTORY:	JRFT - 28.04.1999 - Created
//			JRFT - 15.05.2002 - Made into a "singleton" class - only ever one instance per application
// NOTE:	To use this class first include an instance in the application object
//			to ensure that at least one copy stays in scope for the duration of the
//			application.  If app startup decides logging is required then call the
//			Open() member function, usually with no parameters. Anywhere in the code
//			can now access the log by instantiating CLogFile locally then calling the
//			Format() or Write() members. If a log as opened at startup then it will be
//			written to, if not the call will be ignored. Ensure that any such instances
//			go out of scope locally such that the usage count stays accurate. Hence
//			when the app shuts down the original instance will go out of scope and
//			reduce the usage count to zero, triggering automatic closure of the file.
//			Note that there is no need to remove log calls from "release" code, as they
//			have no effect and very little overhead unless Open() has been called.

#ifndef _LOGFILE_DEF_
#define _LOGFILE_DEF_

#define LOG_LOC_APP		1		// create logfile next to application
#define	LOG_LOC_TEMP	2		// create logfile in local temp folder
#define LOG_START		4		// write to log when application starts
#define LOG_FINISH		8		// write to log when application shuts down

class CLogFile
{
public:
	// Constructor
	CLogFile (int nFlags = LOG_LOC_APP|LOG_START|LOG_FINISH);
	// Destructor
	~CLogFile ();
	// Return fully qualified logfile name
	LPCSTR GetName ()
		{ return m_strFileName; }
	// Returns TRUE if logfile is active
	BOOL IsOpen()
		{ return m_bOpen; }
	// Open - if no name is passed then executable base name is taken + ".LOG"
	BOOL Open (LPCSTR pszFileName = NULL);
	// Write text to logfile, optionally with a carriage return, optionally flushing file immediately
	void Write (LPCSTR pszText, BOOL bNewLine = TRUE, BOOL bFlush = TRUE);
	// send a wsprintf style formatted string to logfile
	void Format (LPCSTR pszText, ... );
	// flush logfile to disk immediately
	void Flush ();
	// clear contents of logfile
	void Empty ()
	{ m_file.SetLength(0); }

	// Read the contents of the file into an internal list
	int	Read	(CDynaList<CString>& list);

protected:
	static int			m_nFlags;
	static BOOL			m_bOpen;
	static CString		m_strFileName;
	static CStdioFile	m_file;
	static int			m_nUsageCount;
};

#endif // #ifndef _LOGFILE_DEF_