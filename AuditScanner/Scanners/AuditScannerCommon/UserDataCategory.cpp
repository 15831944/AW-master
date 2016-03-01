#include "UserDataCategory.h"
#include "stdafx.h"


//
//    CUSerDataCategoryList Class
//    ===========================
//
CUserDataCategory*	CUserDataCategoryList::FindCategory (LPCSTR categoryName)
{
	for (int index = 0; index < GetCount(); index++)
	{
		CUserDataCategory* pCategory = &_listCategories[index];
		if (pCategory->Name() == categoryName)
			return &_listCategories[index];
	}
	return NULL;
}



//
//    FindField
//	  =========
//
//    Find the specified User Defined Data Field given its name and category in delimited format of
//		category|field
//
CUserDataField*	CUserDataCategoryList::FindField (LPCSTR categoryName ,LPCSTR fieldName)
{
	// First find the category
	CUserDataCategory* pCategory = FindCategory(categoryName);
	if (pCategory == NULL)
		return NULL;
		
	// Now find the field in the category
	return pCategory->FindField(fieldName);	
}


//
//    CUserDataCategory Class
//    =======================
//
CUserDataCategory::CUserDataCategory(void)
{
	_name = "";
}

CUserDataCategory::~CUserDataCategory(void)
{
}


//
//    FindField
//    =========
//
//    Return a pointer to the (named) field within this category or NULL if none
//
CUserDataField*	CUserDataCategory::FindField (LPCSTR fieldName)
{
	for (int index = 0; index < (int)_listDataFields.GetCount(); index++)
	{
		CUserDataField* pField = &_listDataFields[index];
		if (pField->Label() == fieldName)
			return &_listDataFields[index];
	}
	return NULL;
}


//
//    CUserDataField Class
//    ====================
//
CUserDataField::CUserDataField()
{
	m_hCtl = NULL;
	m_dwDatabaseID = 0;
	_label = "";
	_length = 0;
	_type = CUserDataField::typeText;	
	_inputCase = CUserDataField::any;
	_mandatory = FALSE;
	_value1 = "";
	_value2 = "";	
	_minValue = 0;
	_maxValue = 0;
	//_envVariable = "";
}
