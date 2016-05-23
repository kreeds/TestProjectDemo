using UnityEngine;
using System.Collections;

public class QuestStart : MonoBehaviour {

	[SerializeField]UILabel				_questName;
	[SerializeField]UILabel				_questDesc;
	[SerializeField]UILabel				_questRewards;

	GameObject							_rootObject;

	TweenAlpha							_tweenAlpha;
	TweenScale							_tweenScale;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(GameObject rootObject, string questName, string questDesc, string rewards)
	{
		_questDesc.text = questDesc;
		_questName.text = questName;

		_rootObject = rootObject;

		_tweenAlpha = gameObject.GetComponent<TweenAlpha> ();
		_tweenScale = gameObject.GetComponent<TweenScale> ();

		_questRewards.text = rewards;
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
		_tweenAlpha.from = 0;
		_tweenAlpha.Play (false);
		_tweenScale.Play (false);
	}

	void OnAnimationFinish()
	{
		if (_tweenAlpha.direction != AnimationOrTween.Direction.Forward)
			Destroy (gameObject);
	}
}
