using UnityEngine;
using System.Collections;


public class Idle : FSMState
{
	BattleManager bmgr;
	Enemy enemy;

	int prevhp;

	public Idle(Enemy emy)
	{
		bmgr = BattleManager.Get();
		enemy = emy;
		prevhp = emy.Hp;	
		stateID = StateID.E_IDLE;
	}

	public override void Init()
	{
		// Play Idle Animation
		enemy.L2dModel.PlayIdleAnim();
	}

	public override void Transit()
	{
		// Attack Player
		if(bmgr.getGestureState == BattleManager.GestureState.END)
			enemy.SetTransition(Transition.E_FAILGESTURE);

		// Death
		if(enemy.Hp <= 0)
			enemy.SetTransition(Transition.E_NOHP);

		// Hp Lost
		if(enemy.Hp < prevhp)
		{
			prevhp = enemy.Hp;
			enemy.SetTransition(Transition.E_LOSTHP);
		}
	}

	public override void Execute()
	{
		
	}
}

public class Damaged: FSMState
{
	Enemy enemy;
	public Damaged(Enemy emy)
	{
		enemy = emy;
		stateID = StateID.E_DAMAGED;
	}

	public override void Init()
	{
		// Play Attack Animation
		enemy.L2dModel.PlayDamageAnim();
	}
	public override void Transit()
	{
		
	}

	public override void Execute()
	{
		
	}


}

public class Attack : FSMState
{
	Enemy enemy;
	public Attack(Enemy emy)
	{
		stateID = StateID.E_ATTACK;
		enemy = emy;
	}

	public override void Transit()
	{
		
	}

	public override void Execute()
	{
		// Play Attack Animation
		enemy.L2dModel.PlayAttackAnim();
	}
}


public class Death : FSMState
{
	Enemy enemy;
	public Death(Enemy emy)
	{
		stateID = StateID.E_DEATH;
		enemy = emy;
	}

	public override void Transit()
	{
		
	}

	public override void Execute()
	{
		// Play Death Animation

	}
}

public class Enemy : MonoBehaviour {

	public int Hp;
	public int totalHp;
	public int attack;
	public int attkInterval;

	FiniteStateMachine enemyState;


	LAppModelProxy l2dInterface;
	public LAppModelProxy L2dModel
	{
		get{return l2dInterface;}
	}

	void Awake()
	{
		l2dInterface = gameObject.GetComponent<LAppModelProxy>();
	}

	void Start()
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

		Death deathState = new Death(this);
		enemyState = new FiniteStateMachine();
		enemyState.AddState(idleState);

		enemyState.AddState(atkState);
		enemyState.AddState(dmgState);
		enemyState.AddState(deathState);
	}

	// Use this for initialization
	public void Initialize (int hp, int totalhp, int atk, int atkint) 
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
		enemyState.currentState.Execute();
	}

}
