using UnityEngine;
using System.Collections;


public class Idle : FSMState
{
	BattleManager bmgr;
	Enemy enemy;

	bool attacked;
	bool damaged;
	int prevhp;

	public Idle(Enemy emy)
	{
		bmgr = BattleManager.Get();
		enemy = emy;
		prevhp = emy.Hp;	
		stateID = StateID.E_IDLE;
		attacked = false;
	}

	public override void OnEnter()
	{
		Debug.Log(enemy.name + ": Entering Idle State");
		// Play Idle Animation
//		enemy.L2dModel.PlayIdleAnim();
		enemy.PlayIdleAnim ();
	}

	public override void Transit()
	{
		// Death
		if(enemy.Hp <= 0)
			enemy.SetTransition(Transition.E_NOHP);

		// Hp Lost
		if(enemy.Hp < prevhp)
		{
			prevhp = enemy.Hp;
			enemy.SetTransition(Transition.E_LOSTHP);
		}


		if((bmgr.currentGestureState == BattleManager.GestureState.START ||
			bmgr.currentGestureState == BattleManager.GestureState.SHOWING) && attacked)
			attacked = false;

		// Attack Player
		if(bmgr.currentGestureState == BattleManager.GestureState.END && !attacked)
		{
			enemy.SetTransition(Transition.E_FAILGESTURE);
			attacked = true;
		}

	
	}

	public override void Update()
	{
		
	}

	public override void OnExit()
	{
		Debug.Log(enemy.name + ": Exiting Idle State");
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
		Debug.Log(enemy.name + ": Entering Damaged State");
		// Play Attack Animation
//		enemy.L2dModel.PlayDamageAnim();
		enemy.PlayDamageAnim ();
	}

	public override void Transit()
	{
		if(enemy.IsAnimationComplete ())
		{
			if(enemy.Hp > 0)
				enemy.SetTransition(Transition.E_FINISHATTACK); // Back to Idle
			else
				enemy.SetTransition(Transition.E_NOHP); // Transit to Death
		}
		else if(enemy.Hp < hp)
		{
			hp = enemy.Hp;
			enemy.SetTransition(Transition.E_LOSTHP); // Transit to Death

		}
	}

	public override void Update()
	{
		
	}

	public override void OnExit()
	{
		Debug.Log(enemy.name + ": Exiting Damaged State");
	}


}

public class Attack : FSMState
{
	Enemy enemy;
	PlayerManager pmgr;
	BattleManager bmgr;

	int hp = 0;
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

		Debug.Log(enemy.name + ": Entering Attack State");
		// Play Attack Animation
//		enemy.L2dModel.PlayAttackAnim();
		enemy.PlayAttackAnim ();
	}

	public override void Transit()
	{

		if(enemy.IsAnimationComplete())
		{
			enemy.SetTransition(Transition.E_FINISHATTACK);

		}
	}

	public override void Update()
	{
	}
	public override void OnExit()
	{
		pmgr.Damaged(enemy.attack);	
		bmgr.ReduceGauge();
		// Restart Gesture State
		bmgr.currentGestureState = BattleManager.GestureState.START;
		Debug.Log("Exiting Attack State");
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
		
		Debug.Log(enemy.name + ": Entering Death State");
		// Play Attack Animation
		//		enemy.L2dModel.PlayAttackAnim();
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
		}
	}
}

public class Enemy : MonoBehaviour {

	public int Hp;
	public int totalHp;
	public int attack;
	public int attkInterval;

	protected FiniteStateMachine enemyState;


//	LAppModelProxy l2dInterface;
//	public LAppModelProxy L2dModel
//	{
//		get{return l2dInterface;}
//	}
//
//	void Awake()
//	{
//		l2dInterface = gameObject.GetComponent<LAppModelProxy>();
//	}

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
