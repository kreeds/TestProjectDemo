using UnityEngine;
using System.Collections;

public class HUDService : CSingleton {

	HUDHandler m_handler = null;
	SceneFadeInOut m_scene = null;
	SoundService m_sound = null;

	GameObject m_transitor = null;

	void Awake()
	{
		//Check if object exist in scene
		GameObject HUDObject = GameObject.Find("HUD");
		if (HUDObject == null) {
			HUDObject = Instantiate (Resources.Load ("Prefabs/HUD")) as GameObject;
		}
		m_handler = HUDObject.GetComponentInChildren<HUDHandler>();

		m_transitor = GameObject.Find("LoadTransitor");
		if (m_transitor == null) {
			m_transitor = Instantiate (Resources.Load ("Prefabs/LoadTransitor")) as GameObject;
			DontDestroyOnLoad(m_transitor);
		}

		m_scene = m_transitor.GetComponentInChildren<SceneFadeInOut>();
		m_sound = Service.Get<SoundService>();

	}

	public HUDHandler HUDControl
	{
		get{ 
			if (m_handler == null){	
				GameObject HUDObject = GameObject.Find("HUD");
				if (HUDObject == null) {
					HUDObject = Instantiate (Resources.Load ("Prefabs/HUD")) as GameObject;
				}
				m_handler = HUDObject.GetComponentInChildren<HUDHandler>();
			}
			return m_handler;
		}
	}

	public void StartScene()
	{
		
		m_scene.StartScene();
	}
	public void CreateBattleHUD()
	{
//		m_handler.CreateBattleHUD();
		HUDControl.CreateBattleHUD ();
	}

	public void ShowTop(bool show)
	{
//		m_handler.ShowTop(show);
		HUDControl.ShowTop (show);
	}

	public void ShowBottom(bool show)
	{
//		m_handler.ShowBottom(show);
		HUDControl.ShowBottom (show);
	}

	public void ShowMid(bool show)
	{
//		m_handler.ShowMid(show);
		HUDControl.ShowMid (show);
	}
	public void ShowQuestTime(bool show)
	{
		HUDControl.ShowQuestTime(show);
	}
	public void ChangeScene(string name)
	{
		m_transitor.SetActive(true);
		m_sound.StopMusic(0);
		m_scene.ChangeScene(name);
		Resources.UnloadUnusedAssets();
	}

	public void ReturnToHome()
	{
		QuestEvent.nextSceneID = 0;
		ChangeScene ("EventScene");
	}
}
