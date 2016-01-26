using UnityEngine;
using System.Collections;

public class L2DEnemy : Enemy {
	
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
		InitializeStateMachine ();
	}
	
	void FixedUpdate()
	{
		enemyState.currentState.Transit();
		enemyState.currentState.Update();
	}
	
	public override void PlayIdleAnim()
	{
		l2dInterface.PlayIdleAnim ();
	}
	
	public override void PlayDamageAnim()
	{
		l2dInterface.PlayDamageAnim ();
	}
	
	public override void PlayAttackAnim()
	{
		l2dInterface.PlayAttackAnim ();
	}

	public override bool IsAnimationComplete()
	{
		return l2dInterface.IsAnimationComplete ();
	}
}
