using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class CostumeItem
{
	public int itemUnlockLvl;
	public int itemCost;

	public int itemId;

	public bool itemIsPremium;

	public bool isLocked;

	public string resourceLocation;

	public CostumeItem(int unlockLv, int id, int cost, bool premium, bool locked, string resourceloc)
	{
		itemUnlockLvl = unlockLv;
		itemCost = cost;
		itemIsPremium = premium;

		itemId = id;

		resourceLocation = resourceloc;

		isLocked = locked;
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

	[SerializeField]UISprite[]			_tabBtnBackground;
	
	[SerializeField]SceneFadeInOut fader;
	
	HUDService m_hudService;

	int currentGroupID;
	int costumeGroupID;

	// Use this for initialization
	void Start () {
		
		Service.Init();	
		m_hudService = Service.Get<HUDService>();
		m_hudService.StartScene();

		PutTestData ();

		currentGroupID = costumeGroupID = 0;

		_l2dModel.PlayIdleAnim ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PutTestData()
	{
		CostumeItemSet costumeSet = new CostumeItemSet ();
		List<CostumeItem> itemList = new List<CostumeItem> ();

		itemList.Add (new CostumeItem (0, 0, 50, false, false, "Texture/Costume/ui_cos_001"));
		itemList.Add (new CostumeItem (0, 2, 50, true, false, "Texture/Costume/ui_cos_002"));
		itemList.Add (new CostumeItem (0, 3, 50, false, false, "Texture/Costume/ui_cos_003"));
		itemList.Add (new CostumeItem (0, 1, 50, true, false, "Texture/Costume/ui_cos_004"));
		itemList.Add (new CostumeItem (10, -1, 50, false, true, "Texture/Costume/ui_cos_005"));
		itemList.Add (new CostumeItem (10, -1, 50, true, true, "Texture/Costume/ui_cos_006"));

		costumeSet.casualItems = itemList.ToArray ();

		itemList.Clear ();

		itemList.Add (new CostumeItem (0, 0, 50, false, false, "Texture/UI_cos_cloth03"));

		costumeSet.uniformItems = itemList.ToArray ();

		itemList.Clear ();

		itemList.Add (new CostumeItem (0, 100, 50, false, false, "Texture/Costume/UI_cos_hair01"));
		itemList.Add (new CostumeItem (0, 101, 50, true, false, "Texture/Costume/UI_cos_hair02"));
		itemList.Add (new CostumeItem (0, 103, 50, false, false, "Texture/Costume/UI_cos_hair03"));
		itemList.Add (new CostumeItem (0, 102, 50, true, false, "Texture/Costume/UI_cos_hair04"));
		itemList.Add (new CostumeItem (10, -1, 50, false, true, "Texture/Costume/UI_cos_hair02"));

		costumeSet.hairItems = itemList.ToArray ();

		itemList.Clear ();

		Initialize (costumeSet);

		OnCostumeTab ();
	}


	public void Initialize(CostumeItemSet itemSet)
	{
		foreach (CostumeItem costumeItem in itemSet.casualItems) {
			GameObject obj = NGUITools.AddChild(_casualGrid.gameObject, Resources.Load("Prefabs/WardrobeItem") as GameObject);
			WardrobeItem item = obj.GetComponent<WardrobeItem>();

			WardrobeItem.IconType iconType;

//			if (costumeItem.itemUnlockLvl > 0)
//				iconType = WardrobeItem.IconType.LOCKED;
			if (costumeItem.itemIsPremium)
				iconType = WardrobeItem.IconType.PREMIUM;
			else
				iconType = WardrobeItem.IconType.GOLD;

			item.Initialize(iconType, costumeItem.itemCost, costumeItem.resourceLocation, costumeItem.isLocked, costumeItem.itemId, gameObject);
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
			
			item.Initialize(iconType, costumeItem.itemCost, costumeItem.resourceLocation, costumeItem.isLocked, costumeItem.itemId, gameObject);
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
			
			item.Initialize(iconType, costumeItem.itemCost, costumeItem.resourceLocation, costumeItem.isLocked, costumeItem.itemId, gameObject);
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

		foreach (UISprite sprite in _tabBtnBackground) {
			sprite.depth = 4;
		}
		_tabBtnBackground [0].depth = 6;
	}

	void OnMakeupTab()
	{
		_hairGrid.gameObject.SetActive (true);
		_costumeGroup.SetActive (false);
		_appearanceGroup.SetActive (true);

		
		foreach (UISprite sprite in _tabBtnBackground) {
			sprite.depth = 4;
		}
		_tabBtnBackground [2].depth = 6;
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

	void OnHomeClick()
	{
		fader.ChangeScene("MainMenu");
	}
}
