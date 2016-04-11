using UnityEngine;
using System.Collections;

public class BattleStart : MonoBehaviour {
	
	[SerializeField]UILabel				_questDesc;
	[SerializeField]string				_desc;
	
	GameObject							_rootObject;
	
	[SerializeField]UITweener			_tween;
	[SerializeField]GameObject			_dialogObject;
	// Use this for initialization
	public void Initialize(GameObject rootObject, string monsterName)
	{
		_desc = _desc.Replace ("<playername>", PlayerProfile.Get ().playerName);
		_desc = _desc.Replace ("<enemyname>", monsterName);
		_questDesc.text = _desc;
		
		_rootObject = rootObject;
		
//		_tween = gameObject.GetComponent<UITweener> ();

		_dialogObject.transform.localScale = Vector3.zero;

//		GameObject obj1 = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Speedline_Fx") as GameObject);
//		obj1.transform.localScale = new Vector3 (100, 200);
//
//		obj1 = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/WarningExclaim_Fx") as GameObject);
		GameObject obj1 = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Caution_Fx") as GameObject);
		DestroyObject (obj1, obj1.particleSystem.duration);

		StartCoroutine (ShowDialog (obj1.particleSystem.duration));

		_dialogObject.transform.localScale = Vector3.zero;
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

	public IEnumerator ShowDialog(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_tween.Play (true);
	}
}
