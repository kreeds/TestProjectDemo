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

	public static IEnumerator AlphaIn(Material mat, float duration)
	{
		// Control alpha from Sprite Renderer
		float t = 0;
		mat.color = Color.clear;
		while(t < 1)
    	{
			t += Time.deltaTime/duration;
			mat.color = Color.Lerp(Color.clear, Color.white, t);
			yield return null;
		}
		mat.color = Color.white;
	}

	public static IEnumerator AlphaOut(Material mat, float duration)
	{
		float t = 0;
		// Control alpha from Sprite Renderer
		while(t < 1) 
    	{
			t += Time.deltaTime/duration;
			mat.color = Color.Lerp(Color.white, Color.clear, t);
			yield return null;
		}

		mat.color = Color.clear;
	}

	public static IEnumerator AlphaInOut(MonoBehaviour mono, Material mat, float inDuration, float outDuration)
	{
		yield return mono.StartCoroutine(AlphaIn(mat, inDuration));
		yield return mono.StartCoroutine(AlphaOut(mat, outDuration));
	}
	
}
