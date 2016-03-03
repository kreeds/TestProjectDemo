using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelationshipUp : MonoBehaviour {

	[SerializeField]LAppModelProxy 			_model;

	[SerializeField]UILabel					_nameLabel;
	[SerializeField]UILabel					_relLvLabel;

	[SerializeField]UISlider				_relLvSlider;

	GameObject								_rootObject;

	float 									_targetValue;
	float									_currentValue;

	int 									_target;
	int										_current;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_currentValue < _targetValue) {
	
			_currentValue += (Time.deltaTime * 0.1f);
			_relLvSlider.sliderValue = _currentValue;
		}
	}

	public void Initialize (GameObject rootObject, int hairIndex, int clothesIndex, int previous, int current, string name){
		_nameLabel.text = name;
		_model.SetClothes (clothesIndex);
		_model.SetHair (hairIndex);

		if ((current / 100 - previous / 100) >= 1) {
			_targetValue = 1.0f;
		} else {
			_targetValue = (current % 100) * 0.01f;
		}

		_currentValue =  (previous % 100) * 0.01f;

		_relLvSlider.sliderValue = _currentValue;

		_relLvLabel.text = (previous / 100).ToString ();

		_rootObject = rootObject;
	}
}
