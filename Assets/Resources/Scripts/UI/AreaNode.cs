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

	public void Init(int aid, bool alock, bool dialog, string name, Vector3 position, bool isSpoken = true, bool isMajor = true)
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
			if (alock){
				m_sprite.spriteName = "icon07";
			}
			else if (dialog){
				if (isSpoken){
					if (isMajor)
						m_sprite.spriteName = "icon06";
					else
						m_sprite.spriteName = "icon06_endstate";

				}
				else{
					m_sprite.spriteName = "icon06_alt";
				}
			}
			else{
				m_sprite.spriteName = "icon08";
			}
			if (dialog){
				TweenPosition tweenPos = m_sprite.gameObject.AddComponent<TweenPosition> ();
				tweenPos.from = new Vector3(0, 20, 0);
				tweenPos.to = new Vector3(0, -20, 0);
				tweenPos.style = UITweener.Style.PingPong;
			}
			m_sprite.MakePixelPerfect ();
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

	public void SetToReadBubble()
	{
		m_sprite.spriteName = "icon06_endstate";
	}
}
