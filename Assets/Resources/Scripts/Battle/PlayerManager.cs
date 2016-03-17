using UnityEngine;
using System.Collections;


struct Skill
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
	Skill m_skills;

	BattleManager m_battleMgr;
	EnemyManager m_enemyMgr;
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



		// Initialize base value of player
		m_player = new PlayerStats();
		m_player.atk = 10;
		m_player.hp = 100;
		m_player.bravebar = 0;

		m_skills = new Skill();
		m_skills.name = "SMOTHER FRIES";
		m_skills.energyCost = 4;
		m_skills.id = 1;

		//Create Skill Buttons

		m_camService = Service.Get<MapService>();

		if(m_handler != null)
		{
			m_handler.AddActionButton("NormalAttack", this.gameObject);
			m_handler.InitializeGauge((int)GAUGE.PLAYER, m_player.hp, m_player.hp, "Player");
		}

		l2dInterface.LoadProfile ();

		l2dInterface.GetModel ().StopBasicMotion (true);
		l2dInterface.PlayCombatIdleAnim ();
	}

	// Update is called once per frame
	void Update () {
		if(!isPlaying && l2dInterface.IsAnimationComplete())
		{
			isPlaying = true;
			// Scroll to enemy
			TweenAttack(true);
			l2dInterface.PlayIdleAnim();
		}

		if(isBeingAttacked && l2dInterface.IsAnimationComplete())
		{
			isBeingAttacked = false;
			l2dInterface.PlayIdleAnim();
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
					//m_enemyMgr.damageEnemy(m_player.atk);
						StartCoroutine(Utility.DelayInSeconds(0.5f,
										(res1) => {m_enemyMgr.damageEnemy(m_player.atk); 	
										attackend = true;
										playerattack = true; 
										} ));

		
				}
				else
				{
					AttackEffect(16, 112);
					AttackEffect(-152, 112);
					AttackEffect(-23, -137);
					GameObject obj = NGUITools.AddChild (Service.Get<HUDService>().HUDControl.gameObject, Resources.Load ("Prefabs/FX/WhiteFader_FX") as GameObject);
					obj.transform.localScale = new Vector3(2048f, 2048f, 0);
					m_enemyMgr.killEnemy();

					attackend = true;
					playerattack = true;
				}
			}


			if(m_battleMgr != null)
				m_battleMgr.Correct();

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

		StartCoroutine(Utility.DelayInSeconds(2, (res1)=>{
											m_handler.ShowActionButtons(true);
											if(playerattack)
											{
												Service.Get<HUDService>().HUDControl.SetSpecialEnable(true);
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

		//Update Gauge
	}



	/// <summary>
	/// Commence Attack Event
	/// </summary>
	void SpecialAttack()
	{
		GameObject obj = NGUITools.AddChild (m_obj, Resources.Load ("Prefabs/FX/Effect_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (720, 0, -20f);
		BattleEffect effect = obj.GetComponent<BattleEffect>();
		if (effect != null) {
			effect.Initialize(gameObject);
		}

		if(m_battleMgr != null)
			m_battleMgr.Correct();

		specialAtk = true;
	}

	public void NormalAttack()
	{
		l2dInterface.PlayAttackAnim ();

		//Service.Get<HUDService>().ShowMid(false);
		m_handler.ShowActionButtons(false);
		isPlaying = attackend = specialAtk = false;


	}

	/// <summary>
	/// Damage Done to player
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void Damaged(int damage)
	{
		m_player.hp -= damage;

		if(m_handler != null)
			m_handler.reduce((int)GAUGE.PLAYER, damage);

		isBeingAttacked = true;
		l2dInterface.PlayDamageAnim ();
	}

	private void OnEffectFinish()
	{
		TweenAttack(true);
		if(m_battleMgr != null)
			m_battleMgr.ResetGauge();
	}

	private void TweenAttack(bool isAttacking)
	{
		Vector3 from, to;
		from = (isAttacking)? new Vector3(703f, -3.76f, 0.0f) : new Vector3(-703f, -3.76f, 0.0f);
		to = (isAttacking)? new Vector3(-703f, -3.76f, 0.0f) : new Vector3(703f, -3.76f, 0.0f);

		m_camService.TweenPos(	from,
								to,
								UITweener.Method.EaseInOut,
								UITweener.Style.Once,
								gameObject,
								"OnAttackEnd");
	}
	#region IEnumurators
	IEnumerator ReturnCamera()
	{
		yield return new WaitForSeconds(waitinterval);
		TweenAttack(false);
	}
	#endregion
}