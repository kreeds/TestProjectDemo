using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy {

	[SerializeField]TweenColor 				_tweenColor;
	[SerializeField]TweenPosition 			_tweenPosition;

	[SerializeField]Animation				_Anim;
	[SerializeField]UITexture				_texture;

	// Use this for initialization
	void Start () {
		
		InitializeStateMachine ();
		_texture.gameObject.transform.localPosition = new Vector3 (0, 211, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void PlayDamageAnim()
	{
		_tweenColor.to = Color.red;
		_tweenColor.Play (true);
		_Anim.clip = _Anim.GetClip("EnemyDamage");
		_Anim.Play();
	}
	
	public override void PlayAttackAnim()
	{
		_tweenColor.to = Color.green;
		_tweenColor.Play(true);
		_tweenPosition.Play (true);
	
	}
	
	public override bool IsAnimationComplete()
	{
		if (_tweenColor.isActiveAndEnabled || 
		    _tweenPosition.isActiveAndEnabled ||
		    _Anim.isPlaying
		    ) {
			return false;
		}
		return true;
	}

	public override void PlayDeathAnim()
	{
		_Anim.clip = _Anim.GetClip("EnemyDeath");
		_Anim.Play();
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
