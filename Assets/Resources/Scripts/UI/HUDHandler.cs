﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GAUGE
{
	ENEMY = 0,
	PLAYER,
	MAX
};

public class HUDHandler : MonoBehaviour {
	// Label fields to be inserted
	[SerializeField]UILabel m_energyLabel;
	[SerializeField]UILabel m_goldLabel;
	[SerializeField]UILabel m_gemLabel;
	[SerializeField]UILabel m_lvlLabel;
	[SerializeField]UILabel	m_refillLabel;

	[SerializeField]GameObject m_Top;
	[SerializeField]GameObject m_Bottom;
	[SerializeField]GameObject m_Mid;
	[SerializeField]GameObject m_bottomLeft;
	[SerializeField]GameObject m_bottomRight;

	[SerializeField]UIButton	m_specialBtn;
	[SerializeField]UISprite 	m_heartSprite;
	[SerializeField]UISprite	m_graySprite;

	[SerializeField]UISlider	m_expBar;

	[SerializeField]GameObject	m_dodgeBtn;
	[SerializeField]GameObject	m_criticalBtn;
	[SerializeField]GameObject  m_sparkle;
	[SerializeField]GameObject	m_lvlbar;
	[SerializeField]GameObject 	m_pauseBtn;
	[SerializeField]GameObject	m_heartObj;
	[SerializeField]GameObject	m_drawTip;

	[SerializeField]UILabel		m_newsLabel;
	[SerializeField]GameObject 	m_newsObject;

	UIGauge[] HPBars;

	BattleManager bmgr;
	PlayerManager pmgr;

	List<ActionButton> m_actionButtonList;

	UIScrollBar cameraScrollBar;

	GameObject m_QuestTImer;
	GameObject m_Shop;

	float m_targetAmt;

	bool m_enableRankingButton;

	public float GetSpecialAmount
	{
		get{return m_targetAmt;}
	}


	#region Mono
	void Awake()
	{
		// Keeps HUD alive
		DontDestroyOnLoad(this);
		m_actionButtonList = new List<ActionButton>();

		m_expBar.sliderValue = 0.4f;
		m_sparkle.SetActive(false);

		m_enableRankingButton = false;
	}

	// Update is called once per frame
	void Update () {
		PlayerProfile.Get ().UpdateTime (Time.deltaTime);
		int stamina = PlayerProfile.Get ().stamina;
		int maxStamina = PlayerProfile.Get ().maxStamina;

		if (stamina < maxStamina) {
			int totalSeconds = (int)PlayerProfile.Get ().recoverAmount;
			int minutes = totalSeconds / 60;
			int seconds = totalSeconds % 60;

			m_refillLabel.text = string.Format ("get more {0,2}:{1:D2}", minutes, seconds);
		} else {
			m_refillLabel.text = "";
		}

		string energyDisplayText = stamina + "/" + PlayerProfile.Get ().maxStamina;
//		Debug.Log(string.Format("MaxStamina:{0}, stamina: {1}" , stamina, maxStamina));
		m_energyLabel.text = energyDisplayText;

		int gold = PlayerProfile.Get ().gold;
		gold = 30000;
		if (gold > 99999)
			m_goldLabel.text = string.Format("{0,2:F1}M", gold/1000000f);
		else if (gold > 999)
			m_goldLabel.text = string.Format("{0,2:F1}K", gold/1000f);
		else
			m_goldLabel.text = gold.ToString ();


		m_gemLabel.text = PlayerProfile.Get ().gems	.ToString ();
		m_lvlLabel.text = PlayerProfile.Get ().level.ToString ();

		int newsCount = PlayerProfile.Get ().unreadNewsCnt;
		if (newsCount > 0) {
			m_newsObject.SetActive (true);
			if (newsCount < 10)
				m_newsLabel.text = newsCount.ToString ();
			else
				m_newsLabel.text = "10+";
		} else {
			m_newsObject.SetActive (false);
		}
	}

	#endregion

