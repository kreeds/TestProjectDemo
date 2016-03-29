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
		StartCoroutine (EnableCollider (1f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		collider.enabled = false;

		GameObject obj = NGUITools.AddChild (gameObject, Resources.Load ("Prefabs/FX/Transformation_Fx") as GameObject);
		obj.transform.localPosition = new Vector3 (0, 0, -10f);
		StartCoroutine (FinishAnim (2f));
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
		Service.Get<HUDService> ().ChangeScene ("EventScene");
		Destroy (gameObject);
	}
}
