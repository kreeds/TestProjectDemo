using UnityEngine;
using System.Collections;

public class SeqMap : MonoBehaviour {

	[SerializeField]SceneFadeInOut fader;

	HUDService m_hudService;

	// Use this for initialization
	void Start () {

		//Enable All Services
		Service.Init();	
		m_hudService = Service.Get<HUDService>();
		m_hudService.StartScene();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClothingClick()
	{
		fader.ChangeScene("CharaScene");
	}
}
