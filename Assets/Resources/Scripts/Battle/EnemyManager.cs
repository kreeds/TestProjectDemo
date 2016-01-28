using UnityEngine;
using System.Collections;



public class EnemyManager : MonoBehaviour {

	Enemy currentEnemy;
	PlayerManager playerMgr;
	public static EnemyManager _instance;
	[SerializeField]UIGauge m_gauge;
	[SerializeField]UIPanel	m_panel;

	public static EnemyManager Get()
	{
		return _instance;
	}


	void Awake()
	{	
		if(_instance == null)
			_instance = this;

		playerMgr = PlayerManager.Get();
	}

	// Use this for initialization
	void Start () 
	{
//		GameObject obj = NGUITools.AddChild(m_panel.gameObject, Resources.Load("Prefabs/EnemySample") as GameObject);
//		obj.transform.rotation = Quaternion.Euler (270f, 0f, 0f);
//		obj.transform.localScale = new Vector3 (20, 20, 20);
//		currentEnemy = obj.GetComponent<Enemy>();
//		currentEnemy.transform.localPosition = new Vector3(268.0f, -120.0f, 0);

//		GameObject obj = NGUITools.AddChild(m_panel.gameObject, Resources.Load("Prefabs/SimpleEnemy") as GameObject);
//		obj.transform.localPosition = new Vector3 (300f, 0, 0);
//		currentEnemy = obj.GetComponent<Enemy>();
//		currentEnemy.Initialize(80, 80, 8, 3);
//	
//		if(m_gauge != null)
//		{
//			m_gauge.Init(currentEnemy.Hp, currentEnemy.totalHp);
//		}
		CreateSimpleEnemy ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void CreateSimpleEnemy()
	{
		GameObject obj = NGUITools.AddChild(m_panel.gameObject, Resources.Load("Prefabs/SimpleEnemy") as GameObject);
		obj.transform.localPosition = new Vector3 (300f, -54f, 0);
		currentEnemy = obj.GetComponent<Enemy>();
		currentEnemy.Initialize(80, 80, 8, 3);
		
		if(m_gauge != null)
		{
			m_gauge.Init(currentEnemy.Hp, currentEnemy.totalHp);
		}
	}

	void CreateLive2DEnemy()
	{
		GameObject obj = NGUITools.AddChild(m_panel.gameObject, Resources.Load("Prefabs/EnemySample") as GameObject);
		obj.transform.rotation = Quaternion.Euler (270f, 0f, 0f);
		obj.transform.localScale = new Vector3 (20, 20, 20);
		currentEnemy = obj.GetComponent<Enemy>();
		currentEnemy.transform.localPosition = new Vector3(268.0f, -120.0f, 0);

		currentEnemy.Initialize(80, 80, 8, 3);
		
		if(m_gauge != null)
		{
			m_gauge.Init(currentEnemy.Hp, currentEnemy.totalHp);
		}
	}
	/// <summary>
	/// Method to damage enemy
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void damageEnemy(int damage)
	{
		currentEnemy.Hp -= damage;

		if(m_gauge != null)
			m_gauge.reduce(damage);
	}
}
