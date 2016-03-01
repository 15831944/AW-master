#include "stdafx.h"
#include "ApplicationRegistryMapping.h"


//////////////////////////////////////////////////////////////////////////
//
//    CApplicationRegistryMapping
//    ===========================
//
//    Class to hold a list of registry mappings for the specified application
//
CApplicationRegistryMapping::CApplicationRegistryMapping(void)
{
	_registryMappings.Empty();
}

CApplicationRegistryMapping::CApplicationRegistryMapping(CString& applicationName)
{
	_registryMappings.Empty();
	_applicationName = applicationName;
}


//
//    AddMapping
//    ==========
//
//    Add mappings from a mappings string
//
void CApplicationRegistryMapping::AddMapping(CString& mappingString)
{
	// Each separate mapping is delimited by a semicolon
	CDynaList<CString> listMappings;
	ListFromString(listMappings ,mappingString ,';');

	// Iterate through the individual mappings
	for (DWORD dw=0; dw<listMappings.GetCount(); dw++)
	{
		CString thisMapping = listMappings[dw];

		// The key/value pair are separated by an ',' sign
		CDynaList<CString> mappingParts;
		ListFromString(mappingParts ,thisMapping ,',');

		// Skip any that don't conform
		if (mappingParts.GetCount() != 2)
			continue;

		// And add a new mapping to our internal list
		AddMapping(mappingParts[0] ,mappingParts[1]);
	}
	listMappings.Empty();
}

void CApplicationRegistryMapping::AddMapping(CString& registryKey ,CString& registryValue)
{
	CRegistryMapping newMapping(registryKey ,registryValue);
	_registryMappings.Add(newMapping);
}

