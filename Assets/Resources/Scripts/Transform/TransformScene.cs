using UnityEngine;
using System.Collections;

public class TransformScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		Service.Init();	
		Service.Get<HUDService> ().StartScene ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
