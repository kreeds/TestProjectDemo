using UnityEngine;
using System.Collections;

public class BondUp : MonoBehaviour {
	
	[SerializeField]UILabel					_descLabel;
	[SerializeField]TweenAlpha				_tweenAlpha;
	[SerializeField]TweenScale				_tweenScale;

	[SerializeField]UISprite				_friendIcon;
	
	GameObject								_rootObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	public void Initialize (GameObject rootObject, string name){
		_descLabel.text = _descLabel.text.Replace ("<name>", name);
		
		_rootObject = rootObject;

		if (name == "Margie")
			_friendIcon.spriteName = "Friendslist_UI_Thumbnail_03";
		else
			_friendIcon.spriteName = "Friendslist_UI_Thumbnail_05";
	}

	void OnConfirm()
	{
		_tweenAlpha.from = 0;
		_tweenScale.Play (false);
		_tweenAlpha.Play (false);
		_rootObject.SendMessage("OnQuestCompleteOk");
		
	}

	void OnAnimationComplete()
	{
		if (_tweenScale.direction == AnimationOrTween.Direction.Reverse)
			Destroy (gameObject);
	}
}
