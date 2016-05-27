using UnityEngine;
using System.Collections;

public class ShopItem : MonoBehaviour {

	[SerializeField]UISprite 	m_image;
	[SerializeField]UISprite 	m_effect;
	[SerializeField]UISprite 	m_buttonBg;
	[SerializeField]UILabel 	m_buttonLabel;
	[SerializeField]UILabel		m_frameLabel;

	ShopType m_type;
	int m_index = 1;

	public void Init(string buttonLabel, string frameLabel, int index, ShopType type = ShopType.Gems)
	{
		m_type = type;
		m_index = index;
		m_frameLabel.text = frameLabel;
		m_buttonLabel.text = buttonLabel;

		switch(m_type)
		{
			case ShopType.Gems:

			if(m_effect != null)
				m_effect.spriteName = "UI_Shop_Gem_FX";

			if(m_image != null)
				m_image.spriteName = "UI_Shop_Gem_" + index.ToString();

			if(m_buttonBg != null)
				m_buttonBg.spriteName = "UI_Shop_Gem_Button";


			break;
			case ShopType.Coins:

			if(m_effect != null)
				m_effect.spriteName = "UI_Shop_Coin_FX";

			if(m_image != null)
				m_image.spriteName = "UI_Shop_Coin_" + index.ToString();

			if(m_buttonBg != null)
				m_buttonBg.spriteName = "UI_Shop_Coin_Button";

			break;
			case ShopType.Energy:

			if(m_effect != null)
				m_effect.spriteName = "UI_Shop_Energy_FX";

			if(m_image != null)
				m_image.spriteName = "UI_Shop_Energy_" + index.ToString();

			if(m_buttonBg != null)
				m_buttonBg.spriteName = "UI_Shop_Energy_Button";

			break;
		}

		if (m_image != null) 
		{
			m_image.MakePixelPerfect ();
			m_image.transform.localScale = new Vector3 (m_image.transform.localScale.x / 2, m_image.transform.localScale.y / 2, 1);
		}
	}

	void OnClickButton()
	{
		
	}
}
