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

	[SerializeField]GameObject			_wardrobeGroup;
	[SerializeField]GameObject			_armoryGroup;

	[SerializeField]LAppModelProxy		_l2dModel;

	[SerializeField]UISprite[]			_tabBtnBackground;
	[SerializeField]UISprite[]			_tabBtnSprite;
	
	[SerializeField]SceneFadeInOut fader;
	
	HUDService m_hudService;
	SoundService m_soundService;
	AudioClip	costumeBGM;
	AudioClip	armoryBGM;

	int currentGroupID;
	int costumeGroupID;

	static Color activeColor = new Color(218 / 255f, 207 / 255f, 191 / 255f, 0.6f);
	static Color inactiveColor = new Color(112/255f,93/255f,76/255f, 0.6f); 

	// Use this for initialization
	void Start () {
		
		Service.Init();	
		m_hudService = Service.Get<HUDService>();
		m_hudService.StartScene();

		m_hudService.HUDControl.ShowRankingGrp (false);

		costumeBGM = Resources.Load("Music/costumemusic") as AudioClip;
		armoryBGM = Resources.Load ("Music/weapon_bgm") as AudioClip;

		m_soundService = Service.Get<SoundService>();
		m_soundService.PlayMusic(costumeBGM, true);



		PutTestData ();

		currentGroupID = costumeGroupID = 0;

		_l2dModel.LoadProfile ();
		_l2dModel.GetModel ().StopBasicMotion (true);
		_l2dModel.PlayIdleAnim ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PutTestData()
	{
		CostumeItemSet costumeSet = new CostumeItemSet ();
		List<CostumeItem> itemList = new List<CostumeItem> ();

		itemList.Add (new CostumeItem (0, 1, 50, false, false, "Texture/Costume/ui_cos_001"));
		itemList.Add (new CostumeItem (0, 2, 50, true, false, "Texture/Costume/ui_cos_002"));
		itemList.Add (new CostumeItem (0, 3, 50, false, false, "Texture/Costume/ui_cos_003"));
		itemList.Add (new CostumeItem (0, 4, 50, true, false, "Texture/Costume/ui_cos_004"));
//		itemList.Add (new CostumeItem (0, 3, 50, true, false, "Texture/Costume/ui_cos_005"));
		itemList.Add (new CostumeItem (10, -1, 50, true, true, "Texture/Costume/ui_cos_002"));

		costumeSet.casualItems = itemList.ToArray ();

		itemList.Clear ();

		itemList.Add (new CostumeItem (0, 0, 50, false, false, "Texture/UI_cos_cloth03"));

		costumeSet.uniformItems = itemList.ToArray ();

		itemList.Clear ();

		itemList.Add (new CostumeItem (0, 101, 50, false, false, "Texture/Costume/UI_cos_hair01"));
		itemList.Add (new CostumeItem (0, 102, 50, true, false, "Texture/Costume/UI_cos_hair02"));
		itemList.Add (new CostumeItem (0, 103, 50, false, false, "Texture/Costume/UI_cos_hair03"));
		itemList.Add (new CostumeItem (0, 104, 50, true, false, "Texture/Costume/UI_cos_hair04"));
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
			sprite.color = inactiveColor;
		}
		int i = 1;
		foreach (UISprite sprite in _tabBtnSprite) {
			sprite.spriteName = "UI_cos_tab_0" + i++ + "a";
		}
		_tabBtnSprite [0].spriteName = "UI_cos_tab_01";
		_tabBtnBackground [0].depth = 6;
		_tabBtnBackground [0].color = activeColor;
	}

	void OnMakeupTab()
	{
		_hairGrid.gameObject.SetActive (true);
		_costumeGroup.SetActive (false);
		_appearanceGroup.SetActive (true);

		
		foreach (UISprite sprite in _tabBtnBackground) {
			sprite.depth = 4;
			sprite.color = inactiveColor;
		}
		int i = 1;
		foreach (UISprite sprite in _tabBtnSprite) {
			sprite.spriteName = "UI_cos_tab_0" + i++ + "a";
		}
		_tabBtnSprite [2].spriteName = "UI_cos_tab_03";
		_tabBtnBackground [2].depth = 6;
		_tabBtnBackground [2].color = activeColor;
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

	void OnSwordClick()
	{
		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/ArmoryPopup") as GameObject);

		obj.transform.localPosition = new Vector3 (583.1f, 0, -23f);
		ArmoryPopup popup = obj.GetComponent<ArmoryPopup> ();
		popup.Initialize (2, gameObject);
	}

	void OnSRankBroachClick()
	{
		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/ArmoryPopup") as GameObject);
		obj.transform.localPosition = new Vector3 (583.1f, 0, -23f);
		ArmoryPopup popup = obj.GetComponent<ArmoryPopup> ();
		popup.Initialize (1, gameObject);
	}

	void OnCRankBroachClick()
	{
		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/ArmoryPopup") as GameObject);
		obj.transform.localPosition = new Vector3 (583.1f, 0, -23f);
		ArmoryPopup popup = obj.GetComponent<ArmoryPopup> ();
		popup.Initialize (0, gameObject);
	}

	void OnHomeClick()
	{
		_l2dModel.SaveProfile ();
		Service.Get<HUDService> ().ReturnToHome ();
	}

	void OnEnterArmory()
	{
//		_armoryGroup.SetActive (true);
//		_wardrobeGroup.SetActive (false);
		m_soundService = Service.Get<SoundService>();
		m_soundService.StopMusic (costumeBGM);
		m_soundService.PlayMusic (armoryBGM, true);

		
		m_hudService.HUDControl.ShowTop (false);

		TweenPosition tween = gameObject.GetComponent<TweenPosition> ();
		tween.Play (true);
	}

	void OnExitArmory()
	{
//		_armoryGroup.SetActive (false);
//		_wardrobeGroup.SetActive (true);
		
		m_hudService.HUDControl.ShowTop (true);

		m_soundService = Service.Get<SoundService>();
		m_soundService.StopMusic (armoryBGM);
		m_soundService.PlayMusic(costumeBGM, true);

		TweenPosition tween = gameObject.GetComponent<TweenPosition> ();
		tween.Play (false);
	}
}
