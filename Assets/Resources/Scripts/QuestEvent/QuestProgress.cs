using UnityEngine;
using System.Collections;

public class QuestProgress : MonoBehaviour {

	[SerializeField]UISprite progressSprite;
	[SerializeField]UILabel questNameLabel;

	[SerializeField]TweenScale progressScale;

	bool isAnimationPlaying;
	// Use this for initialization
	void Start () {
		isAnimationPlaying = false;
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
//		progressSprite.transform.localScale = localScale;

		if (isAnimationPlaying == false)
			progressScale.from = localScale;

		localScale.x = rect.width*(5f*progressRatio);

		Debug.Log ("New scale: " + localScale);
		progressScale.to = localScale;
		
		if (isAnimationPlaying == false) {
			progressScale.Play (true);
			isAnimationPlaying = true;
		}

	}

	public void Initialize(string questName, string time){
		questNameLabel.text = questName;
		Vector3 scale = progressSprite.transform.localScale;
		scale.x = 0;
		progressSprite.transform.localScale = scale;
	}

	void OnScaleFinished()
	{
		isAnimationPlaying = false;
		Vector3 localScale = progressSprite.transform.localScale;
		progressScale.from = localScale;
		progressScale.to = localScale;

		progressScale.Reset ();
	}
}
