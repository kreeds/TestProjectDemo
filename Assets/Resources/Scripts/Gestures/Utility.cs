using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class GenericCopier<T>
{
    public static T DeepCopy(object objectToCopy)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, objectToCopy);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (T) binaryFormatter.Deserialize(memoryStream);
        }
    }
}


public class Utility 
{
	public static IEnumerator DelayInSeconds(float seconds, Action<bool> callback)
	{
		yield return new WaitForSeconds(seconds);
		callback(true);
	}
}
