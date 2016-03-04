using UnityEngine;
using System.Collections;

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

	[SerializeField]GameObject m_Top;
	[SerializeField]GameObject m_Bottom;
	[SerializeField]GameObject m_Mid;
	[SerializeField]GameObject m_BattleBottom;

	[SerializeField]UIButton	m_specialBtn;
	[SerializeField]UISprite 	m_heartSprite;
	[SerializeField]UILabel		m_specialLabel;

	[SerializeField]GameObject	m_dodgeBtn;

	UIGauge[] HPBars;

	#region Mono
	void Awake()
	{
		// Keeps HUD alive
		DontDestroyOnLoad(this);
	}
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	 
	}

	#endregion

	#region Button
	void OnDodgeClick()
	{
		if(m_dodgeBtn != null)
			m_dodgeBtn.SetActive(false);

		Debug.Log("Show Dodge Button");
	}
	#endregion

	#region Public
	public void CreateBattleHUD()
	{
		HPBars = new UIGauge[2];
		GameObject obj = NGUITools.AddChild(gameObject, Resources.Load("Prefabs/Battle/HpBar") as GameObject);
		obj.transform.localPosition = new Vector3(-156.0f, 339.0f, 0);
		obj.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
		HPBars[(int)GAUGE.ENEMY] = obj.GetComponent<UIGauge>();

		obj =  NGUITools.AddChild(gameObject, Resources.Load("Prefabs/Battle/HpBar") as GameObject);
		obj.transform.localPosition = new Vector3(-294.0f, -355.0f, 0);
		obj.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
		HPBars[(int)GAUGE.PLAYER] = obj.GetComponent<UIGauge>();

		if(m_BattleBottom != null)
			m_BattleBottom.SetActive(true);

		if(m_specialBtn != null)
		{
			m_specialBtn.isEnabled = false;
			m_specialBtn.GetComponent<UIButtonMessage>().target = GameObject.Find("BattleManager");
			m_specialBtn.GetComponent<UIButtonMessage>().functionName = "OnFinishPressed";
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
			StartCoroutine(FillSprite(targetAmt));
		}
	}

	public void ShowDodgeBtn(bool show)
	{
		if(m_dodgeBtn != null)
			m_dodgeBtn.SetActive(show);

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


}
