using UnityEngine;
using System.Collections;

public class CollectableItems : MonoBehaviour {

	[SerializeField]UISprite sprite;
	[SerializeField]UILabel label;

	UIButtonMessage m_btnMsg;
	ItemType 		m_type;
	int				m_amt;

	const float 	DEATHTIME = 5.0f;

	void Start()
	{
		m_btnMsg = GetComponent<UIButtonMessage>();
		if(label != null)
			label.enabled = false;

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
				sprite.spriteName = "";
				break;
			case ItemType.GOLD:
				sprite.spriteName = "icon02";
				break;
			case ItemType.STAR:
				sprite.spriteName = "";
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

		StartCoroutine(Utility.DelayInSeconds(DEATHTIME, 
						(res) => 
						{ 
							if(label != null)
							{
								label.enabled = true;
								label.text = "+" + m_amt.ToString();
							}
							// AddGold();
							StartCoroutine(Utility.DelayInSeconds(1.0f, 
											(res1) => { 
											Destroy(gameObject); } ));
						} 
						));
	}

	void OnCollect()
	{

		// AddGold();
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

		if(label != null)
		{
			label.enabled = true;
			label.text = "+" + m_amt.ToString();
		}

		if(sprite != null)
		{
			sprite.enabled = false;
		}

		StartCoroutine(Utility.DelayInSeconds(1.0f, 
												(res) => { 
												Destroy(gameObject); } ));
	}
}
