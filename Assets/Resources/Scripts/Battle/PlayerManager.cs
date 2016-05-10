using UnityEngine;
using System.Collections;


public struct Skill
{
	public string name;
	public int id;
	public int dmg;
	public int energyCost;
}
struct PlayerStats
{
	public int hp;
	public int bravebar;
	public int atk;
	public int energy;
	public int totalenergy;
	public int goldcount;
	public int gemcount;
}

public class PlayerManager : MonoBehaviour {

	PlayerStats m_player;
	Skill[] m_skills;

	BattleManager m_battleMgr;
	EnemyManager m_enemyMgr;
	InputManager m_inputMgr;

	MapService m_camService;
	HUDHandler m_handler;
	Coroutine m_routine;

	[SerializeField]UIGauge m_gauge;
	[SerializeField]UIGauge m_specGauge;
	[SerializeField]LAppModelProxy l2dInterface;
	[SerializeField]GameObject m_obj;
	[SerializeField]float waitinterval;

	bool isPlaying = true;
	bool isBeingAttacked = false;
	bool specialAtk = false;
	bool attackend = false;

	bool playerattack = false; 								// flag to determine if player has dealt damage

	[SerializeField]int m_specialFullGauge = 2;				// Gauge used for special attack. If it is full use, show bb button
	int m_specGaugeCnt = 0;									// Special counter used for special gauge Count;


	const float ranChance = 1.0f;
	GameObject m_braveBurst;
	

	SoundService m_soundService;


	public bool isAttackComplete
	{
		get{ return attackend; }
		set{ attackend = value; }
	}

	public static PlayerManager _instance;


	public static PlayerManager Get()
	{
		return _instance;
	}

	public LAppModelProxy Model
	{
		get{return l2dInterface;}
	}

	void Awake()
	{
		if(_instance == null)
			_instance = this;
	}
	// Use this for initialization
	void Start () 
	{

		m_enemyMgr = EnemyManager.Get();
		m_battleMgr = BattleManager.Get();
		m_handler = Service.Get<HUDService>().HUDControl;
		m_inputMgr = InputManager.Get();



		// Initialize base value of player
		m_player = new PlayerStats();
		m_player.atk = 10;
		m_player.hp = 100;
		m_player.bravebar = 0;

		m_skills = new Skill[3];
		m_skills[0].name = "LadyKnight Punch";
		m_skills[0].energyCost = 3;
		m_skills[0].id = 1;
		m_skills[1].name = "LadyKnight Kick";
		m_skills[1].energyCost = 4;
		m_skills[1].id = 2;
		m_skills[2].name = "Strike A Pose";
		m_skills[2].energyCost = 8;
		m_skills[2].id = 3;

		//Create Skill Buttons

		m_camService = Service.Get<MapService>();
		m_camService.Init();

		if(m_handler != null)
		{
			m_handler.AddActionButton("NormalAttack", this.gameObject, m_skills[0]);
			m_handler.AddActionButton("NormalAttack", this.gameObject, m_skills[1]);
			m_handler.AddActionButton("NormalAttack", this.gameObject, m_skills[2]);

			m_handler.InitializeGauge((int)GAUGE.PLAYER, m_player.hp, m_player.hp, "Ellie");
		}

//		l2dInterface.LoadProfile ();

		l2dInterface.GetModel ().StopBasicMotion (true);
		l2dInterface.PlayCombatIdleAnim ();

		m_soundService = Service.Get<SoundService>();
	}

	// Update is called once per frame
	void Update () {
		if(!isPlaying && l2dInterface.IsAnimationComplete())
		{
			isPlaying = true;
			// Scroll to enemy
			TweenAttack(true);

			l2dInterface.PlayCombatIdleAnim();
		}

		if(isBeingAttacked && l2dInterface.IsAnimationComplete())
		{
			isBeingAttacked = false;
			l2dInterface.PlayCombatIdleAnim();
		}
	}


