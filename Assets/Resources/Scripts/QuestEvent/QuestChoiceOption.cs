using UnityEngine;
using System.Collections;

public class QuestChoiceOption : MonoBehaviour {

	private GameObject 				_rootObject;
	private int						_choiceIndex;

	[SerializeField]UILabel			_buttonText;
	[SerializeField]UILabel			_energyText;
	[SerializeField]UISprite		_iconSprite;

	public void Initialize(GameObject rootObject, int choiceIndex, int cost, string buttonText){
		if (cost < 0) {
			_iconSprite.spriteName = "icon03";
			_iconSprite.transform.localScale = new Vector3(45f, 45f);

//			_iconSprite.gameObject.AddComponent <TweenRotation>();
//			TweenRotation rot = _iconSprite.GetComponent<TweenRotation>();
//			rot.to = new Vector3 (0, 0, -60f);
//			rot.from = new Vector3 (0, 0, -10f);
//
//			rot.style = UITweener.Style.PingPong;
//			rot.duration = 1f;
			cost = -cost;
		} else {
			_iconSprite.spriteName = "icon00";
		}

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
