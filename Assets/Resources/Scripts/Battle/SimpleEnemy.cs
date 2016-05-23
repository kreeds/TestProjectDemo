using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy {

	public enum SimpleEnemyType
	{
		Mirror,
		Rat
	}

	[SerializeField]TweenColor 				_tweenColor;
	[SerializeField]TweenPosition 			_tweenPosition;

	[SerializeField]Animation				_Anim;
	[SerializeField]UITexture				_texture;
	[SerializeField]UITexture				_textureInvert;

	bool isAttacking = false;

	/// <summary>
	/// The interval time of changing shader
	/// </summary>
	[SerializeField]float					_intervalTime = 0.6f; 

	/// <summary>
	/// The life time of effect.
	/// </summary>
	[SerializeField]float					_lifeTIme = 0.6f;

	// Use this for initialization
	void Start () {
		
		InitializeStateMachine ();
		
		_texture.gameObject.transform.localPosition = new Vector3 (0, 211, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetType(SimpleEnemyType enemyType)
	{
		if (enemyType == SimpleEnemyType.Mirror) {
			_texture.mainTexture = Resources.Load ("Texture/LadyKnight_MirrorMon_004_perspective") as Texture;
			_textureInvert.mainTexture = Resources.Load ("Texture/LadyKnight_MirrorMon_004_perspective") as Texture;

			_texture.MakePixelPerfect ();
			_textureInvert.MakePixelPerfect ();
//			gameObject.name = "Mirror Monster";
		} else {
			_texture.mainTexture = Resources.Load ("Texture/Rat") as Texture;
			_textureInvert.mainTexture = Resources.Load ("Texture/Rat") as Texture; 

			_texture.transform.localScale = new Vector3(400, 450);
			_textureInvert.transform.localScale = new Vector3(400, 450);
//			gameObject.name = "Rat Monster";
		}
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
		isAttacking = true;
		StartCoroutine(BlinkInverted(_intervalTime, _lifeTIme));
		_tweenPosition.Play (true);
	 
	}
	
	public override bool IsAnimationComplete()
	{
		if (_tweenColor.isActiveAndEnabled || 
		    _tweenPosition.isActiveAndEnabled ||
		    _Anim.isPlaying || isAttacking
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

	IEnumerator BlinkInverted(float interval, float lifeTime)
	{
		float timeElapsed = 0;
		float startTime = Time.time;
		bool changed = false;
		while(timeElapsed < lifeTime)
		{
		 	timeElapsed = Time.time - startTime;
		 	changed = !changed;

		 	_texture.enabled = !changed;
			_textureInvert.enabled = changed;

			yield return new WaitForSeconds(interval);
		}

		// Switch back to normal shader
		isAttacking = false;
		_texture.enabled = true;
		_textureInvert.enabled = false;
	}
}
