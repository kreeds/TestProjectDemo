using UnityEngine;
using System.Collections;

public class QuestComplete : MonoBehaviour {

	[SerializeField]UILabel				_questRewards;
	[SerializeField]UILabel				_questDesc;
	
	GameObject							_rootObject;

	TweenAlpha							_tweenAlpha;
	TweenScale							_tweenScale;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Initialize(GameObject rootObject, string questDesc, string questRewards)
	{
		_questDesc.text = questDesc;
		_questRewards.text = questRewards;
		
		_rootObject = rootObject;

		_tweenAlpha = gameObject.GetComponent<TweenAlpha> ();
		_tweenScale = gameObject.GetComponent<TweenScale> ();
	}
	
	void OnConfirm()
	{
//		Destroy (gameObject);
		_tweenAlpha.from = 0;
		_tweenScale.Play (false);
		_tweenAlpha.Play (false);
		_rootObject.SendMessage ("OnQuestCompleteOk");
	}

	void OnAnimationComplete()
	{
		Destroy (gameObject);
	}
}
