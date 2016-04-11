using UnityEngine;
using System.Collections;

public class FriendListItem : MonoBehaviour {

	[SerializeField]UIScrollBar 			relBar;

	[SerializeField]UILabel 				nameLabel;
	[SerializeField]UILabel 				relLabel;

	[SerializeField]UISprite				fillGauge;
	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(string name, int relationShipLv){
		nameLabel.text = name;
		float val = (relationShipLv % 5)/5f;

		int relVal = relationShipLv / 5;

		if (relVal > 5) {
			fillGauge.spriteName = "Friendslist_UI_FillBar_Love";
		}

		relLabel.text = "Lv " + relVal;

		relBar.barSize = val;
	}
}
