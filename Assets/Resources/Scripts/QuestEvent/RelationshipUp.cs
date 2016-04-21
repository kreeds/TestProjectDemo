using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipUp : MonoBehaviour {

	[SerializeField]LAppModelProxy 			_model;

	[SerializeField]UILabel					_nameLabel;
	[SerializeField]UILabel					_relLvLabel;
	[SerializeField]UILabel					_friendDescLabel;

	[SerializeField]UISlider				_relLvSlider;

	[SerializeField]Collider				_skipCollider;
	[SerializeField]UIButton				_okButton;

	[SerializeField]TweenAlpha				_tweenAlpha;
	[SerializeField]TweenScale				_tweenScale;

	GameObject								_rootObject;

	float									_currentValue;

	float									_difference;

	float									_finalValue;
	int										_finalLevel;

	int										_bondLevel;

	float counter;
	static float								_levelCost = 5;

	bool 									_isAnimating;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_isAnimating && _difference > 0) {
			float decrement = Time.deltaTime * 0.4f;
			_difference -= decrement;
			_currentValue += decrement;
			if (_currentValue >= 1) {
				_currentValue = 0;
				_bondLevel += 1;
				SetFriendShipLevel (_bondLevel);
			}

			_relLvSlider.sliderValue = _currentValue;
		} else {
			_skipCollider.enabled = false;
			_okButton.gameObject.SetActive (true);
		}
	}

	void SetFriendShipLevel(int level){
		switch (level) {
		case 0:
			_friendDescLabel.text = "Acquaintance";
			break;
			
		case 1:
			_friendDescLabel.text = "Friend";
			break;
			
		case 2:
		case 3:
			_friendDescLabel.text = "Good Friend!";
			break;
		}
		_relLvLabel.text = level.ToString ();
	}

	public void Initialize (GameObject rootObject, int hairIndex, int clothesIndex, int previous, int current, string name){
		_nameLabel.text = name;
		_model.SetClothes (clothesIndex);
		_model.SetHair (hairIndex);

		_bondLevel = (int)(previous / _levelCost);

		_finalLevel = (int)(current / _levelCost);

		_difference = current / _levelCost - previous / _levelCost;

		_currentValue =  (previous % _levelCost) /_levelCost;

		_finalValue = (current % _levelCost)  / _levelCost; 

		_relLvSlider.sliderValue = _currentValue;

		SetFriendShipLevel (_bondLevel);

		_rootObject = rootObject;

		_isAnimating = true;

		_model.PlayIdleAnim ();
	}

	void OnSkip()
	{
		SetFriendShipLevel (_finalLevel);
		_relLvSlider.sliderValue = _finalValue;

		_skipCollider.enabled = false;
		
		_okButton.gameObject.SetActive (true);
		_isAnimating = false;
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
