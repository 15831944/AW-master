// Dictionary.h: interface/implementation for the Dictionary class.
//
// Dictionary is a templatized class which is a direct copy of
// the VBScript Scripting.Dictionary object.  All methods
// and properties are provided with a syntax that is either
// identical to the VBS implementation, or with only trivial
// differences (mainly, C++ member functions require parentheses
// where VBscript methods don't)
//
// Comparison to VBS given in sample at end of header file.
// For most things, VBS docs can be used.
// 
// Copyright 1998, James M. Curran, All Rights reserved.
//  May be used freely, provided copyright noticed remains.
//  May not be sold in source code form.
//
// email: JamesCurran@CompuServe.Com
//   WWW: http://www.NJTheater.Com/JamesCurran
//////////////////////////////////////////////////////////////////////

#pragma once

#include <map>
#include <vector>
#include <utility>
#include <iostream>
#include <algorithm>
#include <functional>

template<typename KEY, typename ITEM>
class Dictionary  : std::map<KEY, ITEM>
{
private:
	class DIH
	{
	private:
		iterator					iter;
	public:
		DIH(iterator i) : iter(i) {}
		void operator=(ITEM i)
		{	*iter = std::pair<KEY,ITEM>( (*iter).first, i);	}

		operator ITEM()
		{	return (*iter).second;	}

		friend std::ostream& operator<<(std::ostream& os, const DIH& dih);
	};

	class DKH
	{
	private:
		iterator					iter;
		std::map<KEY, ITEM>&	mp;
	public:
		DKH(std::map<KEY, ITEM>& m, iterator i) : mp(m), iter(i) {}
		void operator=(KEY k)
		{	std::pair<KEY,ITEM>  p(k, (*iter).second);
			mp.erase(iter);
			mp.insert(p);
		}
	};

	void operator=(const Dictionary<KEY, ITEM>& d);	// disallowed (not written)

public:
	Dictionary() {};
	virtual ~Dictionary(){};
	
	void	Add(KEY k, ITEM i)
	{	insert(std::pair<KEY,ITEM>(k,i));	}

	bool Exists(KEY k) const 
	{	return (find(k) != end());	}

	size_type Count() const
	{	return size();	}

	void Remove(KEY k)
	{	erase(k);	}

	void RemoveAll()
	{	clear();	}


	std::vector<KEY>	Keys() const
	{	std::vector<KEY>		keys;
		std::transform(begin(), end(), std::back_inserter(keys), getKEY);
		return keys;
	}

	std::vector<ITEM>	Items() const
	{	std::vector<ITEM>		items;
		std::transform(begin(), end(), std::back_inserter(items), getITEM);
		return items;
	}

	DIH	Item(KEY k)
	{
		iterator	iter = find(k);
		if (iter == end())
			throw "Key not found in Item() call";
		return (DIH(iter));
	}

	DIH operator[](KEY k)
	{	return Item(k);		}

	DIH operator()(KEY k)
	{	return Item(k);		}

	DKH	Key(KEY k)
	{
		iterator	iter = find(k);
		if (iter == end())
			throw "Key not found in Key() call";
		return (DKH(*this, iter));
	}

protected:
	static KEY getKEY(std::pair<KEY,ITEM> p)
	{	return p.first; }

	static KEY getITEM(std::pair<KEY,ITEM> p)
	{	return p.second; }

};