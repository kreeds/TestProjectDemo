using UnityEngine;
using System.Collections;

public class FriendListItem : MonoBehaviour {

	[SerializeField]UIScrollBar relBar;
	[SerializeField]UILabel nameLabel;
	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(string name, int relationShipLv){
		nameLabel.text = name;
		float val = (relationShipLv % 5)/5f;
		relBar.barSize = val;
	}
}
