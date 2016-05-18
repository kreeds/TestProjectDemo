using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {

	// Use this for initialization

	void Awake()
	{
		Service.Init();	
		Service.Get<HUDService>().StartScene();
		//Service.Get<SoundService>().PlaySound(Resources.Load ("Sound/titlecall00") as AudioClip, false);
		
	}
	void OnTap(){
		Service.Get<HUDService> ().ChangeScene ("EventScene");
	}
}
