using UnityEngine;
using System.Collections;

public class CollectableItems : MonoBehaviour {

	[SerializeField]UISprite sprite;
	[SerializeField]UILabel label;
	[SerializeField]TweenAlpha t_alpha;
	[SerializeField]float 	lifeTime = 5.0f;
	[SerializeField]float	destroyTime = 1.0f;
	[SerializeField]GameObject m_sparkle;

	UIButtonMessage m_btnMsg;
	ItemType 		m_type;
	int				m_amt;


	public float LifeTime
	{
		get{ return lifeTime;}
	}
	void Start()
	{
		m_btnMsg = GetComponent<UIButtonMessage>();

		AutoCollect();
	}
	public void Init(ItemType type, int amt = 1)
	{

		if(sprite == null)
			return;

		m_type = type;
		m_amt = amt;

		switch(type)
		{
			case ItemType.ENERGY:
				sprite.spriteName = "icon00";
				break;
			case ItemType.GOLD:
				sprite.spriteName = "icon02";
				break;
		case ItemType.STAR:
			gameObject.layer = 12;
			label.gameObject.layer = 12;
			sprite.spriteName = "Quest_Bar_XP";
				break;
			case ItemType.XP:
				sprite.spriteName = "";
				break;
		}
	}

	// TODO: Need to implement Add currency
	void AutoCollect()
	{
		switch(m_type)
		{
			case ItemType.ENERGY:
				break;
			case ItemType.GOLD:
				label.color = Color.yellow;

				break;
			case ItemType.STAR:
				break;
			case ItemType.XP:
				break;
		}

		StartCoroutine(Utility.DelayInSeconds(lifeTime, 
						(res) => 
						{ 
							if(label != null)
							{
								label.gameObject.SetActive(true);
								label.text = "+" + m_amt.ToString();
							}
				
							if(t_alpha != null)
							{
								t_alpha.Play(true);
							}

							if (m_type == ItemType.STAR){
								GameObject obj = GameObject.Find ("EventManager");
								obj.SendMessage("OnStarCollected");
							}
//							else if(m_type == ItemType.GOLD)
//							{
//								m_sparkle.SetActive(true);
//							}
						} 
						));
	}

	void OnClick()
	{

		// AddGold();
		switch(m_type)
		{
			case ItemType.ENERGY:
				label.color = Color.blue;
				break;
			case ItemType.GOLD:
				label.color = Color.yellow;
				//m_sparkle.SetActive(true);
				break;
		case ItemType.STAR:
			GameObject obj = GameObject.Find ("EventManager");
			obj.SendMessage("OnStarCollected");
			collider.enabled = false;
				break;
			case ItemType.XP:
				break;
		}

		if(label != null)
		{
			label.gameObject.SetActive(true);
			label.text = "+" + m_amt.ToString();
		}

		if(t_alpha != null)
		{
			t_alpha.Play(true);
		}



	}

	void Destroy()
	{
		StartCoroutine(Utility.DelayInSeconds(1.0f, (res)=>{ Destroy(gameObject); } ));
	}

}
