using UnityEngine;
using System.Collections;

public class DFEnemy
{
	public int hp;
	public int totalHp;
	public int attack;
	public int attkInterval;
}


public class EnemyManager : MonoBehaviour {

	DFEnemy	m_boss;
	public static EnemyManager _instance;


	[SerializeField]UIGauge m_gauge;

	public static EnemyManager Get()
	{
		return _instance;
	}

	void Awake()
	{	
		if(_instance == null)
			_instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		m_boss = new DFEnemy();
		m_boss.hp = 100;
		m_boss.totalHp = 100;
		m_boss.attack = 10;
		m_boss.attkInterval = 2;

		if(_instance == null)
			_instance = this;

		if(m_gauge != null)
		{
			m_gauge.Init(m_boss.hp, m_boss.totalHp);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void damageEnemy(int damage)
	{
		m_boss.hp -= damage;
		if(m_gauge != null)
			m_gauge.reduce(damage);
	}
}
