using UnityEngine;
using System.Collections;

public class HeadLineNewsItem : MonoBehaviour {

	[SerializeField]UITexture		_iconTexture;
	[SerializeField]UILabel		_newsLabel;
	[SerializeField]GameObject		_unreadNewsObject;
	
	private GameObject						_rootObject;
	
	private int								_id;
	private bool							_isRead;

	[SerializeField]UILabel _timeLabel;
	[SerializeField]UILabel _locationLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int id, GameObject rootObject, string texture, string newsTxt, bool isRead){
//		_iconTexture.mainTexture = Resources.Load (texture) as Texture;
		
		_newsLabel.text = newsTxt;
		
		_rootObject = rootObject;
		
		_id = id;

		_isRead = isRead;

		_unreadNewsObject.SetActive (!isRead);
	}
	
	void OnClick() {
		_rootObject.SendMessage ("OnNewsItemClicked", _id);

		_isRead = false;
		_unreadNewsObject.SetActive (false);
	}
}
