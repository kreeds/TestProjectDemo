using UnityEngine;
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
	[SerializeField]UILabel		m_specialLabel;

	[SerializeField]UISlider	m_expBar;

	[SerializeField]GameObject	m_dodgeBtn;
	[SerializeField]GameObject  m_sparkle;

	UIGauge[] HPBars;

	BattleManager bmgr;
	PlayerManager pmgr;

	List<ActionButton> m_actionButtonList;

	UIScrollBar cameraScrollBar;

	#region Mono
	void Awake()
	{
		// Keeps HUD alive
		DontDestroyOnLoad(this);
		m_actionButtonList = new List<ActionButton>();

		m_expBar.sliderValue = 0.4f;
		m_sparkle.SetActive(false);
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
		m_energyLabel.text = energyDisplayText;

		m_goldLabel.text = PlayerProfile.Get ().gold.ToString ();
		m_gemLabel.text = PlayerProfile.Get ().gems	.ToString ();
		m_lvlLabel.text = PlayerProfile.Get ().level.ToString ();
	}

	#endregion

	#region Public
	public void CreateBattleHUD()
	{
		// Create Hp Bars
		HPBars = new UIGauge[2];
		GameObject obj = NGUITools.AddChild(m_Top, Resources.Load("Prefabs/Battle/EnemyHpBar") as GameObject);
		obj.transform.localPosition = new Vector3(22.7f, -55.2f, 0);
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

		// Initialize Battle End Timer
		obj =  Instantiate( Resources.Load("Prefabs/Battle/QuestTimer")) as GameObject;
		obj.transform.SetParent(m_Top.transform, false);

	}

	public void AddActionButton(string funcName, GameObject target)
	{
		GameObject obj = Instantiate(Resources.Load ("Prefabs/ActionButton")) as GameObject;
		obj.transform.SetParent(m_Mid.transform);
		obj.transform.localScale = new Vector3 (0.7f, 0.7f, 0.7f);
		obj.transform.localPosition = new Vector3 (-108.0f, 188.0f - (74 * m_actionButtonList.Count), 0f);

		m_actionButtonList.Add(obj.GetComponentInChildren<ActionButton>());

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

	public void InitializeGauge(int index, int curVal, int totalVal, string name = "")
	{
		if(index < HPBars.Length && HPBars[index] != null)
		{
			HPBars[index].Init(curVal, totalVal, name);
		}
	}

	public void reduce(int index, int damage)
	{
		if(index < HPBars.Length && HPBars[index] != null)
		{
			HPBars[index].reduce(damage);
		}
	}
	public void AttachMid(ref GameObject obj)
	{
		if(obj != null)
			obj.transform.SetParent(m_Mid.transform);
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

	// Only used in Battle
	public void ShowHPBars(bool show)
	{
		foreach(UIGauge gauge in HPBars)
		{
			gauge.gameObject.SetActive(show);
		}
	}

	public void SetSpecialEnable(bool enable)
	{
		Debug.Log("*********Special Enable: " + enable);
		if(m_specialBtn != null)
			m_specialBtn.isEnabled = enable;


		if(m_specialLabel != null)
			m_specialLabel.enabled = enable;
	}

	public void SetSpecialGaugeAmt(float targetAmt)
	{
		if(m_heartSprite != null)
		{
			// Target Amount is full
			if(targetAmt == 1.0f)
			{
				m_sparkle.SetActive(true);
			}
			else
			{
				m_sparkle.SetActive(false);
			}
			StartCoroutine(FillSprite(targetAmt));
		}
	}

	public void ShowDodgeBtn(bool show)
	{
		if(m_dodgeBtn != null)
			m_dodgeBtn.SetActive(show);

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
		//m_specialBtn.isEnabled = (m_heartSprite.fillAmount == 1)? true: false;

	}	

	#endregion

	void OnClothingClick()
	{
		Service.Get<HUDService>().ChangeScene("CharaScene");
	}

}
