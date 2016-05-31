using UnityEngine;
using System.Collections;

public class QuestComplete : MonoBehaviour {

	[SerializeField]UILabel				_questRewards;
	[SerializeField]UILabel				_questDesc;

	[SerializeField]UIButton			_completeButton;
	
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

		float offset = _questDesc.transform.localScale.y * 1.1f;

		float lineThickness = (questDesc.Split (new string[] {"\n"}, System.StringSplitOptions.None).Length) * offset;

		_questRewards.text = questRewards;
		
		_rootObject = rootObject;

		float rewardsPosY = _questDesc.transform.localPosition.y - lineThickness;

		_questRewards.transform.localPosition = new Vector3 (0, rewardsPosY);

		_tweenAlpha = gameObject.GetComponent<TweenAlpha> ();
		_tweenScale = gameObject.GetComponent<TweenScale> ();

//		float y = _questDesc.transform.localPosition.y - lineThickness;
		float buttonY = Mathf.Min (rewardsPosY - offset * 2, -75f);

		_completeButton.transform.localPosition = new Vector3 (0, buttonY);
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
