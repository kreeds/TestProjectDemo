using UnityEngine;
using System.Collections;

public class TransformAnim : MonoBehaviour {

	[SerializeField]Collider collider;

	[SerializeField]float fadeOutTime;
	[SerializeField]float finishTime;
	[SerializeField]float modelAppearTime;

	[SerializeField]UILabel clickMessage;

	[SerializeField]LAppModelProxy model;
	// Use this for initialization

	GameObject fgObj;

	GameObject	rootObject;

	float counter;
	void Start () {
		model.GetModel ().StopBasicMotion (true);
		model.PlayIdleFrontFaceAnim ();
		model.gameObject.SetActive (false);

		StartCoroutine (EnableCollider (1.2f));
		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Bg_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (0, 0, 2f);


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		collider.enabled = false;

		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (0, -140, -10f);

		Destroy (fgObj);

		StartCoroutine (FinishAnim (finishTime));
		StartCoroutine (ShowModel (modelAppearTime));
		StartCoroutine (WhiteOut (fadeOutTime));

		clickMessage.enabled = false;
	}

	IEnumerator EnableCollider(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		clickMessage.enabled = true;
		collider.enabled = true;
		fgObj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Fg_Fx") as GameObject);
		fgObj.transform.localPosition = new Vector3 (0, -140f, -2f);
	}

	IEnumerator FinishAnim(float seconds)
	{
		yield return new WaitForSeconds(seconds);
//		if (Application.loadedLevelName != "TestTransform")
			Application.LoadLevel("BattleScene");
//			Service.Get<HUDService> ().ChangeScene ("BattleScene");
//		else
//		Destroy (gameObject);
	}

	IEnumerator ShowModel(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		model.gameObject.SetActive (true);
	}

	IEnumerator WhiteOut(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/WhiteFader_FX") as GameObject);
		obj.transform.localScale = new Vector3 (2048, 2048);
	}
}
