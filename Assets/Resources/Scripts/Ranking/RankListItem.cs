using UnityEngine;
using System.Collections;

public class RankListItem : MonoBehaviour {

	// Use this for initialization
	[SerializeField]UILabel _nameLabel;
	[SerializeField]UILabel _rankLabel;

	[SerializeField]UISprite _rankIcon;

	[SerializeField]UISprite _bgSprite;
	[SerializeField]UISprite _rankBGSprite;

	GameObject _rootObject;

	public int hairIndex;
	public int clothesIndex;

	public int id;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(GameObject rootObject, string playerName, int playerRankPosition){
		_nameLabel.text = playerName;
		_rootObject = rootObject;

		_rankLabel.text = playerRankPosition.ToString ();

		int playerRank = playerRankPosition / 3;
		if (playerRank > 3)
			playerRank = 3;
		id = playerRankPosition;
		switch (playerRank) {
		case 0:
			_rankIcon.spriteName = "NameRank_S";
			break;
		case 1:
			_rankIcon.spriteName = "NameRank_A";
			break;

		case 2:
			_rankIcon.spriteName = "NameRank_B";
			break;

		case 3:
			_rankIcon.spriteName = "NameRank_C";
			break;
		}

		hairIndex = Random.Range (0, 4);
		clothesIndex = Random.Range (0, 4);
	}

	public void DeSelect()
	{
		_bgSprite.spriteName = "NameBG_UnSelected";
		_rankBGSprite.spriteName = "NameRankBG_UnSelected";
	}

	void OnClick()
	{
		_bgSprite.spriteName = "NameBG_Selected";
		_rankBGSprite.spriteName = "NameRankBG_Selected";
		_rootObject.SendMessage ("OnRankItemSelected", id);
	}
}
