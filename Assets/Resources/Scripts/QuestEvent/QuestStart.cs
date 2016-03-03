using UnityEngine;
using System.Collections;

public class QuestStart : MonoBehaviour {

	[SerializeField]UILabel				_questName;
	[SerializeField]UILabel				_questDesc;

	GameObject							_rootObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(GameObject rootObject, string questName, string questDesc)
	{
		_questDesc.text = questDesc;
		_questName.text = questName;

		_rootObject = rootObject;
	}

	void OnStart()
	{
		_rootObject.SendMessage ("BeginQuest");
		Destroy (gameObject);
	}

	void OnCancel()
	{
		Destroy (gameObject);
	}

}
