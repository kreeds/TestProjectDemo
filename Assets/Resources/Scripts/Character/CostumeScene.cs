using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class CostumeItem
{
	public int itemUnlockLvl;
	public int itemCost;

	public int itemId;

	public bool itemIsPremium;

	public string resourceLocation;

	public CostumeItem(int unlockLv, int id, int cost, bool premium, string resourceloc)
	{
		itemUnlockLvl = unlockLv;
		itemCost = cost;
		itemIsPremium = premium;

		itemId = id;

		resourceLocation = resourceloc;
	}
}

public class CostumeItemSet
{
	public CostumeItem[] casualItems;
	public CostumeItem[] uniformItems;

	public CostumeItem[] hairItems;
}

public class CostumeScene : MonoBehaviour {

	[SerializeField]UIGrid				_casualGrid;
	[SerializeField]UIGrid				_uniformGrid;

	[SerializeField]UIGrid				_hairGrid;

	[SerializeField]GameObject			_costumeGroup;
	[SerializeField]GameObject			_appearanceGroup;

	[SerializeField]LAppModelProxy		_l2dModel;

	int currentGroupID;
	int costumeGroupID;

	// Use this for initialization
	void Start () {
	
		PutTestData ();

		currentGroupID = costumeGroupID = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PutTestData()
	{
		CostumeItemSet costumeSet = new CostumeItemSet ();
		List<CostumeItem> itemList = new List<CostumeItem> ();

		itemList.Add (new CostumeItem (0, 0, 50, false, "Texture/UI_cos_cloth00"));
		itemList.Add (new CostumeItem (0, 2, 50, true, "Texture/UI_cos_cloth01"));
		itemList.Add (new CostumeItem (0, 3, 50, false, "Texture/UI_cos_cloth02"));
		itemList.Add (new CostumeItem (0, 1, 50, true, "Texture/UI_cos_cloth04"));
		itemList.Add (new CostumeItem (10, -1, 50, false, "Texture/UI_cos_cloth07"));
		itemList.Add (new CostumeItem (10, -1, 50, true, "Texture/UI_cos_cloth08"));

		costumeSet.casualItems = itemList.ToArray ();

		itemList.Clear ();

		itemList.Add (new CostumeItem (0, 0, 50, false, "Texture/UI_cos_cloth03"));

		costumeSet.uniformItems = itemList.ToArray ();

		itemList.Clear ();

		itemList.Add (new CostumeItem (0, 100, 50, false, "Texture/UI_cos_hair01"));
		itemList.Add (new CostumeItem (0, 101, 50, true, "Texture/UI_cos_hair02"));
		itemList.Add (new CostumeItem (0, 103, 50, false, "Texture/UI_cos_hair03"));
		itemList.Add (new CostumeItem (0, 102, 50, true, "Texture/UI_cos_hair04"));
		itemList.Add (new CostumeItem (10, -1, 50, false, "Texture/UI_cos_hair02"));

		costumeSet.hairItems = itemList.ToArray ();

		itemList.Clear ();

		Initialize (costumeSet);
	}


	public void Initialize(CostumeItemSet itemSet)
	{
		foreach (CostumeItem costumeItem in itemSet.casualItems) {
			GameObject obj = NGUITools.AddChild(_casualGrid.gameObject, Resources.Load("Prefabs/WardrobeItem") as GameObject);
			WardrobeItem item = obj.GetComponent<WardrobeItem>();

			WardrobeItem.IconType iconType;

			if (costumeItem.itemUnlockLvl > 0)
				iconType = WardrobeItem.IconType.LOCKED;
			else if (costumeItem.itemIsPremium)
				iconType = WardrobeItem.IconType.PREMIUM;
			else
				iconType = WardrobeItem.IconType.GOLD;

			item.Initialize(iconType, costumeItem.itemCost, costumeItem.resourceLocation, costumeItem.itemId, gameObject);
		}

		_casualGrid.Reposition ();

		foreach (CostumeItem costumeItem in itemSet.uniformItems) {
			GameObject obj = NGUITools.AddChild(_uniformGrid.gameObject, Resources.Load("Prefabs/WardrobeItem") as GameObject);
			WardrobeItem item = obj.GetComponent<WardrobeItem>();
			
			WardrobeItem.IconType iconType;
			
			if (costumeItem.itemUnlockLvl > 0)
				iconType = WardrobeItem.IconType.LOCKED;
			else if (costumeItem.itemIsPremium)
				iconType = WardrobeItem.IconType.PREMIUM;
			else
				iconType = WardrobeItem.IconType.GOLD;
			
			item.Initialize(iconType, costumeItem.itemCost, costumeItem.resourceLocation, costumeItem.itemId, gameObject);
		}
		
		_uniformGrid.Reposition ();
		
		foreach (CostumeItem costumeItem in itemSet.hairItems) {
			GameObject obj = NGUITools.AddChild(_hairGrid.gameObject, Resources.Load("Prefabs/WardrobeItem") as GameObject);
			WardrobeItem item = obj.GetComponent<WardrobeItem>();
			
			WardrobeItem.IconType iconType;
			
			if (costumeItem.itemUnlockLvl > 0)
				iconType = WardrobeItem.IconType.LOCKED;
			else if (costumeItem.itemIsPremium)
				iconType = WardrobeItem.IconType.PREMIUM;
			else
				iconType = WardrobeItem.IconType.GOLD;
			
			item.Initialize(iconType, costumeItem.itemCost, costumeItem.resourceLocation, costumeItem.itemId, gameObject);
		}
		
		_hairGrid.Reposition ();
	}

	void OnCasual()
	{
		if (costumeGroupID != 0) {
			costumeGroupID = 0;
			_casualGrid.gameObject.SetActive(true);

			_uniformGrid.gameObject.SetActive(false);
		}
		_costumeGroup.SetActive (true);
		_appearanceGroup.SetActive (false);
	}
	
	void OnUniform()
	{
		if (costumeGroupID != 1) {
			costumeGroupID = 1;
			_casualGrid.gameObject.SetActive(false);
			
			_uniformGrid.gameObject.SetActive(true);
		}
		_costumeGroup.SetActive (true);
		_appearanceGroup.SetActive (false);
	}

	
	void OnCostumeTab()
	{
		if (costumeGroupID != 1) {
			costumeGroupID = 1;
			_casualGrid.gameObject.SetActive(true);
			
			_uniformGrid.gameObject.SetActive(false);
		}
		_costumeGroup.SetActive (true);
		_appearanceGroup.SetActive (false);
	}

	void OnMakeupTab()
	{
		_hairGrid.gameObject.SetActive (true);
		_costumeGroup.SetActive (false);
		_appearanceGroup.SetActive (true);
	}

	void OnCostumeItem(int itemID){
		if (itemID < 0)
			return;

		if (itemID < 100) {
			_l2dModel.SetClothes (itemID);
		} else {
			_l2dModel.SetHair (itemID - 100);
		}

	}
}
