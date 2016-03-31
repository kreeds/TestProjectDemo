using UnityEngine;
using System.Collections;

public class ActionEvent : MonoBehaviour {
	[SerializeField]TweenScale		_expandTween;
	[SerializeField]UILabel			_actionLabel;
	[SerializeField]UISprite		_expandedBG;
//	[SerializeField]UISlider		_progressBar;
	[SerializeField]UIGauge			_progressGauge;
	[SerializeField]BoxCollider		_buttonCollider;

	private int				 		_actionId;
	private GameObject				_rootObject;
	private bool					_isExpanded;

	private int						_requiredAmt;
	private int						_progressAmt;

	public Vector3					expandedCenter;
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
		scale.x = (textScale.x * _actionLabel.transform.localScale.x) + 242;

		expandedCenter = transform.localPosition;
//		expandedCenter.x += scale.x / 4;

		_buttonCollider.size = scale;
//		if (scale.x < 460)
//			scale.x = 460;
		_expandedBG.transform.localScale = scale;

		_progressGauge.Init (0, required*10);

//		_progressBar.sliderValue = 0;
		_requiredAmt = required;
		_progressAmt = 0;

		_isExpanded = false;
	}

	void OnExpand()
	{
		_isExpanded = !_isExpanded;
		_expandTween.Play (_isExpanded);
		_rootObject.SendMessage ("OnExpandAction", _actionId);
	}

	void OnButtonClick()
	{
		float newAmount = (float)++_progressAmt;
		_progressGauge.reduce (-10);
		if (_progressAmt >= _requiredAmt) {
			_rootObject.SendMessage ("OnAction", _actionId);
			Destroy (gameObject);
		}
	}
}
