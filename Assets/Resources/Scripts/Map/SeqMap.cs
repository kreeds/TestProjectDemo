using UnityEngine;
using System.Collections;

public class SeqMap : MonoBehaviour {

	[SerializeField]LAppModelProxy playerModel;

	SoundService m_soundService;
	AudioClip m_bgm;
	HUDService m_hudService;


	void Awake()
	{
		m_hudService = Service.Get<HUDService>();
		m_soundService = Service.Get<SoundService>();
	}
	// Use this for initialization
	void Start () {

		//Enable All Services
		Service.Init();	

		m_hudService.StartScene();
		m_bgm = Resources.Load("Music/outside") as AudioClip;
		m_soundService.PlayMusic(m_bgm, true);

		playerModel.GetModel ().StopBasicMotion (true);
		playerModel.SetClothes (2);
		playerModel.SetHair (1);
		playerModel.PlayIdleAnim ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClothingClick()
	{
		m_hudService.ChangeScene("CharaScene");
		m_soundService.StopMusic(m_bgm);

	}
}
