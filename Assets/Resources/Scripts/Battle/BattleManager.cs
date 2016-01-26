using UnityEngine;
using System.Collections;

/// <summary>
/// Manage Enemy, Manage Event
/// </summary>
public class BattleManager : MonoBehaviour 
{
	public delegate void GestureMethod();
	GestureMethod GestureGenerateMethod;

	public InputGestures m_gestureHandler;
	public GenerateGesture m_gestureGenerator;
	EnemyManager	m_enemyMgr;
	PlayerManager	m_playerMgr;


	public int m_fullgaugeCount = 5;
	int gaugeCount = 0;

	// Measured in seconds
	public float m_gestureInv = 3;
	public float m_interval = 5;
	public float m_specialInv = 3;
	public float m_stunInv = 3;

	IEnumerator m_coroutine;

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
	public BattlePhase getBattlePhase
	{
		get{return m_phase;}
	}

	// Gesture State
	public enum GestureState
	{
		START,
		SHOWING,
		END,
		TOTAL
	};

	GestureState			m_gestureState;
	public GestureState 	getGestureState
	{
		get
		{
			return m_gestureState;
		}
		set 
		{
			m_gestureState = value;
		}
	}


	static BattleManager	m_instance;

	void Awake()
	{
		if(m_instance == null)
			m_instance = this;
	}

	void Start()
	{
		m_enemyMgr = EnemyManager.Get();
		m_playerMgr = PlayerManager.Get();

		m_gestureStart = false;
		m_gestureState = GestureState.START;
		m_phase = BattlePhase.START;

		gaugeCount = 0;
	}

	IEnumerator CommenceAttack()
	{			
		yield return new WaitForSeconds(m_interval);
		if (GestureGenerateMethod != null) {
			GestureGenerateMethod ();
		}
		yield return new WaitForSeconds(m_gestureInv);
		Debug.Log(Time.time);

		//Destroy Gesture
		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();
		m_gestureState = GestureState.END;

		if(m_phase == BattlePhase.SPECIAL)
		{
			m_phase = BattlePhase.ATTACK;
			gaugeCount = 0;
		}

		yield return new WaitForSeconds(m_interval);
		m_gestureState = GestureState.START;



	}

	void Update()
	{
		switch(m_phase)
		{
			case BattlePhase.START:
			{
				m_phase = BattlePhase.ATTACK;

			}
			break;
			case BattlePhase.ATTACK:
			{
				if(gaugeCount >= m_fullgaugeCount)
				{
					Debug.Log("*****************battle phase: " + m_phase );
					m_phase = BattlePhase.SPECIAL;
					break;

				}
				if(m_gestureState == GestureState.START)
				{
					m_gestureState = GestureState.SHOWING;
					m_gestureStart = true;
					m_coroutine = CommenceAttack();
					GestureGenerateMethod = m_gestureGenerator.GenerateEasyGesture;

					StartCoroutine(m_coroutine);
				}
			}
			break;
			case BattlePhase.SPECIAL:
			{
				if(m_gestureState == GestureState.START)
				{
					m_gestureState = GestureState.SHOWING;
					m_gestureStart = true;
					m_coroutine = CommenceAttack();
					GestureGenerateMethod = m_gestureGenerator.GenerateHardGesture;
					StartCoroutine(m_coroutine);
				}
			}
			break;

			case BattlePhase.STUNNED:
			break;
		}
	}

	public void Correct()
	{
		StopCoroutine(m_coroutine);
		m_gestureState = GestureState.START;

		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();

		++gaugeCount;

		if(m_phase == BattlePhase.SPECIAL)
		{
			m_phase = BattlePhase.ATTACK;
			gaugeCount = 0;
		}
	}

	public static BattleManager Get()
	{
		return m_instance;
	}

}
