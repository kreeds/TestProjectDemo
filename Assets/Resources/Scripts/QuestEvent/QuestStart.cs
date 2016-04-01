using UnityEngine;
using System.Collections;

public class QuestStart : MonoBehaviour {

	[SerializeField]UILabel				_questName;
	[SerializeField]UILabel				_questDesc;

	GameObject							_rootObject;

	UITweener							_tween;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(GameObject rootObject, string questName, string questDesc)
	{
		_questDesc.text = questDesc;
		_questName.text = questName;

		_rootObject = rootObject;

		_tween = gameObject.GetComponent<UITweener> ();
	}

	void OnStart()
	{
		_rootObject.SendMessage ("OnBeginQuest");
		Dismiss ();
	}

	void OnCancel()
	{
		_rootObject.SendMessage ("OnDidNotBeginQuest");
		Dismiss ();
	}

	void Dismiss()
	{
		_tween.Play (false);
	}

	void OnAnimationFinish()
	{
		if (_tween.direction != AnimationOrTween.Direction.Forward)
			Destroy (gameObject);
	}
}
