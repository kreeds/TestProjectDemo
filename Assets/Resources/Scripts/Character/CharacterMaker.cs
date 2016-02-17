using UnityEngine;
using System.Collections;

public class CharacterMaker : MonoBehaviour {

	[SerializeField]LAppModelProxy 		live2DModel;
	[SerializeField]UIInput				nameInput;

	[SerializeField]UISprite			clothesBG;
	[SerializeField]UISprite			hairBG;

	public enum CustomizeState
	{
		Hair,
		Clothes
	}

	private CustomizeState				currentState;

	// Use this for initialization
	void Start () {
		currentState = CustomizeState.Hair;
		nameInput.label.text = "Gumi";
		OnHairButton ();
		live2DModel.PlayIdleAnim ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SelectHairLeft(){
		live2DModel.ChangeHair (false);
	}

	void SelectHairRight(){
		live2DModel.ChangeHair (true);
	}

	void SelectClothesLeft(){
		live2DModel.ChangeClothes (false);
	}
	
	void SelectClothesRight(){
		live2DModel.ChangeClothes (true);
	}

	void SaveSettingsAndContinue(){
		live2DModel.SaveProfile ();
		PlayerProfile.Get ().playerName = nameInput.text;
		Application.LoadLevel ("BattleScene");
		//go to next scene
	}

	void OnClothesButton(){
		currentState = CustomizeState.Clothes;
		hairBG.gameObject.SetActive (false);
		clothesBG.gameObject.SetActive (true);
	}

	void OnHairButton()
	{
		currentState = CustomizeState.Hair;
		hairBG.gameObject.SetActive (true);
		clothesBG.gameObject.SetActive (false);
	}

	void SelectLeft(){
		if (currentState == CustomizeState.Hair)
			live2DModel.ChangeHair (false);
		else
			live2DModel.ChangeClothes (false);
	}
	
	void SelectRight(){
		if (currentState == CustomizeState.Hair)
			live2DModel.ChangeHair (true);
		else
			live2DModel.ChangeClothes (true);
	}

	void OnPlayAnimation()
	{
		if (live2DModel.IsAnimationComplete ()) {
			live2DModel.GetModel ().StartRandomMotion ("", LAppDefine.PRIORITY_NORMAL);
		}
	}
}


