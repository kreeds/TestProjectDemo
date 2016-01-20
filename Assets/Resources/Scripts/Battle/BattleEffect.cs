using UnityEngine;
using System.Collections;

public class BattleEffect : MonoBehaviour {
	[SerializeField]LAppModelProxy l2dInterface;

	// Use this for initialization
	void Start () {
		
		l2dInterface.GetModel ().StartMotion ("", 0, LAppDefine.PRIORITY_NORMAL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize()
	{
		l2dInterface.PlayAttackAnim ();
	}
}
