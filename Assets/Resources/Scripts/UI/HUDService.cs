using UnityEngine;
using System.Collections;

public class HUDService : CSingleton {

	HUDHandler m_handler = null;
	SceneFadeInOut m_scene = null;

	void Awake()
	{
		//Check if object exist in scene
		GameObject HUDObject = GameObject.Find("HUD");
		if(HUDObject == null)
			HUDObject = Instantiate(Resources.Load("Prefabs/HUD")) as GameObject;
		m_handler = HUDObject.GetComponentInChildren<HUDHandler>();

		GameObject Transitor = GameObject.Find("LoadTransitor");
		if(Transitor == null)
			Transitor = Instantiate(Resources.Load("Prefabs/LoadTransitor")) as GameObject;
		m_scene = Transitor.GetComponentInChildren<SceneFadeInOut>();

	}

	public HUDHandler HUDControl
	{
		get{ return m_handler;}
	}

	public void StartScene()
	{
		m_scene.StartScene();
	}
	public void ShowTop(bool show)
	{
		m_handler.ShowTop(show);
	}

	public void ShowBottom(bool show)
	{
		m_handler.ShowBottom(show);
	}

}
