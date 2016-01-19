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
	public float m_gestureInv = 5;
	public float m_interval = 3;
	public float m_specialInv = 3;
	public float m_stunInv = 3;


	bool m_gestureStart;


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

		m_gestureStart = false;

		m_phase = BattlePhase.ATTACK;
	}

	IEnumerator CommenceAttack()
	{			
		yield return new WaitForSeconds(m_interval);
		if (m_gestureHandler != null) {
			m_gestureHandler.GenerateRandomGesture ();
			m_playerMgr.Idle ();
			m_curBoss.Idle ();
		}
		yield return new WaitForSeconds(m_gestureInv);
		//Destroy Gesture
		if(m_gestureHandler != null)
			m_gestureHandler.DestroyGesture();
		yield return new WaitForSeconds(m_interval);
			m_gestureStart = false;

	}

	void FixedUpdate()
	{
		switch(m_phase)
		{
			case BattlePhase.START:
			{
				Debug.Log("Start Phase");

			}
			break;
			case BattlePhase.ATTACK:
			{
				if(m_gestureStart == false)
				{
					m_gestureStart = true;
					StartCoroutine(CommenceAttack());
				}
				m_phase = BattlePhase.ATTACK;
			}
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
