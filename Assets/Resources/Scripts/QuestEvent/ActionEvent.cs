using UnityEngine;
using System.Collections;

public class ActionEvent : MonoBehaviour {
	[SerializeField]TweenScale		_expandTween;
	[SerializeField]UILabel			_actionLabel;
	[SerializeField]UISprite		_expandedBG;


	private int				 		_actionId;
	private GameObject				_rootObject;
	private bool					_isExpanded;
	// Use this for initialization
	void Start () {
		
		_expandTween.Play (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int actionId, GameObject rootObject, string actionDesc){
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
	}

	void OnExpand()
	{
		_expandTween.Play (true);
	}

	void OnButtonClick()
	{
		_rootObject.SendMessage ("OnAction", _actionId);
	}
}
