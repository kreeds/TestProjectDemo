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

	[SerializeField]GameObject fgObj;

	GameObject	rootObject;

	SoundService m_soundService;



	float counter;

	void Awake()
	{
		m_soundService = Service.Get<SoundService>();
	}
	void Start () {
		model.GetModel ().StopBasicMotion (true);
		model.PlayTransformAnim ();
		model.gameObject.SetActive (false);

		StartCoroutine (EnableCollider (1.2f));

		StartCoroutine(Utility.DelayInSeconds(0.2f, (res)=>{m_soundService.PlaySound(Resources.Load("Sound/compactopen") as AudioClip, false);} ));

//		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Bg_Fx") as GameObject);
//		obj.transform.localPosition = new Vector3 (0, 0, 2f);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		collider.enabled = false;

		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (0, -140, -10f);
		m_soundService.PlayMusic(Resources.Load("Music/transform") as AudioClip, false);

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
		//m_soundService.PlaySound(Resources.Load("Sound/compactopen_jingle") as AudioClip, false);

//		fgObj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Fg_Fx") as GameObject);
//		fgObj.transform.localPosition = new Vector3 (0, -140f, -2f);
	}

	IEnumerator FinishAnim(float seconds)
	{
		yield return new WaitForSeconds(seconds);
//		if (Application.loadedLevelName != "TestTransform")
		Application.LoadLevel("BattleScene");
		m_soundService.StopAll();
		//Service.Get<HUDService> ().ChangeScene ("BattleScene");
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
