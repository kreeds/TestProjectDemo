using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int Hp;
	public int totalHp;
	public int attack;
	public int attkInterval;

	FiniteStateMachine enemyState;

	public enum STATE
	{
		IDLE,
		ATTACK,
		DEATH,
		TOTAL
	};	

	STATE nextState;
	STATE curState;

	public STATE EnemyNextState
	{
		get{	return nextState; }
		set	{	nextState = value;	}
	}
	// Use this for initialization
	public void Initialize (int hp, int totalhp, int atk, int atkint) 
	{
		Hp = hp;
		totalHp = totalhp;
		attack = atk;
		attkInterval = atkint;


	}

	// Update is called once per frame
	void Update () 
	{

	}

	#region StateMethods
	void Idle()
	{
		Debug.Log("Enemy Idling");
	}
	void NormalAttack()
	{
		Debug.Log("Attack");
		// Play Attack Animation and Effects
	}
	void Death()
	{
		Debug.Log("Death");
	}
	#endregion
}