	void AttackEffect(float x, float y)
	{
		GameObject obj = NGUITools.AddChild (Service.Get<HUDService>().HUDControl.gameObject, Resources.Load ("Prefabs/Attack01_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (x, y);

	}
	void OnAttackEnd()
	{

		StartCoroutine(Utility.DelayInSeconds(0.2f, (res) =>
		{
			if(m_enemyMgr != null)
			{
				if(!specialAtk)
				{
					AttackEffect(16, 112);
					StartCoroutine(Utility.DelayInSeconds(0.5f,
									(res1) => {m_enemyMgr.damageEnemy(m_player.atk); 	
									attackend = true;
									playerattack = true; 
									m_soundService.PlaySound(m_soundService.GetSFX("attack03"), false);
									} ));


					if(m_battleMgr != null)
						m_battleMgr.Correct();

		
				}
				else
				{
					AttackEffect(16, 112);
					AttackEffect(-152, 112);
					AttackEffect(-23, -137);

					StartCoroutine(Utility.DelayInSeconds(0.5f,
									(res1) => 
									{
										GameObject obj = NGUITools.AddChild (Service.Get<HUDService>().HUDControl.gameObject, Resources.Load ("Prefabs/FX/WhiteFader_FX") as GameObject);
										obj.transform.localScale = new Vector3(2048f, 2048f, 0);
										obj.GetComponent<WhiteFader>().fadeSpeed = 0.5f;

										m_enemyMgr.killEnemy();
										m_soundService.PlaySound(m_soundService.GetSFX("attack03"), false);
										m_soundService.PlaySound(m_soundService.GetSFX("attack03"), false);
										m_soundService.PlaySound(m_soundService.GetSFX("attack03"), false);
										attackend = true;
										playerattack = true;
									} ));
					
				}
			}



			//StartCoroutine(ReturnCamera());
		
		}
		));


	}
	void OnMovementEnd()
	{
		StartCoroutine(Utility.DelayInSeconds(0.2f, (res) =>
		{

			// Add Dodge here
			if(!CalculateDodgeChance())
			{
				AttackEffect(16, 112);
				DamageEffect();
			}
			else
			{
				// move player and spawn label miss
				m_handler.ShowDodgeBtn(true);

				// Fail to press
				m_routine = StartCoroutine(Utility.DelayInSeconds(1.0f, 
											(res1) => 
											{ 
												m_handler.ShowDodgeBtn(false); 
												AttackEffect(16, 112); 
												DamageEffect(); 

											} ) ); 
			}											

			m_soundService.PlaySound(m_soundService.GetSFX("attack01"), false);
		}
		));

	}

	void OnEffectDestroy()
	{
		m_enemyMgr.damageEnemy(m_player.atk);
	}

	bool CalculateDodgeChance()
	{
//		float rand = Random.Range(0.0f, 1.0f);
//		Debug.Log( "Random: " + rand);
//		if(rand <= ranChance)
//		{
//			// Show chance
//			m_handler.ShowDodgeBtn(true);
//			return true;
//		}
		return false;
	}

	#region Dodge Mechanic
	void Dodge()
	{
		TweenPosition tpos = l2dInterface.GetComponent<TweenPosition>();
		if(tpos != null)
			tpos.Play(true);

		m_handler.ShowDodgeBtn(false);

		if(m_routine != null)
			StopCoroutine(m_routine);

		StartCoroutine(Utility.DelayInSeconds(1.0f, 
						(res1) => { 
							if(tpos != null)
							{
								tpos.eventReceiver = null;
								tpos.callWhenFinished = "";
								tpos.Play(false);
							}
							m_handler.ShowActionButtons(true);
						} ) ); 
	}
	void DamageEffect()
	{
		// Apply Damage to player
		Damaged(m_enemyMgr.GetCurrentEnemyAttack());

		if(m_battleMgr.CurBattlePhase != BattleManager.BattlePhase.END)
		StartCoroutine(Utility.DelayInSeconds(2, (res1)=>{
											m_handler.ShowActionButtons(true);
											if(m_handler.GetSpecialAmount == 1.0f)
											{
												SoundService ss = Service.Get<SoundService>();
												ss.PlaySound(ss.GetSFX("gaugefull"), false);
												m_handler.SetSpecialFxGlow(true);
											}
											if(playerattack)
											{
												m_handler.SetSpecialEnable(true);
												playerattack = false;
											}

											}));
	}
	#endregion

	public void AddSpecialCount()
	{
		m_specGaugeCnt++;
		if(m_specGaugeCnt == m_specialFullGauge)
		{	
			SpecialAttack();
			m_specGaugeCnt = 0;
		}
	}

	/// <summary>
	/// Commence Attack Event
	/// </summary>
	void SpecialAttack()
	{
		m_braveBurst = Instantiate(Resources.Load ("Prefabs/Battle/BraveBurst")) as GameObject;
		m_braveBurst.transform.SetParent(Camera.main.transform, false);
		m_braveBurst.layer = LayerMask.NameToLayer("EffectUI");

		m_inputMgr.DisableGesture = true;

		if(m_battleMgr != null)
		{
			m_battleMgr.StopSpecialCoroutines();
			m_battleMgr.ClearGesture();
		}

		m_soundService.PlaySound(m_soundService.GetSFX("finalstrokeappear"), false);

		specialAtk = true;
	}


	public void RemoveBB()
	{
		// Remove Brave Burst
		if(m_braveBurst != null)
			Destroy(m_braveBurst);
	}


	public void NormalAttack()
	{
		l2dInterface.PlayAttackAnim ();
		m_soundService.PlaySound(m_soundService.GetSFX("playermoveattack"), false);
		//Service.Get<HUDService>().ShowMid(false);
		m_handler.ShowActionButtons(false);
		isPlaying = attackend = specialAtk = false;
		Service.Get<HUDService>().HUDControl.SetSpecialEnable(false);

	}

	/// <summary>
	/// Damage Done to player
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void Damaged(int damage)
	{
		m_player.hp -= damage;

		if(m_player.hp <= 0 )
		{
			m_player.hp = 0;
			m_battleMgr.GameOver();
			// Activate Lose State
		}

		if(m_handler != null)
			m_handler.reduce((int)GAUGE.PLAYER, damage);

		isBeingAttacked = true;
		l2dInterface.PlayDamageAnim ();
	}

	/// <summary>
	/// Recover healthpoint with the specified amount.
	/// </summary>
	/// <param name="amt">Amt.</param>
	public void Recover(int amt)
	{
		m_player.hp += amt;

		if(m_handler != null)	
			m_handler.reduce((int)GAUGE.PLAYER, -amt);
	}

	/// <summary>
	/// Shows the special effect for attack
	/// </summary>
	public void ShowSpecialEffect()
	{
		RemoveBB();

		m_soundService.PlaySound(m_soundService.GetSFX("spatk02"), false);

		GameObject obj = NGUITools.AddChild (m_obj, Resources.Load ("Prefabs/FX/Effect_FX") as GameObject);
		obj.transform.localPosition = new Vector3 (720, 0, -20f);
		BattleEffect effect = obj.GetComponent<BattleEffect>();
		if (effect != null) {
			effect.Initialize(gameObject);
			m_soundService.PlaySound(m_soundService.GetSFX("supermove"), false);
		}
	}

	/// <summary>
	/// Raises the Special attack effect finish event.
	/// </summary>
	private void OnEffectFinish()
	{
		TweenAttack(true);
		if(m_battleMgr != null)
			m_battleMgr.ResetGauge();

		// Play Special Attack Effect
		//GameObject speedlines = Instantiate(Resources.Load ("Prefabs/FX/Mob_Speedlines_Fx")) as GameObject;
		//speedlines.transform.SetParent(Service.Get<HUDService>().HUDControl.transform, false);
	}

	/// <summary>
	/// Tween Motion effect for attacking
	/// </summary>
	/// <param name="isAttacking">If set to <c>true</c> is attacking.</param>
	private void TweenAttack(bool isAttacking)
	{
		Debug.Log("TweenAttack: " + isAttacking);
		Vector3 from, to;
		from = (isAttacking)? new Vector3(703f, -3.76f, 0.0f) : new Vector3(-703f, -3.76f, 0.0f);
		to = (isAttacking)? new Vector3(-703f, -3.76f, 0.0f) : new Vector3(703f, -3.76f, 0.0f);

		m_camService.TweenPos(	from,
								to,
								UITweener.Method.EaseInOut,
								UITweener.Style.Once,
								gameObject,
								"OnAttackEnd");
		m_soundService.PlaySound(m_soundService.GetSFX("sceneswish"), false);


		// When attack tweening is happening, special attack should not be active
		Service.Get<HUDService>().HUDControl.SetSpecialEnable(false);
	}
	#region IEnumurators
	IEnumerator ReturnCamera()
	{
		yield return new WaitForSeconds(waitinterval);
		TweenAttack(false);
	}
	#endregion
}