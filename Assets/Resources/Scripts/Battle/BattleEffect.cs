using UnityEngine;
using System.Collections;

public class BattleEffect : MonoBehaviour {
	[SerializeField]LAppModelProxy l2dInterface;

	private GameObject _rootObject;

	// Use this for initialization
	void Start () {

		//gameObject.animation.Play ();
		l2dInterface.GetModel ().StopBasicMotion (true);
		l2dInterface.GetModel ().StartMotion ("special", 0, LAppDefine.PRIORITY_NORMAL);
	}
	
	// Update is called once per frame
	void Update () {
		if (animation.isPlaying == false) {
			_rootObject.SendMessage("OnEffectFinish");
			Destroy(gameObject);
		}
	}

	public void Initialize(GameObject rootObject)
	{	
		l2dInterface.GetModel ().StartMotion ("special", 0, LAppDefine.PRIORITY_NORMAL);
//		l2dInterface.PlayAttackAnim ();
		_rootObject = rootObject;
	}
}
