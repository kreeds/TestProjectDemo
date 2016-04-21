using UnityEngine;
using System.Collections;

public class NewsItem : MonoBehaviour {
	[SerializeField]protected UISprite		_iconTexture;
	[SerializeField]protected UILabel		_newsLabel;
	[SerializeField]GameObject				_unreadNewsObject;

	private GameObject						_rootObject;

	private int								_id;

	private bool							_isRead;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int id, GameObject rootObject, string texture, string newsTxt, bool isRead){
//		_iconTexture.mainTexture = Resources.Load (texture) as Texture;
		_iconTexture.spriteName = texture;

		_newsLabel.text = newsTxt;

		_rootObject = rootObject;

		_id = id;

		_isRead = isRead;

		_unreadNewsObject.SetActive (!_isRead);
	}

	void OnClick() {
		_rootObject.SendMessage ("OnNewsItemClicked", _id);
		_isRead = true;
		_unreadNewsObject.SetActive (false);
	}
}
