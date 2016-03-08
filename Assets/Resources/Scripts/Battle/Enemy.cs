using UnityEngine;
using System.Collections;


public class Idle : FSMState
{
	BattleManager bmgr;
	PlayerManager pmgr;
	Enemy enemy;

	bool attacked;
	bool damaged;
	int prevhp;

	public Idle(Enemy emy)
	{
		bmgr = BattleManager.Get();
		pmgr = PlayerManager.Get();
		enemy = emy;
		prevhp = emy.Hp;	
		stateID = StateID.E_IDLE;
		attacked = false;
	}

	public override void OnEnter()
	{
		// Play Idle Animation
		enemy.PlayIdleAnim ();
	}

	public override void Transit()
	{
		// Death
		if(enemy.Hp <= 0)
		{
			enemy.SetTransition(Transition.E_NOHP);
			return;
		}

		// Hp Lost
		if(enemy.Hp < prevhp)
		{
			prevhp = enemy.Hp;
			enemy.SetTransition(Transition.E_LOSTHP);
			return;
		}

		if((bmgr.currentGestureState == BattleManager.GestureState.START ||
			bmgr.currentGestureState == BattleManager.GestureState.SHOWING) && attacked)
			attacked = false;

		// Attack Player
		if(pmgr.isAttackComplete || (bmgr.currentGestureState == BattleManager.GestureState.END && !attacked) )
		{
			enemy.SetTransition(Transition.E_FAILGESTURE);
			pmgr.isAttackComplete = false;
			attacked = true;
		}

	}

	public override void Update()
	{
		
	}

	public override void OnExit()
	{

	}
}

public class Damaged: FSMState
{

	Enemy enemy;
	int hp = 0;

	public Damaged(Enemy emy)
	{
		enemy = emy;
		stateID = StateID.E_DAMAGED;
		hp = emy.Hp;
	}

	public override void OnEnter()
	{
		enemy.PlayDamageAnim ();
	}

	public override void Transit()
	{
		if(enemy.IsAnimationComplete ())
		{
			if(enemy.Hp > 0)
			{
				enemy.SetTransition(Transition.E_FINISHATTACK); // Back to Idle
			}
			else
			{
				enemy.SetTransition(Transition.E_NOHP); // Transit to Death
			}
		}
//		else if(enemy.Hp < hp)
//		{
//			hp = enemy.Hp;
//			enemy.SetTransition(Transition.E_LOSTHP); // Transit to Death
//
//		}
	}

	public override void Update()
	{
		
	}

	public override void OnExit()
	{
	}


}

public class Attack : FSMState
{
	Enemy enemy;
	PlayerManager pmgr;
	BattleManager bmgr;

	int hp = 0;
	float intervalCount = 0;
	const float DelayCount = 2.0f;
	bool attack = true;


	public Attack(Enemy emy)
	{
		stateID = StateID.E_ATTACK;
		enemy = emy;
		pmgr = PlayerManager.Get();
		bmgr = BattleManager.Get();
		hp = enemy.Hp;
	
	}

	public override void OnEnter()
	{
		attack = true;

	}

	public override void Transit()
	{
		if(attack)
		{
			intervalCount += Time.deltaTime;
			if(intervalCount > DelayCount)
			{
				enemy.PlayAttackAnim();
				intervalCount = 0;
				attack = false;
			}
		}

		if(!attack && enemy.IsAnimationComplete())
		{
			enemy.SetTransition(Transition.E_FINISHATTACK);
			Service.Get<MapService>().TweenPos(new Vector3(-721.0f, 0.0f, 0.0f),
													new Vector3(721.0f, 0.0f, 0.0f),
													UITweener.Method.EaseInOut,
													UITweener.Style.Once,
													pmgr.gameObject,
													"OnMovementEnd");

		}
	}

	public override void Update()
	{

	}
	public override void OnExit()
	{
		// Restart Gesture State
		bmgr.currentGestureState = BattleManager.GestureState.START;
	}
}


public class Death : FSMState
{
	Enemy enemy;
	BattleManager bmr;

	public Death(Enemy emy)
	{
		stateID = StateID.E_DEATH;
		enemy = emy;
		bmr = BattleManager.Get();
	}

	public override void OnEnter()
	{
		enemy.PlayDeathAnim ();
	}

	public override void Transit()
	{

	}

	public override void Update()
	{
		if(enemy.IsAnimationComplete())
		{
			bmr.CurBattlePhase = BattleManager.BattlePhase.END;
			GameObject.DestroyObject(enemy.gameObject);
			Vector3 vec = new Vector3(enemy.transform.localPosition.x, 0 , 0);

			bmr.CreateEmitter(vec);
		}
	}
}

public class Enemy : MonoBehaviour {

	public int Hp;
	public int totalHp;
	public int attack;
	public int attkInterval;

	protected FiniteStateMachine enemyState;


	void Start()
	{
		InitializeStateMachine ();
	}

	protected virtual void InitializeStateMachine()
	{
		Idle idleState = new Idle(this);
		idleState.AddTransition(Transition.E_FAILGESTURE, StateID.E_ATTACK);
		idleState.AddTransition(Transition.E_LOSTHP, StateID.E_DAMAGED);
		idleState.AddTransition(Transition.E_NOHP, StateID.E_DEATH);

		Attack atkState = new Attack(this);
		atkState.AddTransition(Transition.E_FINISHATTACK, StateID.E_IDLE);
		atkState.AddTransition(Transition.E_LOSTHP, StateID.E_DAMAGED);

		Damaged dmgState = new Damaged(this);
		dmgState.AddTransition(Transition.E_NOHP, StateID.E_DEATH);
		dmgState.AddTransition(Transition.E_FINISHATTACK, StateID.E_IDLE);
		dmgState.AddTransition(Transition.E_LOSTHP, StateID.E_DAMAGED);

		Death deathState = new Death(this);
		enemyState = new FiniteStateMachine();
		enemyState.AddState(idleState);

		enemyState.AddState(atkState);
		enemyState.AddState(dmgState);
		enemyState.AddState(deathState);
	}

	// Use this for initialization
	public virtual void Initialize (int hp, int totalhp, int atk, int atkint) 
	{
		Hp = hp;
		totalHp = totalhp;
		attack = atk;
		attkInterval = atkint;
	}

	public void SetTransition(Transition t) 
	{ 
		enemyState.Transit(t); 
	}

	void FixedUpdate()
	{
		enemyState.currentState.Transit();
		enemyState.currentState.Update();
	}

	public virtual void PlayIdleAnim()
	{

	}

	public virtual void PlayDamageAnim()
	{
	}

	public virtual void PlayAttackAnim()
	{

	}

	public virtual bool IsAnimationComplete()
	{
		return true;
	}

	public virtual void PlayDeathAnim()
	{
	}
}
