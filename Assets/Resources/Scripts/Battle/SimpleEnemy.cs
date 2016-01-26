using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy {

	[SerializeField]TweenColor 				_tweenColor;
	[SerializeField]TweenPosition 			_tweenPosition;


	// Use this for initialization
	void Start () {
		
		InitializeStateMachine ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void PlayDamageAnim()
	{
		_tweenColor.Play (true);
	}
	
	public override void PlayAttackAnim()
	{
		_tweenPosition.Play (true);
	}
	
	public override bool IsAnimationComplete()
	{
		if (_tweenColor.isActiveAndEnabled || _tweenPosition.isActiveAndEnabled) {
			return false;
		}
		return true;
	}

	void OnDamagedAnim()
	{
		if (_tweenColor.direction == AnimationOrTween.Direction.Forward) {
			_tweenColor.Play(false);
		}
	}

	void OnAttackAnim()
	{
		if (_tweenPosition.direction == AnimationOrTween.Direction.Forward) {
			_tweenPosition.Play(false);
		}
	}
}
