using UnityEngine;
using System.Collections;

public class HeadLineNewsItem : MonoBehaviour {

	[SerializeField]UITexture		_iconTexture;
	[SerializeField]UILabel		_newsLabel;
	
	private GameObject						_rootObject;
	
	private int								_id;

	[SerializeField]UILabel _timeLabel;
	[SerializeField]UILabel _locationLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(int id, GameObject rootObject, string texture, string newsTxt){
//		_iconTexture.mainTexture = Resources.Load (texture) as Texture;
		
		_newsLabel.text = newsTxt;
		
		_rootObject = rootObject;
		
		_id = id;
	}
	
	void OnClick() {
		_rootObject.SendMessage ("OnNewsItemClicked", _id);
	}
}
