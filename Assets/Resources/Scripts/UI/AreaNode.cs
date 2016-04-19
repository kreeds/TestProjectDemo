using UnityEngine;
using System.Collections;

public class AreaNode: MonoBehaviour {

	public int 			m_areaid;
	public bool 		m_lock;
	public string 		m_name;
	public Vector2 		m_pos;

	public string 		callback;
	public GameObject	receiver;

//	UITexture			m_texture;
	[SerializeField]UISprite 		m_sprite;

	public void Init(int aid, bool alock, bool dialog, string name, Vector3 position)
	{
		m_areaid = aid;
		m_lock = alock;
		m_name = name;
		m_pos = position;

		gameObject.name = name;

		Debug.Log("m_pos.x : " + m_pos.x + " m_pos.y: " + m_pos.y);
		transform.localPosition = new Vector3(m_pos.x, m_pos.y, -10);
		transform.localScale = new Vector3(0.5f,0.5f,1.0f);

//		m_texture = GetComponent<UITexture>();
//		m_texture.mainTexture = (m_lock)? 
//									Resources.Load("Texture/icon07") as Texture :
//									Resources.Load("Texture/icon08") as Texture ;

		if (m_sprite != null) {
			m_sprite.spriteName = m_lock? "icon07" : dialog? "icon06": "icon08";
		}							
		
	}

	void Awake()
	{
//		m_texture = gameObject.GetComponent<UITexture>();
	}
	void OnClick()
	{
		Debug.Log("OnClicked");

		receiver.SendMessage (callback, m_areaid);
		// Scene Changing
	}
}
