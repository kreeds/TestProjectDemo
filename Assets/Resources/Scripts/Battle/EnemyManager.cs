using UnityEngine;
using System.Collections;



public class EnemyManager : MonoBehaviour {

	Enemy currentEnemy;
	PlayerManager playerMgr;
	HUDHandler handle;

	public static EnemyManager _instance;
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
		handle = Service.Get<HUDService>().HUDControl;
		CreateSimpleEnemy (BattleManager.nextBattleMonster);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void CreateSimpleEnemy(int enemyType)
	{
		GameObject obj = NGUITools.AddChild(m_panel.gameObject, Resources.Load("Prefabs/SimpleEnemy") as GameObject);
		obj.transform.localPosition = new Vector3 (-719f, -221, 0);
		currentEnemy = obj.GetComponent<Enemy>();
		currentEnemy.Initialize(80, 80, 8, 3);
		SimpleEnemy sE = currentEnemy as SimpleEnemy;

		SimpleEnemy.SimpleEnemyType type = (SimpleEnemy.SimpleEnemyType)enemyType;
		string name = "Mirror Monster";
		if (type == SimpleEnemy.SimpleEnemyType.Rat)
			name = "Rat Monster";

		sE.SetType (type);
		
		if(handle != null)
		{
			handle.InitializeGauge((int)GAUGE.ENEMY, currentEnemy.Hp, currentEnemy.totalHp,  name);
		}
	}

	/// </summary>
	void CreateLive2DEnemy()
	{
		GameObject obj = NGUITools.AddChild(m_panel.gameObject, Resources.Load("Prefabs/EnemySample") as GameObject);
		obj.transform.rotation = Quaternion.Euler (270f, 0f, 0f);
		obj.transform.localScale = new Vector3 (20, 20, 20);
		currentEnemy = obj.GetComponent<Enemy>();
		currentEnemy.transform.localPosition = new Vector3(268.0f, -120.0f, 0);

		currentEnemy.Initialize(80, 80, 8, 3);
		
		if(handle != null)
		{
			handle.InitializeGauge((int)GAUGE.ENEMY, currentEnemy.Hp, currentEnemy.totalHp, "HP" + currentEnemy.name);
		}
	}

	/// <summary>
	/// Method to damage enemy
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void damageEnemy(float damage)
	{
		currentEnemy.Hp -= damage;

		if(handle != null)
			handle.reduce((int)GAUGE.ENEMY, damage);
	}

	/// <summary>
	/// Method to kill Enemy
	/// </summary>
	public void killEnemy()
	{
		if(handle != null)
			handle.reduce((int)GAUGE.ENEMY, currentEnemy.Hp);

		currentEnemy.Hp = -1;
	}


	/// <summary>
	/// Getter method to get current enemy attack value
	/// </summary>
	/// <returns>The current enemy attack.</returns>
	public float GetCurrentEnemyAttack()
	{
		return currentEnemy.attack;
	}
}
