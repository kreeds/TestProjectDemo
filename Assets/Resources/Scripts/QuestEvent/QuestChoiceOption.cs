using UnityEngine;
using System.Collections;

public class QuestChoiceOption : MonoBehaviour {

	private GameObject 				_rootObject;
	private int						_choiceIndex;

	[SerializeField]UILabel			_buttonText;
	[SerializeField]UILabel			_energyText;

	public void Initialize(GameObject rootObject, int choiceIndex, int cost, string buttonText){
		_rootObject = rootObject;
		_choiceIndex = choiceIndex;

		_buttonText.text = buttonText;
		_energyText.text = cost.ToString ();
	}

	void OnClick()
	{
		_rootObject.SendMessage ("OnChoiceSelected", _choiceIndex);
	}
}
