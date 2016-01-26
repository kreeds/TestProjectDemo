using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy {

	[SerializeField]TweenColor 				_tweenColor;
	[SerializeField]TweenPosition 			_tweenPosition;


	// Use this for initialization
	void Start () {
	
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
		return true;
	}
}
