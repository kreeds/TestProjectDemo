﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Manage Enemy, Manage Event
/// </summary>
public class BattleManager : MonoBehaviour 
{
	public InputGestures m_gestureHandler;
	EnemyManager	m_enemyMgr;
	PlayerManager	m_playerMgr;

	// Measured in seconds
	public float m_gestureInv = 3;
	public float m_interval = 3;
	public float m_specialInv = 3;
	public float m_stunInv = 3;

	IEnumerator m_coroutine;

	bool m_gestureStart;

	// Battle Phases
	private enum BattlePhase
	{
		START,
		ATTACK,
		SPECIAL,
		STUNNED,

		TOTAL

	};

	BattlePhase m_phase; 

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
	}

	IEnumerator CommenceAttack()
	{			
		yield return new WaitForSeconds(m_interval);
		if (m_gestureHandler != null) {
			m_gestureHandler.GenerateRandomGesture ();
		}
		yield return new WaitForSeconds(m_gestureInv);
		Debug.Log(Time.time);

		//Destroy Gesture
		if(m_gestureHandler != null)
			m_gestureHandler.DestroyGesture();
		m_gestureState = GestureState.END;

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
				if(m_gestureState == GestureState.START)
				{
					m_gestureState = GestureState.SHOWING;
					m_gestureStart = true;
					m_coroutine = CommenceAttack();
					StartCoroutine(m_coroutine);
				}
			}
			break;
			case BattlePhase.SPECIAL:
			break;

			case BattlePhase.STUNNED:
			break;
		}
	}

	public void Correct()
	{
		StopCoroutine(m_coroutine);
		m_gestureState = GestureState.START;

		if(m_gestureHandler != null)
			m_gestureHandler.DestroyGesture();
	}

	public static BattleManager Get()
	{
		return m_instance;
	}

}
