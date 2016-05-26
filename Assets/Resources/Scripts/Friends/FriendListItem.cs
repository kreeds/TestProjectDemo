using UnityEngine;
using System.Collections;

public class FriendListItem : MonoBehaviour {

	[SerializeField]UIScrollBar 			relBar;

	[SerializeField]UILabel 				nameLabel;
	[SerializeField]UILabel 				relLabel;

	[SerializeField]UISprite				fillGauge;
	[SerializeField]UISprite				iconBG;
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

		iconBG.spriteName = string.Format ("Friendslist_UI_Thumbnail_0{0}", Random.Range (1, 5));

		int thickness = 2;
		int labelCount = thickness * 4;

		float angle = 360f / labelCount;

		Vector3[] offsets = {Vector2.up, -Vector2.up, Vector2.right, -Vector2.right};
		for (int i = 0; i < labelCount; ++i) {
			UILabel label = NGUITools.AddWidget<UILabel>(gameObject);
			label.depth = nameLabel.depth - 1;

			Vector3 offset = new Vector3(Mathf.Cos(angle*i), Mathf.Sin (angle*i));

			label.transform.localPosition = nameLabel.transform.localPosition + offset*thickness;
			label.transform.localScale = nameLabel.transform.localScale;

			label.text = name;
			label.color = Color.black;
			label.font = nameLabel.font;
		}
	}
}
