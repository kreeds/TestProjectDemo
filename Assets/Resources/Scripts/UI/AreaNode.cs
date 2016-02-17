using UnityEngine;
using System.Collections;

public class AreaNode: MonoBehaviour {

	public int 			m_areaid;
	public bool 		m_lock;
	public string 		m_name;
	public Vector2 		m_pos;

	UITexture			m_texture;

	public void Init(int aid, bool alock, string name, Vector3 position)
	{
		m_areaid = aid;
		m_lock = alock;
		m_name = name;
		m_pos = position;

		gameObject.name = name;

		Debug.Log("m_pos.x : " + m_pos.x + " m_pos.y: " + m_pos.y);
		transform.localPosition = new Vector3(m_pos.x, m_pos.y, -10);
		transform.localScale = new Vector3(50.0f,50.0f,1.0f);

		m_texture = GetComponent<UITexture>();
		m_texture.mainTexture = (m_lock)? 
									Resources.Load("Texture/icon07") as Texture :
									Resources.Load("Texture/icon08") as Texture ;
									
		
	}

	void Awake()
	{
		m_texture = gameObject.GetComponent<UITexture>();
	}
	void OnClick()
	{
		Debug.Log("OnClicked");
		// Scene Changing
	}
}
