using UnityEngine;
using System.Collections;

public class NewsNotificationItem : MonoBehaviour {

	[SerializeField]UISprite		_iconTexture;
	[SerializeField]UILabel		_newsLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(string texture, string newsTxt){
		//		_iconTexture.mainTexture = Resources.Load (texture) as Texture;
		_iconTexture.spriteName = texture;
		
		_newsLabel.text = newsTxt;
	}
}
