using UnityEngine;
using System.Collections;

public class CharacterMaker : MonoBehaviour {

	[SerializeField]LAppModelProxy live2DModel;

	// Use this for initialization
	void Start () {
	
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
}
