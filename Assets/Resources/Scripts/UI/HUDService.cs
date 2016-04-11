﻿using UnityEngine;
using System.Collections;

public class HUDService : CSingleton {

	HUDHandler m_handler = null;
	SceneFadeInOut m_scene = null;
	SoundService m_sound = null;

	void Awake()
	{
		//Check if object exist in scene
		GameObject HUDObject = GameObject.Find("HUD");
		if (HUDObject == null) {
			HUDObject = Instantiate (Resources.Load ("Prefabs/HUD")) as GameObject;
		}
		m_handler = HUDObject.GetComponentInChildren<HUDHandler>();

		GameObject Transitor = GameObject.Find("LoadTransitor");
		if (Transitor == null) {
			Transitor = Instantiate (Resources.Load ("Prefabs/LoadTransitor")) as GameObject;
			DontDestroyOnLoad(Transitor);
		}
		m_scene = Transitor.GetComponentInChildren<SceneFadeInOut>();
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
		m_sound.StopMusic(0);
		m_scene.ChangeScene(name);
	}

	public void ReturnToHome()
	{
		QuestEvent.nextSceneID = 0;
		ChangeScene ("EventScene");
	}
}
