#pragma once

class CAssetType
{
public:
	CAssetType(void);
public:
	~CAssetType(void);
	
public:
	CString&	GetAssetType()
	{ return _assetType; }
	
private:
	CString	_assetType;
};
