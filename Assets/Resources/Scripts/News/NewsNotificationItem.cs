using UnityEngine;
using System.Collections;

public class NewsNotificationItem : MonoBehaviour {

	[SerializeField]UISprite		_iconTexture;
	[SerializeField]UILabel			_newsLabel;

	private GameObject				_rootObject;

//	private TweenScale				_tweenScale;
	private TweenAlpha				_tweenAlpha;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(GameObject rootObject, string texture, string newsTxt){
		//		_iconTexture.mainTexture = Resources.Load (texture) as Texture;
		_iconTexture.spriteName = texture;
		
		_newsLabel.text = newsTxt;

		_rootObject = rootObject;
//		_tweenScale = GetComponent<TweenScale> ();
		_tweenAlpha = GetComponent<TweenAlpha> ();
	}

	void OnClose(){
		_rootObject.SendMessage ("GoToNext");
		
//		Service.Get<HUDService>().ChangeScene("FeedScene");

		_tweenAlpha.from = 0;
//		_tweenScale.from = new Vector3 (0.8f, 0.8f);

//		_tweenScale.Play (false);
		_tweenAlpha.Play (false);

		Destroy (gameObject, 0.25f);
	}
}
