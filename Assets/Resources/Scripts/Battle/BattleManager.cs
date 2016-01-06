using UnityEngine;
using System.Collections;

/// <summary>
/// Manage Enemy, Manage Event
/// </summary>
public class BattleManager : MonoBehaviour 
{
	public InputGestures m_gestureHandler;
	EnemyManager	m_curBoss;
	PlayerManager	m_playerMgr;

	// Measured in seconds
	public float specialInterval = 10;
	public float stunnedInterval = 10;


	// Battle Phases
	public enum BattlePhase
	{
		START,
		ATTACK,
		SPECIAL,
		STUNNED,

		TOTAL

	};

	BattlePhase m_phase;

	static BattleManager	m_instance;

	void Awake()
	{
		if(m_instance == null)
			m_instance = this;
	}

	void Start()
	{
		m_curBoss = EnemyManager.Get();
		m_playerMgr = PlayerManager.Get();

		m_phase = BattlePhase.START;
	}

	void FixedUpdate()
	{
		Debug.Log("m_phase: " + m_phase);
		switch(m_phase)
		{
			case BattlePhase.START:
			{
				Debug.Log("AttackPhase");
				if(m_gestureHandler != null)
					m_gestureHandler.GenerateRandomGesture();
				m_phase = BattlePhase.ATTACK;
			}
			break;
			case BattlePhase.ATTACK:
			break;
			case BattlePhase.SPECIAL:
			break;

			case BattlePhase.STUNNED:
			break;
		}
	}

	public static BattleManager Get()
	{
		return m_instance;
	}
}
