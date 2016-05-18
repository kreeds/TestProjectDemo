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

	// Coroutines
	IEnumerator m_coroutine;
	IEnumerator m_cntDwnRnt;					//Count Down Coroutine;

	bool m_gestureStart;
	bool m_beginFinisher;

	HUDService m_HUDService;
	SoundService m_soundService;

	[SerializeField]GameObject		m_boardCollider;
	[SerializeField]GameObject		m_loseLogo;
	[SerializeField]BoardHandler	m_boardHandler;
	[SerializeField]GameObject		m_itemParent;
	[SerializeField]Transform		m_bgParent;
	[SerializeField]string			m_bgmMusic;

	GameObject m_FXObj;

	AudioClip m_BGM;
	AudioClip m_SPECIAL;

	bool m_win;
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

		m_win = true;

		Service.Init();
		m_HUDService = Service.Get<HUDService>();
		m_HUDService.StartScene();

		// Create Battle HUD
		m_HUDService.CreateBattleHUD();
		m_HUDService.ShowBottom(false);
		m_HUDService.HUDControl.SetSpecialEnable(false);

		// Create Sound Service
		m_soundService = Service.Get<SoundService>();
		m_soundService.PreloadSFXResource(new string[13]{"attack01", "attack02", "attack03", 
															"countdown", "enemyattack", "finalstrokeappear", 
															"gaugefull", "magicnotecorrect", "playermoveattack", 
															"sceneswish", "supermove", "win", "LadyKnight_Shine"});

		m_BGM = Resources.Load("Music/" + m_bgmMusic) as AudioClip;
		m_SPECIAL = Resources.Load("Music/supermove_jingle" ) as AudioClip;
		m_soundService.PlayMusic(m_BGM, true);
		// Create Battle HUD

	}

	IEnumerator CommenceAtkInterval()
	{	
		// Testing for Emiko's Proposal		
		// yield return new WaitForSeconds(m_interval);
		if (GestureGenerateMethod != null) {
			GestureGenerateMethod ();
		}

		m_HUDService.HUDControl.ShowTip(true);

		// Display Special Effect
		m_FXObj = Instantiate(Resources.Load ("Prefabs/FX/Special_Attack_Fx")) as GameObject;
		m_FXObj.transform.SetParent(m_itemParent.transform, false);
		m_FXObj.transform.localPosition = new Vector3(720, m_FXObj.transform.localPosition.y, -20); 

		// Remove HP Bar
		m_HUDService.HUDControl.ShowHPBars(false);
		m_HUDService.ShowQuestTime(false);



		m_cntDwnRnt = Utility.DelayInSeconds(m_gestureInv - m_specialCountDown, 
						(res) => 
						{ 
							GameObject obj = Instantiate(Resources.Load("Prefabs/Battle/CountDown")) as GameObject;
							obj.transform.SetParent(m_HUDService.HUDControl.transform, false);
							obj.transform.localScale = new Vector3(180, 180, 1);
						} );
		StartCoroutine(m_cntDwnRnt);


		// Count Down till Gesture Failure
		yield return new WaitForSeconds(m_gestureInv);

		ClearGesture();

		m_playerMgr.RemoveBB();
		m_HUDService.HUDControl.ShowHPBars(true);
		m_HUDService.ShowQuestTime(true);
		m_soundService.StopMusic(m_SPECIAL);
		m_soundService.PlayMusic(m_BGM, true);

		if(m_FXObj != null)
			Destroy(m_FXObj);

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
				if(m_gestureState == GestureState.START)
				{
					m_gestureHandler.DisableGesture = false;
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
				if(m_win)
				{
					StartCoroutine(Utility.DelayInSeconds( 4.0f, 
														(res) => 
														{ 
															m_soundService.StopMusic(m_SPECIAL);
														
															m_boardCollider.SetActive(true);
															if(m_boardHandler != null)
																m_boardHandler.Init(m_win);
														} ));
				}
				else
				{
					m_win = true; // Reset win
					GameObject obj = Instantiate(Resources.Load("Prefabs/GeneralPopUp")) as GameObject;
					GeneralPopUp popuphandler = obj.GetComponent<GeneralPopUp>();
					popuphandler.Init("Use 1 Gem to Revive with full HP.", "Yes", "No", Revive, Exit);
					m_HUDService.HUDControl.AttachMid(ref obj);
				
				}													

				m_phase = BattlePhase.TOTAL;
			}
			break;
		}
	}

	void Revive()
	{	
		m_playerMgr.Recover(100);
		m_HUDService.HUDControl.ShowActionButtons(true);
		if(m_HUDService.HUDControl.GetSpecialAmount == 1.0f)
		{
			SoundService ss = Service.Get<SoundService>();
			ss.PlaySound(ss.GetSFX("gaugefull"), false);
			m_HUDService.HUDControl.SetSpecialFxGlow(true);
			m_HUDService.HUDControl.SetSpecialEnable(true);
		}
		m_phase = BattlePhase.ATTACK;
	}
	void Exit()
	{
		m_HUDService.ChangeScene("MainMenu");
		m_HUDService.HUDControl.RemoveBattleHUD();
	}

	public void ClearGesture()
	{
		if(m_gestureGenerator != null)
			m_gestureGenerator.DestroyGesture();

		if(m_gestureHandler != null)
			m_gestureHandler.DestroyTrails();
	}

	public void CorrectGesture()
	{
		ClearGesture();

		// play effects for Correct Gestures
		GameObject spark = Instantiate(Resources.Load("Prefabs/FX/Special_Attack_Fx_Pass")) as GameObject;
		spark.transform.SetParent(m_itemParent.transform, false);

	
		m_soundService.PlaySound(m_soundService.GetSFX("magicnotecorrect"), false);
		// Generate new Gesture
		if (GestureGenerateMethod != null) {
			GestureGenerateMethod ();
		}
	}

	/// <summary>
	/// Stops the Corountines for Special.
	/// </summary>
	public void StopSpecialCoroutines()
	{
		if(m_cntDwnRnt != null)
			StopCoroutine(m_cntDwnRnt);
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);
		
	}

	public void Correct()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);

		m_gestureState = GestureState.START;

		m_playerMgr.RemoveBB();

		m_HUDService.HUDControl.ShowHPBars(true);
		m_HUDService.ShowQuestTime(true);

		if(m_FXObj != null)
			Destroy(m_FXObj);

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

	public void SpecialCorrect()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);
		m_gestureState = GestureState.START;

		m_playerMgr.RemoveBB();

		m_HUDService.HUDControl.ShowHPBars(true);
		m_HUDService.ShowQuestTime(true);

		if(m_FXObj != null)
			Destroy(m_FXObj);

		m_phase = BattlePhase.ATTACK;
		gaugeCount = 0;
		ResetGauge();
	}

	public void ResetGauge()
	{
		m_HUDService.HUDControl.SetSpecialGaugeAmt(0);
	}
	public static BattleManager Get()
	{
		return m_instance;
	}

	public void GameOver()
	{
		m_phase = BattlePhase.END;
		m_win = false;
	}

	public void ReduceGauge()
	{
		--gaugeCount;
		if(gaugeCount < 0)
			gaugeCount = 0;

		m_HUDService.HUDControl.SetSpecialGaugeAmt((float)gaugeCount/m_fullgaugeCount);
	}

	//TODO: Add parameter to take in data for items to launch
	public void CreateEmitter(Vector3 pos, ref ItemType[] type, ref int[] amount, 
								float rangeMinX, float rangeMaxX, float rangeMinY, 
								float rangeMaxY, Emitter.EmitType mytype = Emitter.EmitType.Flow)
	{
		GameObject obj = NGUITools.AddChild(m_itemParent, Resources.Load("Prefabs/Emitter") as GameObject);
		obj.transform.localPosition = pos;

		obj.GetComponent<Emitter>().Init(type, amount, rangeMinX, rangeMaxX, rangeMinY, rangeMaxY, mytype);
	}

	#region Button
	void OnFinishPressed()
	{
		if(m_coroutine != null)
			StopCoroutine(m_coroutine);

		m_gestureState = GestureState.START;

		m_soundService.StopMusic(m_BGM);
		m_soundService.PlayMusic(m_SPECIAL, true);

		m_HUDService.HUDControl.SetSpecialFxGlow(false);

		ResetGauge();

		m_beginFinisher = true;
		m_HUDService.HUDControl.ShowActionButtons(false);
		m_HUDService.HUDControl.SetSpecialEnable(false);
	}

	#endregion

}
