using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///	Extended dictionary that provides mapping of non-basic data type variables
/// </summary>
public class DictionaryExtended 
{
	/// <summary>
	/// Stores generic type dictioanry
	/// </summary>
	private Dictionary<string, object> dictInfo = null;

	/// <summary>
	/// Constructor to initialize Dictionary Extended with generic Dictionary
	/// </summary>
	/// <param name="info">Info.</param>
	public DictionaryExtended(Dictionary<string, object> info)
	{
		this.dictInfo = info;
	}

	/// <summary>
	/// Get object of type specified by user with the specified key.
	/// </summary>
	/// <param name="key">Key.</param>
	/// <typeparam name="T">The type of the object.</typeparam>
	public T Get<T>(string key)
	{
		if(key != null)
		{
			object obj = null; // used to store object to return
			try // to be safe
			{
				 dictInfo.TryGetValue(key, out obj);
				 return (obj != null)? (T)obj : default(T);		
			}
			catch(InvalidCastException)
			{
				new InvalidCastException( key + ": trying to cast <size=11><b>" + obj.GetType().ToString() + " into " + typeof(T).ToString() + " </b></size>" );
			}
		}
		return default(T);
	}

	/// <summary>
	/// Gets the dictionary
	/// </summary>
	/// <returns>The dictionary info.</returns>
	public Dictionary<string, object> GetDictionaryInfo()
	{
		return dictInfo;
	}
}
