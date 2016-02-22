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

	public UIGauge[] HPBars;

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

	#region Public
	public void CreateBattleHUD()
	{
		HPBars = new UIGauge[2];
		GameObject obj = NGUITools.AddChild(gameObject, Instantiate(Resources.Load("Prefabs/Battle/HpBar")) as GameObject);
		HPBars[(int)GAUGE.ENEMY] = obj.GetComponent<UIGauge>();
		obj = Instantiate(Resources.Load("Prefabs/Battle/HpBar")) as GameObject;
		HPBars[(int)GAUGE.PLAYER] = obj.GetComponent<UIGauge>();
	}

	public void ShowTop(bool show)
	{
		m_Top.SetActive(show);
	}

	public void ShowBottom(bool show)
	{
		m_Bottom.SetActive(show);
	}
	#endregion


}