	#region Public
	public void CreateBattleHUD()
	{
		// Create Hp Bars
		HPBars = new UIGauge[2];
		GameObject obj = NGUITools.AddChild(m_Top, Resources.Load("Prefabs/Battle/EnemyHpBar") as GameObject);
		obj.transform.localPosition = new Vector3(28.5f, -55.2f, 0);
		obj.transform.localScale = new Vector3(0.25f,0.25f,1f);
		HPBars[(int)GAUGE.ENEMY] = obj.GetComponent<UIGauge>();

		obj =  NGUITools.AddChild(m_bottomLeft, Resources.Load("Prefabs/Battle/PlayerHPBar") as GameObject);
		obj.transform.localPosition = new Vector3(423f, 127f, 0);
		obj.transform.localScale = new Vector3(0.8f,0.8f,1f);
		HPBars[(int)GAUGE.PLAYER] = obj.GetComponent<UIGauge>();

		if(m_bottomLeft != null)
			m_bottomLeft.SetActive(true);

		// Initialize Managers
		bmgr = BattleManager.Get();
		pmgr = PlayerManager.Get();

		// Initialize Buttons
		if(m_specialBtn != null)
		{
			m_specialBtn.isEnabled = false;
			m_specialBtn.GetComponent<UIButtonMessage>().target = bmgr.gameObject;
			m_specialBtn.GetComponent<UIButtonMessage>().functionName = "OnFinishPressed";
		}

		if(m_dodgeBtn != null)
		{
			m_dodgeBtn.GetComponent<UIButtonMessage>().target = pmgr.gameObject;
			m_dodgeBtn.GetComponent<UIButtonMessage>().functionName = "Dodge"; 
		}

		if(m_criticalBtn != null)
		{
			m_criticalBtn.GetComponent<UIButtonMessage>().target = pmgr.gameObject;
			m_criticalBtn.GetComponent<UIButtonMessage>().functionName = "CriticalDamage"; 
		}

		// Initialize Battle End Timer
		m_QuestTImer =  Instantiate( Resources.Load("Prefabs/Battle/QuestTimer")) as GameObject;
		m_QuestTImer.transform.SetParent(m_Top.transform, false);


		// Initialize lvl bar
		if(m_lvlbar != null)
			m_lvlbar.SetActive(false);

//		if(m_pauseBtn != null)
//			m_pauseBtn.SetActive(true);
		
	}

	public void RemoveBattleHUD()
	{
		// Initialize lvl bar
		if(m_lvlbar != null)
			m_lvlbar.SetActive(true);

		if(m_pauseBtn != null)
			m_pauseBtn.SetActive(false);
	}

	public void AddActionButton(string funcName, GameObject target, Skill playerskill)
	{
		GameObject obj = Instantiate(Resources.Load ("Prefabs/ActionButton")) as GameObject;
		obj.transform.SetParent(m_Mid.transform);
		obj.transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
		obj.transform.localPosition = new Vector3 (-108.0f, -8.0f - (100 * m_actionButtonList.Count), 0f);
		obj.name = playerskill.name;

		ActionButton abtn = obj.GetComponent<ActionButton>();
		abtn.Init(playerskill.name, playerskill.energyCost.ToString());
		m_actionButtonList.Add(abtn);

		UIButtonMessage button = obj.GetComponent<UIButtonMessage>();
		button.functionName = funcName;
		button.target = target;

	}

	public void ShowActionButtons(bool show)
	{
		foreach(ActionButton obj in m_actionButtonList)
		{
			obj.PlayTween(show);
		}
	}

	public void InitializeGauge(int index, float curVal, float totalVal, string name = "")
	{
		if(index < HPBars.Length && HPBars[index] != null)
		{
			HPBars[index].Init(curVal, totalVal, name);
		}
	}

	public void reduce(int index, float damage)
	{
		if(index < HPBars.Length && HPBars[index] != null)
		{
			HPBars[index].reduce(damage);
		}
	}
	public void AttachTop(ref GameObject obj)
	{
		if(obj != null)
			obj.transform.SetParent(m_Top.transform, false);
	}
	public void AttachMid(ref GameObject obj)
	{
		if(obj != null)
			obj.transform.SetParent(m_Mid.transform, false);
	}
	public void AttachBottom(ref GameObject obj)
	{
		if(obj != null)
			obj.transform.SetParent(m_Bottom.transform, false);
	}
	public void ShowTop(bool show)
	{
		m_Top.SetActive(show);
	}
	public void ShowBottom(bool show)
	{
		m_Bottom.SetActive(show);
	}

