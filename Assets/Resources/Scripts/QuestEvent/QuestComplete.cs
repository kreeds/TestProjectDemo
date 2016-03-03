using UnityEngine;
using System.Collections;

public class QuestComplete : MonoBehaviour {

	[SerializeField]UILabel				_questName;
	[SerializeField]UILabel				_questDesc;
	
	GameObject							_rootObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Initialize(GameObject rootObject, string questDesc)
	{
		_questDesc.text = questDesc;
		
		_rootObject = rootObject;
	}
	
	void OnConfirm()
	{
		Destroy (gameObject);
		_rootObject.SendMessage ("OnQuestCompleteOk");
	}
}
