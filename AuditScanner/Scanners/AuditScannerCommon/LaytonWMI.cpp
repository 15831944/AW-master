
#include "stdafx.h"
#include <atlbase.h>
#include "LaytonWMI.h"

CLaytonWMI::CLaytonWMI()
{ 
	m_pIWbemServices = NULL;
	m_pEnumClassObject = NULL;
	m_pClassObject = NULL;

	// Initialize COM.
	m_hResult =  CoInitializeEx(0, COINIT_MULTITHREADED); 
	if (FAILED(m_hResult))
		AfxThrowOleException(m_hResult);
}

CLaytonWMI::~CLaytonWMI()
{
	Disconnect();
	CoUninitialize();
}

//
//    Connect
//    =======
//
//    Establish a WMI connection to the specified workspace
//
BOOL CLaytonWMI::Connect(LPCTSTR lpstrNameSpace)
{
	CLogFile log;

	try
	{
		m_pIWbemServices = NULL;
		m_pEnumClassObject = NULL;
		BOOL bResult = TRUE;

		// Adjust the security to allow client impersonation.
		m_hResult = CoInitializeSecurity( NULL, -1, NULL, NULL, 
										  RPC_C_AUTHN_LEVEL_NONE, 
										  RPC_C_IMP_LEVEL_IMPERSONATE, 
										  NULL, 
										  EOAC_NONE,
										  NULL );
		if (FAILED(m_hResult))
		{
			// CMD 8.3.4
			// We ignore this error as the Agent seems to work perfectly happily even if the above call fails
			// and if we exit because of this error we cannot make ANY WMI calls which actually work perfectly.
			//
			//log.Format("Error calling CoInitializeSecurity. Reason: (0x%lX)\n", m_hResult);
			//return FALSE;
		}

		// Create an instance of the WbemLocator interface.
		IWbemLocator *pIWbemLocator = NULL;
		CComBSTR pNamespace = CComBSTR( lpstrNameSpace);

		if ((m_hResult = CoCreateInstance( CLSID_WbemLocator, NULL, CLSCTX_INPROC_SERVER,
										   IID_IWbemLocator, (LPVOID *) &pIWbemLocator)) == S_OK)
		{
			// If already connected, release m_pIWbemServices.
			if (m_pIWbemServices)
				m_pIWbemServices->Release();

			// Using the locator, connect to CIMOM in the given namespace.
			if((m_hResult = pIWbemLocator->ConnectServer( BSTR( pNamespace),
											 NULL,  // using current account for simplicity
											 NULL,	// using current password for simplicity
											 0L,	// locale
											 0L,	// securityFlags
											 //128,
											 NULL,	// authority (domain for NTLM)
											 NULL,	// context
											 &m_pIWbemServices)) == S_OK)
			{
				// Indicate success.
				bResult = TRUE;
			}
			else
			{
				bResult = FALSE;
				log.Format("Error calling ConnectServer. Reason: (0x%lX)\n", m_hResult);
			}

			// Done with pIWbemLocator. 
			pIWbemLocator->Release(); 
		}
		else
		{
			bResult = FALSE;
			log.Format("Error calling CoCreateInstance. Reason: (0x%lX)\n", m_hResult);
		}

		// Done with pNamespace.
		return bResult;
	}

	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return FALSE;
	}
}


//
//    Disconnect
//    ==========
//
//    Disconnect from the WMI session
//
BOOL CLaytonWMI::Disconnect()
{
	try
	{
		if (m_pClassObject)
		{
			m_pClassObject->Release();
			m_pClassObject = NULL;
		}

		if (m_pEnumClassObject)
		{
			m_pEnumClassObject->Release();
			m_pEnumClassObject = NULL;
		}

		if (m_pIWbemServices)
		{
			m_pIWbemServices->Release();
			m_pIWbemServices = NULL;
		}

		return TRUE;
	}

	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return FALSE;
	}
}



BOOL CLaytonWMI::BeginEnumClassObject(LPCTSTR lpstrObject)
{
	try
	{
		// Get the object class
		CComBSTR className = CComBSTR( lpstrObject);
		if ((!m_pIWbemServices))
			return FALSE;

		if (m_pEnumClassObject)
		{
			m_pEnumClassObject->Release();
			m_pEnumClassObject = NULL;
		}

		// Get the list of object instances.
		m_hResult = m_pIWbemServices->CreateInstanceEnum( BSTR( className),	0, NULL, &m_pEnumClassObject);
		if (FAILED( m_hResult))
			return FALSE;
		return TRUE;
	}

	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return FALSE;
	}
}



