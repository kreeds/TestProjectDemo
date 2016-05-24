using UnityEngine;
using System.Collections;

public class ArmoryPopup : MonoBehaviour {
	[SerializeField]UISprite			_backgroundSprite;
	[SerializeField]TweenAlpha			_tweenAlpha;
	[SerializeField]TweenScale			_tweenScale;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int id, GameObject rootObject){
		_backgroundSprite.spriteName = "UI_Armory_WeaponLookUp_00" + (id + 1).ToString ();

	}

	void OnClick(){
		Dismiss ();
	}

	void OnClose(){
		Dismiss ();
	}

	void Dismiss(){
		_tweenAlpha.from = 0;
		_tweenAlpha.Play (false);
		_tweenScale.Play (false);
	}

	void OnAnimationComplete(){
		if (_tweenScale.direction != AnimationOrTween.Direction.Forward)
		Destroy (gameObject);
	}
}
