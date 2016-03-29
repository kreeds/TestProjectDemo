using UnityEngine;
using System.Collections;

public class QuestProgress : MonoBehaviour {

	[SerializeField]UISprite progressSprite;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetProgress(float current, float max){
		float progressRatio = current / max;

		Vector3 localScale = progressSprite.transform.localScale;
		localScale.x = (63)*(5f*progressRatio);
		progressSprite.transform.localScale = localScale;
	}

	public void SetProgress(float ratio){
		float progressRatio = ratio;
		
		Rect rect = progressSprite.GetAtlasSprite ().outer;

		Vector3 localScale = progressSprite.transform.localScale;
		localScale.x = rect.width*(5f*progressRatio);
		progressSprite.transform.localScale = localScale;
	}
}
