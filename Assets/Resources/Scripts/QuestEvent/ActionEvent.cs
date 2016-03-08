using UnityEngine;
using System.Collections;

public class ActionEvent : MonoBehaviour {
	[SerializeField]TweenScale		_expandTween;
	[SerializeField]UILabel			_actionLabel;
	[SerializeField]UISprite		_expandedBG;
	[SerializeField]UISlider		_progressBar;

	private int				 		_actionId;
	private GameObject				_rootObject;
	private bool					_isExpanded;

	private int						_requiredAmt;
	private int						_progressAmt;
	// Use this for initialization
	void Start () {
		
		_expandTween.Play (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int actionId, int required, GameObject rootObject, string actionDesc){
		_rootObject = rootObject;

		_actionLabel.text = actionDesc;

		Vector2 textScale = _actionLabel.relativeSize;

		_actionId = actionId;

		_expandTween.Play (false);

		Vector3 scale = _expandedBG.transform.localScale;
		scale.x = textScale.x*_actionLabel.font.size;
		if (scale.x < 460)
			scale.x = 460;
		_expandedBG.transform.localScale = scale;

		_progressBar.sliderValue = 0;
		_requiredAmt = required;
		_progressAmt = 0;
	}

	void OnExpand()
	{
		_expandTween.Play (true);
	}

	void OnButtonClick()
	{
		float newAmount = (float)++_progressAmt;
		_progressBar.sliderValue = newAmount / _requiredAmt;
		if (_progressAmt >= _requiredAmt) {
			_rootObject.SendMessage ("OnAction", _actionId);
			Destroy (gameObject);
		}
	}
}
