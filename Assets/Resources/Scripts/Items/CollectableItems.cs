using UnityEngine;
using System.Collections;

public class CollectableItems : MonoBehaviour {

	[SerializeField]UISprite sprite;

	UIButtonMessage m_btnMsg;

	void Start()
	{
		m_btnMsg = GetComponent<UIButtonMessage>();


	}
	public void Init(ItemType type)
	{
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

	void OnCollect()
	{
		Debug.Log("OnCollect activated*****");
	}
}
