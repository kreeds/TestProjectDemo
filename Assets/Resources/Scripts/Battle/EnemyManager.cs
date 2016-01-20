using UnityEngine;
using System.Collections;



public class EnemyManager : MonoBehaviour {

	Enemy currentEnemy;
	PlayerManager playerMgr;
	public static EnemyManager _instance;
	[SerializeField]UIGauge m_gauge;
	[SerializeField]UIPanel	m_panel;
	
	LAppModelProxy l2dInterface;

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
//		GameObject obj = Instantiate(Resources.Load("Prefabs/EnemySample")) as GameObject;
		GameObject obj = NGUITools.AddChild (m_panel.gameObject, Resources.Load ("Prefabs/EnemySample") as GameObject);
		obj.transform.localScale = new Vector3(15, 15, 15);
		obj.transform.rotation = Quaternion.Euler (new Vector3 (270, 0, 0));
		currentEnemy = obj.GetComponent<Enemy>();
		currentEnemy.transform.position = new Vector3(3.5f, -2.48f, currentEnemy.transform.position.z);
		currentEnemy.Initialize(100, 100, 10, 2);

		l2dInterface = obj.GetComponent<LAppModelProxy>();

	
		if(m_gauge != null)
		{
			m_gauge.Init(currentEnemy.Hp, currentEnemy.totalHp);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}


	public void attack()
	{
		currentEnemy.EnemyNextState = Enemy.STATE.ATTACK;
		if(playerMgr != null)
			playerMgr.Damaged(currentEnemy.attack);
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

		l2dInterface.PlayDamageAnim ();
	}
	
	public void Idle(){
		l2dInterface.PlayIdleAnim ();
	}
}
