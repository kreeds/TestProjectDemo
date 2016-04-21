using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GeneralPopUp : MonoBehaviour {

	[SerializeField]UILabel PopUpText;

	UnityAction[] func;

	void Awake()
	{
		func = new UnityAction[2];
	}


	GameObject createButton(Vector3 pos)
	{
		GameObject obj = Instantiate(Resources.Load("Prefabs/GeneralPopUpButton")) as GameObject;
		obj.transform.SetParent(transform, false);
		obj.transform.localPosition = pos;
		return obj;
	}
	public void Init(string text, string btnLabel, GameObject target, string funcName )
	{
		if(PopUpText != null)
			PopUpText.text = text;

		// Create Button
		GameObject obj = createButton(new Vector3(0,-90, 0));
		UILabel label = obj.GetComponentInChildren<UILabel>();
		if(label != null)
		{
			label.text = btnLabel;
		}
		UIButtonMessage btn = obj.GetComponent<UIButtonMessage>();
		btn.functionName = funcName;
		btn.target = target;

	}

	public void Init(string text, string btnLabel, string btnLabel2, UnityAction func1, UnityAction func2)
	{
		if(PopUpText != null)
			PopUpText.text = text;

		GameObject obj = createButton(new Vector3(98.6f,-103.0f, 0));
		UILabel label = obj.GetComponentInChildren<UILabel>();
		if(label != null)
		{
			label.text = btnLabel;
		}
		UISprite sprite = obj.GetComponentInChildren<UISprite>();
		if(sprite != null)
			sprite.color = Color.green;

		func[0] = func1;
		func[1] = func2;


		UIButtonMessage btn = obj.GetComponent<UIButtonMessage>();
		btn.functionName = "Function1";
		btn.target = this.gameObject;


		obj = createButton(new Vector3(-98.6f,-103.0f, 0));
		label = obj.GetComponentInChildren<UILabel>();

		if(label != null)
		{
			label.text = btnLabel2;
		}

	 	sprite = obj.GetComponentInChildren<UISprite>();
		if(sprite != null)
			sprite.color = Color.red;

		UIButtonMessage btn1 = obj.GetComponent<UIButtonMessage>();
		btn1.functionName = "Function2";
		btn1.target = this.gameObject;
	}

	#region button message receiver

	void Function1()
	{
		if(func[0] != null)
			func[0]();
			
		DestroyObject(this.gameObject);
	}

	void Function2()
	{
		if(func[1] != null)
			func[1]();
		// Close effect before calling
		Destroy(this.gameObject);
	}
	#endregion

}
