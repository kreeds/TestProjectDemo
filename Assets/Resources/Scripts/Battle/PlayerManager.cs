using UnityEngine;
using System.Collections;

struct PlayerStats
{
	public int hp;
	public int bravebar;
	public int atk;
}

public class PlayerManager : MonoBehaviour {

	PlayerStats m_player;
	BattleManager m_battleMgr;
	EnemyManager m_enemyMgr;

	[SerializeField]UIGauge m_gauge;
	[SerializeField]LAppModelProxy l2dInterface;

	public static PlayerManager _instance;


	public static PlayerManager Get()
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
		// Initialize base value of player
		m_player = new PlayerStats();
		m_player.atk = 10;
		m_player.hp = 100;
		m_player.bravebar = 0;

		m_enemyMgr = EnemyManager.Get();
		m_battleMgr = BattleManager.Get();

		if(m_gauge != null)
		{
			m_gauge.Init(m_player.hp, m_player.hp);
		}
		l2dInterface.LoadProfile ();

		l2dInterface.PlayIdleAnim ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Commence Attack Event
	/// </summary>
	public void Attack()
	{
		Debug.Log("Attacking Enemy");
		if(m_enemyMgr != null)
			m_enemyMgr.damageEnemy(m_player.atk);

		l2dInterface.PlayAttackAnim ();
	}

	/// <summary>
	/// Damage Done to player
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void Damaged(int damage)
	{
		m_player.hp -= damage;

		if(m_gauge != null)
			m_gauge.reduce(damage);

		l2dInterface.PlayDamageAnim ();
	}


}