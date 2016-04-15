using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Service.Init();	
		Service.Get<HUDService>().StartScene();
	}

	void OnTap(){
		Service.Get<HUDService> ().ChangeScene ("EventScene");
	}
}
