using UnityEngine;
using UnityExtensions;
using System.Collections;
using System.Collections.Generic;


public enum ShopType
{
	Gems = 0,
	Coins,
	Energy,
	Total
};
public class ShopHandler : MonoBehaviour {


	ShopType 	m_type;
	int			m_index = 1;
	[SerializeField]UIImageButton[] m_tabs;
	[SerializeField]UILabel m_energyLabel;
	[SerializeField]UILabel m_gemLabel;
	[SerializeField]UILabel m_goldLabel;
	[SerializeField]UILabel m_refillLabel;
	[SerializeField]UIGrid	m_grid;

	List<GameObject> m_itemContainer;
	GenericAction m_DoneCallBack;
	bool isDirty = false;

	GameObject CreateShopItem(int currencyAmt, int price)
	{
		GameObject obj = Instantiate(Resources.Load("Prefabs/Shop/ShopItem")) as GameObject;
		obj.transform.SetParent(m_grid.transform, false);

		if(currencyAmt == 0 || price == 0)
			obj.GetComponent<ShopItem>().Init("OK", "Get Free", m_index, m_type);
		else
			obj.GetComponent<ShopItem>().Init("$" + price.ToString() + ".00", "+ " + currencyAmt.ToString(), m_index, m_type);

		m_index++;

		return obj;
	}
	public void Init(ShopType type = ShopType.Gems)
	{
		m_itemContainer = new List<GameObject>();
		m_type = type;
		switch(m_type)
		{
			case ShopType.Gems:
			//Load Gems Shop Data
			//Create shop bundles
			m_itemContainer.Add(CreateShopItem(0,0));
			m_itemContainer.Add(CreateShopItem(5,100));
			m_itemContainer.Add(CreateShopItem(10,250));
			m_itemContainer.Add(CreateShopItem(50,1550));

			m_grid.repositionNow = true;

			m_tabs[0].isEnabled = false;
			m_tabs[1].isEnabled = true;
			m_tabs[2].isEnabled = true;
			break;

			case ShopType.Coins:

			m_itemContainer.Add(CreateShopItem(0,0));
			m_itemContainer.Add(CreateShopItem(5,100));
			m_itemContainer.Add(CreateShopItem(10,250));
			m_itemContainer.Add(CreateShopItem(50,1550));

			m_tabs[0].isEnabled = true;
			m_tabs[1].isEnabled = false;
			m_tabs[2].isEnabled = true;

			m_grid.repositionNow = true;
			break;

			case ShopType.Energy:

			m_itemContainer.Add(CreateShopItem(0,0));
			m_itemContainer.Add(CreateShopItem(5,100));
			m_itemContainer.Add(CreateShopItem(10,250));
			m_itemContainer.Add(CreateShopItem(50,1550));


			m_tabs[0].isEnabled = true;
			m_tabs[1].isEnabled = true;
			m_tabs[2].isEnabled = false;

			m_grid.repositionNow = true;
			break;
		}
	}

	void OnDone()
	{
		if(m_DoneCallBack != null)
			m_DoneCallBack.InvokeEvent();

		Service.Get<PopUpService>().CloseTopestPopup();
	}

	void ClearAll()
	{
		foreach(GameObject obj in m_itemContainer)
			Destroy(obj);

		m_itemContainer.Clear();
	}

	void OnTabClick(GameObject obj)
	{
		m_index = 1;

		if(obj.name.CompareTo("Gems") == 0)
		{
			ClearAll();

			m_type = ShopType.Gems;

			m_itemContainer.Add(CreateShopItem(0,0));
			m_itemContainer.Add(CreateShopItem(5,100));
			m_itemContainer.Add(CreateShopItem(10,250));
			m_itemContainer.Add(CreateShopItem(50,1550));

			m_tabs[0].isEnabled = false;
			m_tabs[1].isEnabled = true;
			m_tabs[2].isEnabled = true;
		}
		else if (obj.name.CompareTo("Coins") == 0)
		{
			ClearAll();

			m_type = ShopType.Coins;

			m_itemContainer.Add(CreateShopItem(0,0));
			m_itemContainer.Add(CreateShopItem(5,100));
			m_itemContainer.Add(CreateShopItem(10,250));
			m_itemContainer.Add(CreateShopItem(50,1550));

			m_tabs[0].isEnabled = true;
			m_tabs[1].isEnabled = false;
			m_tabs[2].isEnabled = true;
		}

		else if (obj.name.CompareTo("Energy") == 0)
		{
			ClearAll();

			m_type = ShopType.Energy;

			m_itemContainer.Add(CreateShopItem(0,0));
			m_itemContainer.Add(CreateShopItem(5,100));
			m_itemContainer.Add(CreateShopItem(10,250));
			m_itemContainer.Add(CreateShopItem(50,1550));

			m_tabs[0].isEnabled = true;
			m_tabs[1].isEnabled = true;
			m_tabs[2].isEnabled = false;
		}

		isDirty = true;
	}

	void Update()
	{
		string[] labels = Service.Get<HUDService> ().HUDControl.GetLabelsDisplay ();
		if (labels != null && labels.Length > 0) 
		{
			m_energyLabel.text = labels [0];
			m_goldLabel.text = labels [1];
			m_gemLabel.text = labels [2];
			m_refillLabel.text = labels [3];
		}
	}
	void LateUpdate()
	{
		if(isDirty)
		{
			m_grid.repositionNow = true;
			isDirty = false;
		}
	}
}
