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

	EnemyManager	m_enemyMgr;
	PlayerManager	m_playerMgr;
	InputManager	m_gestureHandler;


	public int m_fullgaugeCount = 5;
	int gaugeCount = 1;

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
	[SerializeField]GameObject		m_winLogo;
	[SerializeField]GameObject		m_itemParent;

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

		Service.Init();
		m_HUDService = Service.Get<HUDService>();
		m_HUDService.StartScene();

		// Create Battle HUD
		m_HUDService.CreateBattleHUD();
		m_HUDService.ShowBottom(false);
		m_HUDService.HUDControl.SetSpecialEnable(false);
	}

	IEnumerator CommenceAtkInterval()
	{	
		//Testing for Emiko's Proposal		
		//yield return new WaitForSeconds(m_interval);
		if (GestureGenerateMethod != null) {
			GestureGenerateMethod ();
		}

		// Count Down till Gesture Failure
		yield return new WaitForSeconds(m_gestureInv);

		// Gesture Failure, Destroy Gesture and reset State
		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();

		m_gestureState = GestureState.END;

		gaugeCount = 0;

		ResetGauge();

		m_phase = BattlePhase.ATTACK;

		// Move Camera to view Enemy
		Service.Get<CameraService>().TweenPos(new Vector3(1.46f, -3.76f, 0.0f),
													new Vector3(-1.46f, -3.76f, 0.0f),
													UITweener.Method.EaseInOut,
													UITweener.Style.Once,
													null,
													"");

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
				}
			}
			break;
			case BattlePhase.SPECIAL:
			{
				m_gestureHandler.DisableGesture = false;
				if(m_gestureState == GestureState.START)
				{
					m_gestureState = GestureState.SHOWING;
					m_gestureStart = true;
					m_coroutine = CommenceAtkInterval();

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
		if(gaugeCount > m_fullgaugeCount)
			gaugeCount = m_fullgaugeCount;

		if(m_phase == BattlePhase.SPECIAL)
		{
			m_phase = BattlePhase.ATTACK;
			gaugeCount = 0;
			ResetGauge();
		}
		else
		{
			m_HUDService.HUDControl.SetSpecialGaugeAmt((float)gaugeCount/m_fullgaugeCount);
		}
	}
	 
	public void ResetGauge()
	{
		m_HUDService.HUDControl.SetSpecialGaugeAmt(0);
	}
	public static BattleManager Get()
	{
		return m_instance;
	}

	public void ReduceGauge()
	{
		--gaugeCount;
		if(gaugeCount < 0)
			gaugeCount = 0;

		m_HUDService.HUDControl.SetSpecialGaugeAmt((float)gaugeCount/m_fullgaugeCount);
	}

	//TODO: Add parameter to take in data for items to launch
	public void CreateEmitter(Vector3 pos)
	{
		GameObject obj = NGUITools.AddChild(m_itemParent, Resources.Load("Prefabs/Emitter") as GameObject);
		obj.transform.localPosition = pos;
	}

	#region Button
	void OnFinishPressed()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);

		m_gestureState = GestureState.START;
	
		ResetGauge();

//		if(m_gestureGenerator != null)
//			m_gestureGenerator.DestroyGesture();

		m_beginFinisher = true;
		m_HUDService.ShowMid(false);
		m_HUDService.HUDControl.SetSpecialEnable(false);
	}
	#endregion

}
