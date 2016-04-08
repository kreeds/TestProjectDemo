using UnityEngine;
using System.Collections;

public class NewsItem : MonoBehaviour {
	[SerializeField]protected UISprite		_iconTexture;
	[SerializeField]protected UILabel		_newsLabel;

	private GameObject						_rootObject;

	private int								_id;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int id, GameObject rootObject, string texture, string newsTxt){
//		_iconTexture.mainTexture = Resources.Load (texture) as Texture;
		_iconTexture.spriteName = texture;

		_newsLabel.text = newsTxt;

		_rootObject = rootObject;

		_id = id;
	}

	void OnClick() {
//		_rootObject.SendMessage ("OnNewsItemClicked", _id);
	}
}