	public void ShowMid(bool show)
	{
		m_Mid.SetActive(show);
	}

	public void ShowTip(bool show)
	{
		if(m_drawTip != null)
			m_drawTip.SetActive(show);
	}

	// Only used in Battle
	public void ShowHPBars(bool show)
	{
		foreach(UIGauge gauge in HPBars)
		{
			gauge.gameObject.SetActive(show);
		}

		m_heartObj.SetActive(show);
	}

	public void ShowQuestTime(bool show)
	{
		if(m_QuestTImer != null)
			m_QuestTImer.SetActive(show);
	}

	public void SetSpecialEnable(bool enable)
	{
		Debug.Log("*********Special Enable: " + enable);
		if(m_specialBtn != null)
			m_specialBtn.isEnabled = enable;
	}
	public void SetSpecialFxGlow(bool show)
	{
		m_sparkle.SetActive(show);
	}
	public void SetSpecialGaugeAmt(float targetAmt)
	{
		m_targetAmt = targetAmt;
		if(m_heartSprite != null)
		{
			StartCoroutine(FillSprite(targetAmt));
		}
		if(m_graySprite != null)
		{
			m_graySprite.fillAmount = targetAmt;
		}

	}

	public void ShowRankingGrp(bool show){
		if (m_lvlbar != null)
			m_lvlbar.SetActive (show);
	}

	public void ShowDodgeBtn(bool show)
	{
		if(m_dodgeBtn != null)
			m_dodgeBtn.SetActive(show);

	}

	public void ShowCriticalBtn(bool show)
	{
		if(m_criticalBtn != null)
			m_criticalBtn.SetActive(show);

	}

	public void SetCameraScrollBar(UIScrollBar scrollBar)
	{
		cameraScrollBar = scrollBar; 
	}

	public void ShowScrollBar(bool show)
	{
		if (cameraScrollBar != null)
			cameraScrollBar.gameObject.SetActive (show);
	}

	public string[] GetLabelsDisplay()
	{
		string[] result = new string[4];
		result [0] = m_energyLabel.text;
		result [1] = m_goldLabel.text;
		result [2] = m_gemLabel.text;
		result [3] = m_refillLabel.text;

		return result;
	}

	IEnumerator FillSprite(float targetAmt)
	{
		float diff = targetAmt - m_heartSprite.fillAmount;
		while( diff > 0.01f )
		{
			diff = targetAmt - m_heartSprite.fillAmount;
			m_heartSprite.fillAmount = Mathf.Lerp(m_heartSprite.fillAmount, targetAmt, Time.deltaTime);
			yield return null;
		}
		m_heartSprite.fillAmount = targetAmt;
	}	

	#endregion
	
	void OnHomeClick()
	{
		QuestEvent.nextSceneID = 0;
		Service.Get<HUDService> ().ReturnToHome ();
	}

	void OnFriendClick()
	{
		Service.Get<HUDService> ().ChangeScene ("FriendScene");
	}

	void OnClothingClick()
	{
		Service.Get<HUDService>().ChangeScene("CostumeScene");
	}

	void OnNewsClick()
	{
		Service.Get<HUDService>().ChangeScene("FeedScene");
	}

	void OnGemClick()
	{
		Service.Get<PopUpService>().ShowShop(ShopType.Gems);
	}
		
	void OnEnergyClick()
	{
		Service.Get<PopUpService> ().ShowShop (ShopType.Energy);
	}

	void OnCoinClick()
	{
		Service.Get<PopUpService> ().ShowShop (ShopType.Coins);
	}
	public void EnableRanking(bool enable)
	{
		m_enableRankingButton = enable;
	}
		
	void OnRanking()
	{
		if (!m_enableRankingButton)
			return;

		GameObject obj = GameObject.Instantiate (Resources.Load ("Prefabs/Ranking/RankingDisplay")) as GameObject;
		AttachMid (ref obj);
		obj.transform.localPosition = new Vector3(0,0, -7f);
	}
}
