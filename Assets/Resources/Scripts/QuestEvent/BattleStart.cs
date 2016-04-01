using UnityEngine;
using System.Collections;

public class BattleStart : MonoBehaviour {
	
	[SerializeField]UILabel				_questDesc;
	[SerializeField]string				_desc;
	
	GameObject							_rootObject;
	
	UITweener							_tween;
	// Use this for initialization
	public void Initialize(GameObject rootObject, string monsterName)
	{
		_desc = _desc.Replace ("<playername>", PlayerProfile.Get ().playerName);
		_desc = _desc.Replace ("<enemyname>", monsterName);
		_questDesc.text = _desc;
		
		_rootObject = rootObject;
		
		_tween = gameObject.GetComponent<UITweener> ();
	}
	
	void OnStart()
	{
		_rootObject.SendMessage ("OnBeginBattle");
		Dismiss ();
	}
	
	void OnCancel()
	{
//		_rootObject.SendMessage ("OnDidNotBeginQuest");
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