BOOL CLaytonWMI::MoveNextEnumClassObject()
{
	try
	{
		ULONG uReturned = 1;
		if (!m_pEnumClassObject)
			return FALSE;

		if (m_pClassObject)
		{
			m_pClassObject->Release();
			m_pClassObject = NULL;
		}

		// enumerate through the resultset.
		m_hResult = m_pEnumClassObject->Next(2000,				// timeout in two seconds
											  1,				// return just one storage device
											  &m_pClassObject,	// pointer to storage device
											  &uReturned);		// number obtained: one or zero
		if (SUCCEEDED( m_hResult) && (uReturned == 1))
			return TRUE;
		return FALSE;
	}

	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return FALSE;
	}
}



BOOL CLaytonWMI::CloseEnumClassObject()
{
	try
	{
		if (m_pClassObject)
			m_pClassObject->Release();
		m_pClassObject = NULL;

		if (m_pEnumClassObject)
			m_pEnumClassObject->Release();
		m_pEnumClassObject = NULL;

		return TRUE;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return FALSE;
	}
}



LPCTSTR CLaytonWMI::GetClassObjectStringValue(LPCTSTR lpstrProperty)
{
	try
	{
		CComBSTR propName = CComBSTR( lpstrProperty);
		VARIANT variant;
		CIMTYPE pType;

		if (!m_pClassObject)
			return NULL;

		m_hResult = m_pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (SUCCEEDED( m_hResult)) 
		{
			if (pType == 101) 
			{
				m_csResult = strCimValue( variant, pType);
				VariantClear(&variant);
				return m_csResult;
			}

			else
			{
				m_csResult = strCimArrayValue( variant, pType);
				VariantClear(&variant);
				return m_csResult;
			}
		}

		return NULL;
	}

	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return NULL;
	}
}



DWORD CLaytonWMI::GetClassObjectDwordValue(LPCTSTR lpstrProperty)
{
	try
	{
		CComBSTR propName = CComBSTR( lpstrProperty);
		VARIANT variant;
		CIMTYPE pType;

		if (!m_pClassObject)
			return 0;

		m_hResult = m_pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (SUCCEEDED( m_hResult))
		{
			DWORD dwRet = dwCimValue( variant, pType);		
			VariantClear(&variant);
			return dwRet;
		}
		return 0;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return 0;
	}
}



__int64 CLaytonWMI::GetClassObjectI64Value(LPCTSTR lpstrProperty)
{
	try
	{
		CComBSTR propName = CComBSTR( lpstrProperty);
		VARIANT variant;
		CIMTYPE pType;

		if (!m_pClassObject)
			return 0;

		m_hResult = m_pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (SUCCEEDED( m_hResult))
		{
			__int64 i64RetVal = i64CimValue( variant, pType);
			VariantClear(&variant);
			return i64RetVal;
		}
		return 0;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return 0;
	}
}

BOOL CLaytonWMI::GetClassObjectBoolValue(LPCTSTR lpstrProperty)
{
	BOOL boolRetVal = 0;

	CLogFile log;
	try
	{
		VARIANT variant;
		CIMTYPE pType;
		CComBSTR propName = CComBSTR( lpstrProperty);

		if (m_pClassObject)
		{
			// Get the property name as a BSTR
			BSTR bstrName = (BSTR)propName;

			// Pass it to the GET
			m_hResult = m_pClassObject->Get(bstrName, 0L, &variant, &pType, NULL);
			if (SUCCEEDED(m_hResult))
			{
				boolRetVal = (V_BOOL(&variant) ? 1 : 0);
				VariantClear(&variant);
			}
		}
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
	}

	return boolRetVal;
}



unsigned __int64 CLaytonWMI::GetClassObjectU64Value(LPCTSTR lpstrProperty)
{
	unsigned __int64 ui64RetVal = 0;

	CLogFile log;
	try
	{
		VARIANT variant;
		CIMTYPE pType;
		CComBSTR propName = CComBSTR( lpstrProperty);

		if (m_pClassObject)
		{
			// Get the property name as a BSTR
			BSTR bstrName = (BSTR)propName;

			// Pass it to the GET
			m_hResult = m_pClassObject->Get(bstrName, 0L, &variant, &pType, NULL);
			if (SUCCEEDED(m_hResult))
			{
				ui64RetVal = u64CimValue( variant, pType);
				VariantClear(&variant);
			}
		}
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
	}

	return ui64RetVal;
}





