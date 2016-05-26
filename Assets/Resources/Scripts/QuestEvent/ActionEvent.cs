using UnityEngine;
using System.Collections;

public class ActionEvent : MonoBehaviour {
	[SerializeField]TweenScale		_expandTween;
	[SerializeField]TweenScale		_expandFill;

	[SerializeField]UILabel			_actionLabel;

	[SerializeField]UISprite		_expandedBG;
	[SerializeField]UISprite		_frameBG;
	[SerializeField]UISprite		_fillBG;

	[SerializeField]UISprite		_energyIcon;
	[SerializeField]UILabel			_costLabel;

	[SerializeField]UISprite		_baseBG;

	[SerializeField]BoxCollider		_buttonCollider;

	[SerializeField]TweenColor		_alphaTween;
	[SerializeField]TweenScale		_scaleTween;

	private int				 		_actionId;
	private GameObject				_rootObject;
	private bool					_isExpanded;

	private int						_requiredAmt;
	private int						_progressAmt;

	public Vector3					expandedCenter;

	Vector3					nextScale;
	float 					fillWidth;

	// Use this for initialization
	void Start () {
		
		_expandTween.Play (false);
		_alphaTween.Play (false);
		_scaleTween.Play (false);
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
		scale.x = (textScale.x * _actionLabel.transform.localScale.x) + 180f;

		expandedCenter = transform.localPosition;
//		expandedCenter.x += scale.x / 4;

		_buttonCollider.size = scale;
//		if (scale.x < 460)
//			scale.x = 460;
		_expandedBG.transform.localScale = scale;

		scale.y = _frameBG.transform.localScale.y;

		_actionLabel.transform.localPosition = new Vector3 (-scale.x/2 + 40f, 0, -30f);
		_fillBG.transform.localPosition = new Vector3 (-scale.x/2, 0);

		_costLabel.transform.localPosition = new Vector3 (scale.x / 2 - 40f, 0, -12f);
		_costLabel.text = required.ToString ();

		_energyIcon.transform.localPosition = new Vector3 (scale.x / 2 - 70f, 0);

		_frameBG.transform.localScale = scale;

//		_progressGauge.Init (0, required*10);

//		_progressBar.sliderValue = 0;
		_requiredAmt = required;
		_progressAmt = 0;

		_isExpanded = false;

		fillWidth = scale.x;

		_expandFill.from = scale;
		_expandFill.from.x = 0;

 		_fillBG.transform.localScale = Vector3.zero;
	}

	void OnExpand()
	{
		_baseBG.enabled = _isExpanded;
		_isExpanded = !_isExpanded;
		_expandTween.Play (_isExpanded);
//		_alphaTween.Play (_isExpanded);
//		_scaleTween.Play (_isExpanded);
		_rootObject.SendMessage ("OnExpandAction", _actionId);
	}

	public void Close()
	{
		if (_isExpanded == false)
			return;

		_baseBG.enabled = true;
		_isExpanded = false;
		_expandTween.Play (_isExpanded);
	}

	void OnExpandFinish()
	{
		_expandedBG.enabled = false;
	}

	void OnFillFinish()
	{
		_expandFill.from = _expandFill.to;
		_expandFill.Reset ();

		if (_progressAmt >= _requiredAmt) {
			_rootObject.SendMessage ("OnAction", _actionId);
			Destroy (gameObject);
		}
	}

	void OnButtonClick()
	{
		float newAmount = (float)++_progressAmt;

		_costLabel.text = (_requiredAmt-_progressAmt).ToString ();

		_expandFill.to = _expandFill.from;
		_expandFill.to.x = fillWidth * newAmount / _requiredAmt;

		_expandFill.Play (true);
	}
}
