using UnityEngine;
using System.Collections;

public class SpecialEffects : MonoBehaviour {
	
	public static GameObject MakeTrail(Vector3 pos)
	{
		GameObject obj = Instantiate(Resources.Load("Prefabs/FX/DrawPoint_FX")) as GameObject;
		obj.transform.position = pos;

		return obj;
	}
}
