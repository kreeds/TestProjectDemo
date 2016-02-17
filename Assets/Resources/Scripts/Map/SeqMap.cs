using UnityEngine;
using System.Collections;

public class SeqMap : MonoBehaviour {

	[SerializeField]LAppModelProxy l2dInterface;
	[SerializeField]SceneFadeInOut fader;


	// Use this for initialization
	void Start () {
		l2dInterface.LoadProfile ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClothingClick()
	{
		fader.ChangeScene("CharaScene");
	}
}
