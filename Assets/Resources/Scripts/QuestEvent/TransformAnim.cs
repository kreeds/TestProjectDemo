using UnityEngine;
using System.Collections;

public class TransformAnim : MonoBehaviour {

	[SerializeField]Collider collider;

	[SerializeField]UISprite spriteAnim;
	[SerializeField]UILabel clickMessage;
	// Use this for initialization

	GameObject	rootObject;

	float counter;
	void Start () {
		StartCoroutine (EnableCollider (0.2f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		collider.enabled = false;

		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (0, 0, -10f);
		StartCoroutine (FinishAnim (2f));
		obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/WhiteFader_FX") as GameObject);
	}

	IEnumerator EnableCollider(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		clickMessage.enabled = true;
		collider.enabled = true;
	}

	IEnumerator FinishAnim(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Service.Get<HUDService> ().ChangeScene ("BattleScene");
		Destroy (gameObject);
	}
}
