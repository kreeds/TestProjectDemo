using UnityEngine;
using System.Collections;

public class QuestChoiceOption : MonoBehaviour {

	private GameObject 				_rootObject;
	private int						_choiceIndex;

	[SerializeField]UILabel			_buttonText;

	public void Initialize(GameObject rootObject, int choiceIndex, string buttonText){
		_rootObject = rootObject;
		_choiceIndex = choiceIndex;

		_buttonText.text = buttonText;
	}

	void OnClick()
	{
		_rootObject.SendMessage ("OnChoiceSelected", _choiceIndex);
	}
}
