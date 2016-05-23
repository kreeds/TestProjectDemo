using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankingDisplay : MonoBehaviour {


	[SerializeField]UIGrid				_rankGrid;
	[SerializeField]LAppModelProxy		_live2DModel;
	[SerializeField]UISlider	m_expBar;


	List<RankListItem>			_rankItemList;
	int playerRank;


	// Use this for initialization
	void Start () {
		PutDummyPlayers ();
		_live2DModel.LoadProfile ();
		_live2DModel.PlayIdleAnim ();

		
		m_expBar.sliderValue = 0.4f;
	}

	void PutDummyPlayers()
	{
		playerRank = 0;
		_rankItemList = new List<RankListItem> ();
		string[] names = {"Marie", 
			"Judith",
			"Lisa",
			"Violet",
			"Betty",
			"Pauline",
			"Linda",
			"Anna",
			"Kelly",
			"Clara",
			"Heather",
			"Gina",
			"Jane",
			"Doris",
			"Candice"};
		for (int i = 0; i < 15; ++i) {
			GameObject obj = NGUITools.AddChild (_rankGrid.gameObject, Resources.Load ("Prefabs/Ranking/RankListItem") as GameObject);
			RankListItem listItem = obj.GetComponent<RankListItem>();
			string name = "name";
			if (i == playerRank)
				name = PlayerProfile.Get ().playerName;
			else
				name = names[i];

			listItem.Initialize(gameObject, name, i);
			_rankItemList.Add(listItem);
		}

		_rankItemList [playerRank].SendMessage ("OnClick");
		OnRankItemSelected (playerRank);
		_rankGrid.Reposition ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnRankItemSelected(int id){
		if (id != playerRank)
			_live2DModel.SetCostume (_rankItemList [id].clothesIndex, _rankItemList [id].hairIndex);
		else
			_live2DModel.LoadProfile ();

		for (int i = 0; i < 15; ++i) {
			if (i == id)
				continue;

			_rankItemList[i].DeSelect();
		}
	}

	void OnClose()
	{
		Destroy (gameObject);
	}
}