LPCTSTR CLaytonWMI::GetRefElementClassObjectStringValue(LPCTSTR lpstrRefElement, LPCTSTR lpstrProperty)
{
	CLogFile log;

	try
	{
		CComBSTR elementName = CComBSTR( lpstrRefElement);
		CComBSTR propName = CComBSTR( lpstrProperty);
		CString	csObject;
		VARIANT variant;
		CIMTYPE pType;

		IWbemClassObject *pClassObject;
		if (!m_pClassObject)
			return NULL;

		m_hResult = m_pClassObject->Get( BSTR( elementName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			return NULL;

		csObject = strCimValue( variant, pType);
		if (csObject.IsEmpty())
		{
			VariantClear(&variant);
			return NULL;
		}

		// Clear the varient before using it again
		VariantClear(&variant);

		m_hResult = m_pIWbemServices->GetObject( CComBSTR( csObject),
												WBEM_FLAG_RETURN_WBEM_COMPLETE,
												NULL,
												&pClassObject,
												NULL);
		if (FAILED( m_hResult))
			return NULL;

		m_hResult = pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (FAILED(m_hResult))
		{
			pClassObject->Release();
			return NULL;
		}

		if(pType == 101)
				m_csResult = strCimValue( variant, pType);
		else
			m_csResult = strCimArrayValue( variant, pType);

		pClassObject->Release();
		VariantClear(&variant);

		return m_csResult;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return NULL;
	}
}



DWORD CLaytonWMI::GetRefElementClassObjectDwordValue(LPCTSTR lpstrRefElement, LPCTSTR lpstrProperty)
{
	CLogFile log;

	try
	{
		CComBSTR elementName = CComBSTR( lpstrRefElement);
		CComBSTR propName = CComBSTR( lpstrProperty);
		CString	csObject;
		VARIANT variant;
		CIMTYPE pType;
		IWbemClassObject *pClassObject;
		static DWORD dwResult;

		if (!m_pClassObject)
			return 0;

		m_hResult = m_pClassObject->Get( BSTR( elementName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			return 0;

		csObject = strCimValue( variant, pType);
		VariantClear(&variant);
		if (csObject.IsEmpty())
			return 0;

		m_hResult = m_pIWbemServices->GetObject( CComBSTR( csObject),
												WBEM_FLAG_RETURN_WBEM_COMPLETE,
												NULL,
												&pClassObject,
												NULL);
		if (FAILED( m_hResult))
			return 0;

		m_hResult = pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			dwResult = 0;
		else
			dwResult = dwCimValue( variant, pType);

		pClassObject->Release();
		VariantClear(&variant);
		return dwResult;
	}

	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return 0;
	}
}



__int64 CLaytonWMI::GetRefElementClassObjectI64Value(LPCTSTR lpstrRefElement, LPCTSTR lpstrProperty)
{
	try
	{
		CComBSTR elementName = CComBSTR( lpstrRefElement);
		CComBSTR propName = CComBSTR( lpstrProperty);
		CString	csObject;
		VARIANT variant;
		CIMTYPE pType;
		IWbemClassObject *pClassObject;
		static __int64 i64Result;

		if (!m_pClassObject)
			return 0;

		m_hResult = m_pClassObject->Get( BSTR( elementName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			return 0;

		csObject = strCimValue( variant, pType);
		VariantClear(&variant);
		if (csObject.IsEmpty())
			return 0;

		m_hResult = m_pIWbemServices->GetObject( CComBSTR( csObject),
												WBEM_FLAG_RETURN_WBEM_COMPLETE,
												NULL,
												&pClassObject,
												NULL);
		if (FAILED( m_hResult))
			return 0;

		VariantClear(&variant);
		m_hResult = pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			i64Result = 0;
		else
			i64Result = i64CimValue( variant, pType);
		VariantClear(&variant);
		pClassObject->Release();
		return i64Result;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return 0;
	}
}



unsigned __int64 CLaytonWMI::GetRefElementClassObjectU64Value(LPCTSTR lpstrRefElement, LPCTSTR lpstrProperty)
{
	try
	{
		CComBSTR elementName = CComBSTR( lpstrRefElement);
		CComBSTR propName = CComBSTR( lpstrProperty);
		CString	csObject;
		VARIANT variant;
		CIMTYPE pType;
		IWbemClassObject *pClassObject;
		static unsigned __int64 u64Result;

		if (!m_pClassObject)
			return 0;

		m_hResult = m_pClassObject->Get( BSTR( elementName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			return 0;

		csObject = strCimValue( variant, pType);
		VariantClear(&variant);
		if (csObject.IsEmpty())
			return 0;

		m_hResult = m_pIWbemServices->GetObject( CComBSTR( csObject),
												WBEM_FLAG_RETURN_WBEM_COMPLETE,
												NULL,
												&pClassObject,
												NULL);
		if (FAILED( m_hResult))
			return 0;

		m_hResult = pClassObject->Get( BSTR( propName), 0L, &variant, &pType, NULL);
		if (FAILED( m_hResult))
			u64Result = 0;
		else
			u64Result = u64CimValue( variant, pType);
		pClassObject->Release();
		VariantClear(&variant);
		return u64Result;
	}
	catch (CException *pEx)
	{
		pEx->Delete();
		m_hResult = WBEM_E_FAILED;
		return 0;
	}
}



HRESULT CLaytonWMI::GetLastErrorWMI()
{
	return m_hResult;
}



LPCTSTR CLaytonWMI::strCimValue(VARIANT &variant, CIMTYPE &pType)
{
	//COleDateTime pOleDate;
	static CString csResult;

	if ((variant.vt == VT_NULL) || (variant.vt == VT_EMPTY))
	{
		csResult.Empty();
	}
	else
	{
		switch (pType)
		{
			case CIM_ILLEGAL:
				csResult = _T( "CIM_ILLEGAL");
				break;

			case CIM_EMPTY:
				csResult = _T( "");
				break;

			case CIM_SINT8:
				csResult.Format(_T( "%hd"),V_I1(&variant));
				break;

			case CIM_UINT8:
				csResult.Format(_T( "%hu"),V_UI1(&variant));
				break;

			case CIM_SINT16:
				csResult.Format(_T( "%d"),V_I2(&variant));
				break;

			case CIM_UINT16:
				csResult.Format(_T( "%u"),V_UI2(&variant));
				break;

			case CIM_SINT32:
				csResult.Format(_T( "%ld"),V_I4(&variant));
				break;

			case CIM_UINT32:
				csResult.Format(_T( "%lu"),V_UI4(&variant));
				break;

			case CIM_SINT64:
				csResult = V_BSTR(&variant);
				break;

			case CIM_UINT64:
				csResult = V_BSTR(&variant);
				break;

			case CIM_REAL32:
				csResult.Format(_T( "%e"),V_R4(&variant));
				break;

			case CIM_REAL64:
				csResult.Format(_T( "%le"),V_R8(&variant));
				break;

			case CIM_BOOLEAN:
				csResult = (V_BOOL(&variant) ? _T("TRUE") : _T("FALSE"));
				break;

			case CIM_STRING:
				csResult = V_BSTR(&variant);
				break;

			case CIM_DATETIME:
				//pOleDate = COleDateTime( variant);
				//if (pOleDate.GetStatus() == COleDateTime::valid)
				//	csResult = pOleDate.Format( VAR_DATEVALUEONLY);
				//else
				//	csResult = V_BSTR( &variant);
				break;

			case CIM_REFERENCE:
				csResult = V_BSTR( &variant);
				break;

			case CIM_CHAR16:
				csResult = V_BSTR(&variant);
				break;

			case CIM_OBJECT:
				csResult = _T( "CIM_OBJECT");
				break;

			default:
				csResult = strVariantArray( variant);
				break;
		}
	}

	return csResult;
}



DWORD CLaytonWMI::dwCimValue(VARIANT &variant, CIMTYPE &pType)
{
	if ((variant.vt == VT_NULL) || (variant.vt == VT_EMPTY))
		return 0;

	switch (pType)
	{
		case CIM_SINT8:
			return V_I1(&variant);

		case CIM_UINT8:
			return V_UI1(&variant);

		case CIM_SINT16:
			return V_I2(&variant);

		case CIM_UINT16:
			return V_UI2(&variant);

		case CIM_SINT32:
			return V_I4(&variant);

		case CIM_UINT32:
			return V_UI4(&variant);

		case CIM_BOOLEAN:
			return (V_BOOL(&variant) ? 1 : 0);

		default:
			return 0;
	}
}



__int64 CLaytonWMI::i64CimValue(VARIANT &variant, CIMTYPE &pType)
{
	CString csResult;

	if ((variant.vt == VT_NULL) || (variant.vt == VT_EMPTY))
		return 0;

	switch (pType)
	{
		case CIM_SINT8:
			return V_I1(&variant);

		case CIM_UINT8:
			return V_UI1(&variant);

		case CIM_SINT16:
			return V_I2(&variant);

		case CIM_UINT16:
			return V_UI2(&variant);

		case CIM_SINT32:
			return V_I4(&variant);

		case CIM_UINT32:
			return V_UI4(&variant);

		case CIM_SINT64:
			csResult = V_BSTR(&variant);
			return _ttoi64( csResult);

		case CIM_UINT64:
			csResult = V_BSTR(&variant);
			return _ttoi64( csResult);

		case CIM_BOOLEAN:
			return (V_BOOL(&variant) ? 1 : 0);

		default:
			return 0;

	}
}



unsigned __int64 CLaytonWMI::u64CimValue(VARIANT &variant, CIMTYPE &pType)
{
	CString csResult;
	if ((variant.vt == VT_NULL) || (variant.vt == VT_EMPTY))
		return 0;

	switch (pType)
	{
		case CIM_SINT8:
			return V_I1(&variant);

		case CIM_UINT8:
			return V_UI1(&variant);

		case CIM_SINT16:
			return V_I2(&variant);

		case CIM_UINT16:
			return V_UI2(&variant);

		case CIM_SINT32:
			return V_I4(&variant);

		case CIM_UINT32:
			return V_UI4(&variant);

		case CIM_SINT64:
			csResult = V_BSTR(&variant);
			return _ttoi64( csResult);

		case CIM_UINT64:
			csResult = V_BSTR(&variant);
			return _ttoi64( csResult);

		case CIM_BOOLEAN:
			return (V_BOOL(&variant) ? 1 : 0);

		default:
			return 0;

	}
}



LPCTSTR CLaytonWMI::strCimArrayValue(VARIANT &variant, CIMTYPE &pType)
{
	LONG dwSLBound = 0;
    LONG dwSUBound = 0;
    VARIANT v;
    LONG i;
	static CString strRet;
	HRESULT hr;
	CIMTYPE cimTypeWithoutArray;

    if (!V_ISARRAY(&variant))
        return strCimValue( variant, pType);

    // Check that there is only one dimension in this array
    if ((V_ARRAY(&variant))->cDims != 1)
		return NULL;

	// Check that there is atleast one element in this array
    if ((V_ARRAY(&variant))->rgsabound[0].cElements == 0)
		return NULL;

    hr = SafeArrayGetLBound( V_ARRAY(&variant), 1, (long FAR *)&dwSLBound);
	if (FAILED( hr))
		return NULL;

    hr = SafeArrayGetUBound( V_ARRAY(&variant), 1, (long FAR *)&dwSUBound);
    if (FAILED( hr))
		return NULL;

	// Calculate CIM type without the array flag
	cimTypeWithoutArray = pType ^ CIM_FLAG_ARRAY;

	// Parse the array
	strRet = _T( "");

	v.vt=(unsigned short)cimTypeWithoutArray;
	DECIMAL_SETZERO(v.decVal);
	for (i = dwSLBound; i <= dwSUBound; i++)
	{
		  hr = SafeArrayGetElement( V_ARRAY(&variant), (long FAR *)&i, &v.lVal);
		  if (FAILED(hr))
				continue;

		  strRet += strCimValue( v, cimTypeWithoutArray);
		  if (i < dwSUBound)
				strRet += _T( ";");
		  DECIMAL_SETZERO(v.decVal);
	}
    return(strRet);
}



LPCTSTR CLaytonWMI::strVariant( VARIANT variant)
{
	static CString strRet;
	strRet = _T( "N/A");

	switch (variant.vt)
	{
		case VT_EMPTY:
		case VT_NULL:
			strRet = _T( "");
			break;

		case VT_I1:
			strRet.Format(_T( "%hd"),V_I2(&variant));
			break;

		case VT_I2:
			strRet.Format(_T( "%d"),V_I2(&variant));
			break;

		case VT_I4:
			strRet.Format(_T( "%ld"),V_I4(&variant));
			break;

		case VT_I8:
			strRet.Format(_T( "%I64d"), V_I8(&variant));
			break; 

		case VT_UI1:
			strRet.Format(_T( "%hu"),V_UI1(&variant));
			break;

		case VT_UI2:
			strRet.Format(_T( "%u"),V_UI2(&variant));
			break;

		case VT_UI4:
			strRet.Format(_T( "%lu"),V_UI4(&variant));
			break;

		case VT_UI8:
			strRet.Format(_T( "%I64u"),V_UI8(&variant));
			break;

		case VT_INT:
			strRet.Format(_T( "%d"),V_INT(&variant));
			break;

		case VT_UINT:
			strRet.Format(_T( "%u"),V_UINT(&variant));
			break;

		case VT_R4:
			strRet.Format(_T( "%e"),V_R4(&variant));
			break;

		case VT_R8:
			strRet.Format(_T( "%le"),V_R8(&variant));
			break;

		case VT_CY:
			//strRet = COleCurrency(variant).Format();
			break;

		case VT_DATE:
			//strRet = COleDateTime(variant).Format( VAR_DATEVALUEONLY);
			break;

		case VT_BSTR:
			strRet = V_BSTR(&variant);
			break;

		case VT_DISPATCH:
			strRet = _T( "VT_DISPATCH");
			break;

		case VT_ERROR:
			strRet = _T( "VT_ERROR");
			break;

		case VT_BOOL:
			strRet = (V_BOOL(&variant) ? _T("TRUE") : _T("FALSE"));
			break;

		case VT_VARIANT:
			strRet = _T( "VT_VARIANT");
			break;

		case VT_UNKNOWN:
			strRet = _T( "VT_UNKNOWN");
			break;

		case VT_VOID:
			strRet = _T( "VT_VOID");
			break;

		case VT_HRESULT:
			strRet = _T( "VT_HRESULT");
			break;

		case VT_PTR:
			strRet = _T( "VT_PTR");
			break;

		case VT_SAFEARRAY:
			strRet = _T( "VT_SAFEARRAY");
			break;

		case VT_CARRAY:
			strRet = _T( "VT_CARRAY");
			break;

		case VT_USERDEFINED:
			strRet = _T( "VT_USERDEFINED");
			break;

		case VT_LPSTR:
			strRet = _T( "VT_LPSTR");
			break;

		case VT_LPWSTR:
			strRet = _T( "VT_LPWSTR");
			break;

		case VT_FILETIME:
			strRet = _T( "VT_FILETIME");
			break;

		case VT_BLOB:
			strRet = _T( "VT_BLOB");
			break;

		case VT_STREAM:
			strRet = _T( "VT_STREAM");
			break;

		case VT_STORAGE:
			strRet = _T( "VT_STORAGE");
			break;

		case VT_STREAMED_OBJECT:
			strRet = _T( "VT_STREAMED_OBJECT");
			break;

		case VT_STORED_OBJECT:
			strRet = _T( "VT_STORED_OBJECT");
			break;

		case VT_BLOB_OBJECT:
			strRet = _T( "VT_BLOB_OBJECT");
			break;

		case VT_CF:
			strRet = _T( "VT_CF");
			break;

		case VT_CLSID:
			strRet = _T( "VT_CLSID");
			break;
	}

	return strRet;
}

LPCTSTR CLaytonWMI::strVariantArray( VARIANT var)
{
    LONG dwSLBound = 0;
    LONG dwSUBound = 0;
    VARIANT v;
    LONG i;
	static CString strRet;
	HRESULT hr;

    if(!V_ISARRAY(&var))
        return strVariant( var);

	// Check that there is only one dimension in this array
    if ((V_ARRAY(&var))->cDims != 1)
		return NULL;

    // Check that there is atleast one element in this array
    if ((V_ARRAY(&var))->rgsabound[0].cElements == 0)
		return NULL;

    // We know that this is a valid single dimension array
    hr = SafeArrayGetLBound( V_ARRAY(&var), 1, (long FAR *)&dwSLBound);
	if (FAILED( hr))
		return NULL;

    hr = SafeArrayGetUBound( V_ARRAY(&var), 1, (long FAR *)&dwSUBound);
    if (FAILED( hr))
		return NULL;

	strRet = _T( "");

	DECIMAL_SETZERO(v.decVal);

    for (i = dwSLBound; i <= dwSUBound; i++)
	{
        hr = SafeArrayGetElement( V_ARRAY(&var), (long FAR *)&i, &v);
        if (FAILED(hr))
            continue;

        if (i < dwSUBound)
		{
			strRet += strVariant( v);
			strRet += _T( ";");
		}

        else
		{
			strRet += strVariant( v);
        }

		DECIMAL_SETZERO(v.decVal);
    }

    return(strRet);
}