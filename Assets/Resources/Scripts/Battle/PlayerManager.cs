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
	}

	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Commence Attack Event
	/// </summary>
	public void Attack()
	{
		Debug.Log("Player Attack");
		if(m_enemyMgr != null)
			m_enemyMgr.damageEnemy(m_player.atk);

		l2dInterface.PlayAnimation ();
	}	

}