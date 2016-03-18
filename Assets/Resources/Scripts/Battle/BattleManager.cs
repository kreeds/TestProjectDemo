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

	#region Field Time Counters
	// Measured in seconds
	public float m_gestureInv = 3;
	public float m_interval = 5;
	public float m_specialInv = 3;
	public float m_stunInv = 3;
	public float m_specialCountDown = 3.0f;
	#endregion

	IEnumerator m_coroutine;

	bool m_gestureStart;
	bool m_beginFinisher;

	HUDService m_HUDService;

	[SerializeField]GameObject		m_winLogo;
	[SerializeField]BoardHandler	m_boardHandler;
	[SerializeField]GameObject		m_itemParent;
	[SerializeField]Transform		m_bgParent;

	GameObject m_FXObj;

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

		// Display Special Effect

		m_FXObj = NGUITools.AddChild (m_itemParent, Resources.Load ("Prefabs/FX/Special_Attack_Fx") as GameObject);
		m_FXObj.transform.localPosition = Vector3.zero;
		m_FXObj.transform.localPosition = new Vector3 (720, 0, -20f);


		StartCoroutine(Utility.DelayInSeconds(m_specialCountDown, 
						(res) => 
						{ 
							GameObject obj = Instantiate(Resources.Load("Prefabs/Battle/CountDown")) as GameObject;
							obj.transform.parent = m_HUDService.HUDControl.transform;
							obj.transform.localScale = new Vector3(180, 180, 1);
						} ));


		// Count Down till Gesture Failure
		yield return new WaitForSeconds(m_gestureInv);

		ClearGesture();

		m_gestureState = GestureState.END;

		gaugeCount = 0;

		ResetGauge();

		m_phase = BattlePhase.ATTACK;

		// Move Camera to view Enemy
		Service.Get<MapService>().TweenPos(new Vector3(703f, -3.76f, 0.0f),
													new Vector3(-703f, -3.76f, 0.0f),
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

					GestureGenerateMethod = m_gestureGenerator.GenerateEasyGesture;
					StartCoroutine(m_coroutine);
				}
			}
			break;
			case BattlePhase.END:
			{
				StartCoroutine(Utility.DelayInSeconds( 5.0f, 
														(res) => 
														{ 
															if(m_winLogo != null) 
																m_winLogo.SetActive(true); 
															if(m_boardHandler != null)
																m_boardHandler.Init(true);
														} ));
			}
			break;
		}
	}


	void ClearGesture()
	{
		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();

		if(m_gestureHandler != null)
			m_gestureHandler.DestroyTrails();

		if(m_FXObj != null)
			Destroy(m_FXObj);
	}

	public void CorrectGesture()
	{
		ClearGesture();

		if (GestureGenerateMethod != null) {
			GestureGenerateMethod ();
		}
	}

	public void Correct()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);

		m_gestureState = GestureState.START;

		ClearGesture();

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
	public void CreateEmitter(Vector3 pos, ref ItemType[] type, ref int[] amount, float rangeMinX, float rangeMaxX, float rangeMinY, float rangeMaxY)
	{
		GameObject obj = NGUITools.AddChild(m_itemParent, Resources.Load("Prefabs/Emitter") as GameObject);
		obj.transform.localPosition = pos;

		obj.GetComponent<Emitter>().Init(type, amount, rangeMinX, rangeMaxX, rangeMinY, rangeMaxY, Emitter.EmitType.Flow);
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
		m_HUDService.HUDControl.ShowActionButtons(false);
		m_HUDService.HUDControl.SetSpecialEnable(false);
	}
	#endregion

}
