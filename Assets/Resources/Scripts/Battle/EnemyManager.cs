using UnityEngine;
using System.Collections;



public class EnemyManager : MonoBehaviour {

	Enemy currentEnemy;
	public static EnemyManager _instance;
	[SerializeField]UIGauge m_gauge;
	
	LAppModelProxy l2dInterface;

	public enum EnemyState
	{
		IDLE,
		ATTACK,
		TOTAL
	};

	EnemyState m_enemyState;

	public static EnemyManager Get()
	{
		return _instance;
	}

	void LoadEnemyData(int questID)
	{
		
	}

	void Awake()
	{	
		if(_instance == null)
			_instance = this;
	}

	// Use this for initialization
	void Initialize () 
	{

		if(_instance == null)
			_instance = this;

		GameObject obj = Resources.Load("Prefabs/EnemySample") as GameObject;
		currentEnemy = obj.GetComponent<Enemy>();
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

}
