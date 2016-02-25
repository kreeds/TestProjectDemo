using UnityEngine;
using System.Collections;

/// <summary>
/// Manage Enemy, Manage Event
/// </summary>
public class BattleManager : MonoBehaviour 
{
	public delegate void GestureMethod();
	GestureMethod GestureGenerateMethod;

	public GenerateGesture m_gestureGenerator;
	public GameObject	m_actionRoot;

	EnemyManager	m_enemyMgr;
	PlayerManager	m_playerMgr;
	InputManager	m_gestureHandler;


	public int m_fullgaugeCount = 5;
	int gaugeCount = 0;

	// Measured in seconds
	public float m_gestureInv = 3;
	public float m_interval = 5;
	public float m_specialInv = 3;
	public float m_stunInv = 3;

	IEnumerator m_coroutine;

	bool m_gestureStart;
	bool m_beginFinisher;

	HUDService m_HUDService;

	[SerializeField]UIGauge			m_finishGauge;
	[SerializeField]UIButton		m_finishButton;
	[SerializeField]GameObject		m_winLogo;

	// Battle Phases
	public enum BattlePhase
	{
		START,
		ATTACK,
		SPECIAL,
		STUNNED,
		END,

		TOTAL

	};

	BattlePhase m_phase; 
	public BattlePhase CurBattlePhase
	{
		get{return m_phase;}
		set{
			m_phase = value;
		}
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
	public GestureState 	currentGestureState
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
		m_gestureHandler = InputManager.Get();

		m_gestureStart = false;
		m_beginFinisher = false;

		m_gestureState = GestureState.START;
		m_phase = BattlePhase.START;

		gaugeCount = 0;

		m_finishGauge.Init (gaugeCount, m_fullgaugeCount);

		m_finishButton.isEnabled = false;

		Service.Init();
		m_HUDService = Service.Get<HUDService>();
		m_HUDService.StartScene();

		// Create Battle HUD
		m_HUDService.CreateBattleHUD();
		m_HUDService.ShowBottom(false);
	}

	IEnumerator CommenceAttack()
	{	
		//Testing for Emiko's Proposal		
		//yield return new WaitForSeconds(m_interval);
		if (GestureGenerateMethod != null) {
			GestureGenerateMethod ();
		}
		yield return new WaitForSeconds(m_gestureInv);

		//Destroy Gesture
		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();
		m_gestureState = GestureState.END;

		if(m_phase == BattlePhase.SPECIAL)
		{
			m_phase = BattlePhase.ATTACK;
			gaugeCount = 0;
		}

		m_actionRoot.SetActive(true);
		m_phase = BattlePhase.ATTACK;

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
				m_gestureHandler.DisableGesture = true;
				if(gaugeCount >= m_fullgaugeCount)
				{
					if(m_beginFinisher)
					{
						m_phase = BattlePhase.SPECIAL;
						m_beginFinisher = false;

					}
					else
					{
						m_finishButton.isEnabled = true;
					}
				}
			}
			break;
			case BattlePhase.SPECIAL:
			{
				m_gestureHandler.DisableGesture = false;
				if(m_gestureState == GestureState.START)
				{
					if(m_actionRoot != null)
						m_actionRoot.SetActive(false);

					m_gestureState = GestureState.SHOWING;
					m_gestureStart = true;
					m_coroutine = CommenceAttack();

					GestureGenerateMethod = m_gestureGenerator.GenerateHardGesture;
					StartCoroutine(m_coroutine);
				}
			}
			break;
			case BattlePhase.END:
			{
				if(m_winLogo != null)
					m_winLogo.SetActive(true);
			}
			break;
		}
	}

	public void Correct()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);
		m_gestureState = GestureState.START;

		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();

		++gaugeCount;
		m_finishGauge.gain (1);

		if(m_phase == BattlePhase.SPECIAL)
		{
			m_phase = BattlePhase.ATTACK;
			gaugeCount = 0;
		}
		m_actionRoot.SetActive(true);
	}

	public void ResetGauge()
	{
		m_finishGauge.Init(gaugeCount, m_fullgaugeCount);
	}
	public static BattleManager Get()
	{
		return m_instance;
	}

	public void ReduceGauge()
	{
		--gaugeCount;
		m_finishGauge.reduce(1);
	}


	void OnFinishPressed()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);

		m_gestureState = GestureState.START;

//		if(m_gestureGenerator != null)
//			m_gestureGenerator.DestroyGesture();

		m_beginFinisher = true;
		m_actionRoot.SetActive(false);

		m_finishButton.isEnabled = false;
	}

}
