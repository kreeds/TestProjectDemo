using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int Hp;
	public int totalHp;
	public int attack;
	public int attkInterval;

	public enum EnemyState
	{
		IDLE,
		ATTACK
	};

	EnemyState state;

	public EnemyState GetState()
	{
		return state;
	}
	// Use this for initialization
	public void Initialize (int hp, int totalhp, int atk, int atkint) 
	{
		Hp = hp;
		totalHp = totalhp;
		attack = atk;
		attkInterval = atkint;

		state = EnemyState.IDLE;
	}

	// Update is called once per frame
	void Update () 
	{
		switch(state)
		{
			case EnemyState.IDLE:
			break;
			case EnemyState.ATTACK:
				StartCoroutine(NormalAttack());
			break;
			default:
			break;

		}
	}

	IEnumerator NormalAttack()
	{
		yield return null;
	}
}
