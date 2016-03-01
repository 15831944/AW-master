#pragma once

// Product Flags
#define AWL_PRODUCT1	0x00000001
#define AWL_PRODUCT2	0x00000002
#define AWL_PRODUCT3	0x00000004
#define AWL_PRODUCT4	0x00000008
#define AWL_PRODUCT5	0x00000010
#define AWL_PRODUCT6	0x00000020
#define AWL_PRODUCT7	0x00000040
#define AWL_PRODUCT8	0x00000080

// Options
#define AWL_OPTION1		0x00000100
#define AWL_OPTION2		0x00000200
#define AWL_OPTION3		0x00000400
#define AWL_OPTION4		0x00000800
#define AWL_OPTION5		0x00001000
#define AWL_OPTION6		0x00002000
#define AWL_OPTION7		0x00004000
#define AWL_OPTION8		0x00008000

// Eval version if set
#define AWL_EVAL		0x00010000
#define AWL_SPARE		0x00020000

typedef enum eDurationUnits { days = 0, weeks, months, years };

class CProductLicense : public CObject
{
	DECLARE_DYNAMIC(CProductLicense)

public:
	// constructor
	CProductLicense ();
	CProductLicense (DWORD dwFlags, WORD wCount, CTime ctDate, int nDuration, eDurationUnits units, DWORD dwCompanyID);
	~CProductLicense ();

	// copy operator
	CProductLicense const & operator= (CProductLicense const & other);

	// convert to a readable string
	CString	ToString () const;
	// read from a string, and run a CRC check
	BOOL FromString (LPCSTR pszString);
	// serialize to disk file
	void Serialize (CArchive & ar);

	// generate unique check value
	DWORD CalculateCRC () const;
	// return the stored CRC check value
	DWORD GetCRC () const;
	// update the CRC value, following changes
	void SetCRC ();

	// Product Flags
	BOOL IsProductSet (DWORD dwProductFlag) const;
	void SetProduct (DWORD dwProductFlag, BOOL bSet = TRUE);

	// Option Flags
	BOOL IsOptionSet (DWORD dwOptionFlag) const;
	void SetOption (DWORD dwOptionFlag, BOOL bSet = TRUE);

	// specific flag settings
	BOOL IsEval () const
		{ return IsFlagSet (AWL_EVAL); }
	void SetEvalMode (BOOL bSet = TRUE)
		{ SetFlag (AWL_EVAL, bSet); }

	// returns true if product can run (is non-eval or within expiry)
	BOOL CanRun () const
		{ return (!IsEval() || ((GetExpiryDate() - CTime::GetCurrentTime()) > 0)); }
	// returns true if product is within support
	BOOL IsSupported () const
		{ return ((GetExpiryDate() - CTime::GetCurrentTime()) > 0); }

	// Licence Count
	WORD GetLicenceCount () const
		{ return m_wCount; }
	void SetLicenceCount (WORD wLicenceCount)
		{ m_wCount = wLicenceCount; }

	// Activation Date
	CTime GetStartDate () const;
	void SetStartDate (CTime ctStart);

	// Duration of Licence / Support
	WORD GetDuration () const;
	void SetDuration (WORD wDuration);
	// units in which the duration is measured (see enum)
	eDurationUnits GetDurationUnits () const
		{ return (eDurationUnits)(LOWORD(m_dwDate) >> 14); }
	void SetDurationUnits (eDurationUnits n);

	// Expiry Date
	CTime GetExpiryDate () const;
	// return time left of eval / support contract
	int GetRemainingDays () const;

	// Company Identifier key
	DWORD GetCompanyID () const;
	void SetCompanyID (DWORD dwCompanyID);

protected:
	// return TRUE if a specific flag is set
	BOOL IsFlagSet (DWORD dwFlag) const;
	// set or reset a specific flag value
	void SetFlag (DWORD dwFlag, BOOL bSet = TRUE);
	// encode the date and licence duration	
	void StoreDate (CTime ctDate, int nDuration, eDurationUnits units);
	// conversion to / from BASE32
	CString WordToBase32 (WORD wValue) const;
	CString DwordToBase32 (DWORD dwValue) const;
	WORD Base32ToWord (LPCSTR pszValue) const;
	DWORD Base32ToDword (LPCSTR pszValue) const;
	char IntToBase32 (BYTE nValue) const;
	BYTE Base32ToInt (char ch) const;
	// count the number of "1" bits in a DWORD
	DWORD CountBits (DWORD dwValue) const;
	// encrypt / decrypt the data values
	void Encrypt ();

protected:
	DWORD		m_dwFlags;
	WORD		m_wCount;
	DWORD		m_dwDate;
	DWORD		m_dwCompanyID;
};

/*
** Class to manage a collection of licences
*/
class CProductLicenseList : public CDynaList<CProductLicense>
{
public:
	// return TRUE if the only licence for the product is an Eval one
	BOOL IsEval (DWORD dwProductCode);
	// Return TRUE if product has a current valid licence
	BOOL CanRunProduct (DWORD dwProductCode);
};