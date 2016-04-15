using UnityEngine;
using System.Collections;

public class BoardHandler : MonoBehaviour {

	
	[SerializeField]UILabel gained;
	[SerializeField]GameObject gainboard;
	[SerializeField]GameObject winboard;
	[SerializeField]GameObject loseboard;

	GameObject Board;

	bool clicked = false;

	int m_gold;
	int m_exp;

	// Use this for initialization
	void Start () {

		//TODO: load quest win amount from somwhere
		m_gold = 100;
		m_exp = 100;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		// Load 
		if(!clicked)
		{
			gainboard.SetActive(true);
			gained.text = string.Format("You have gained:\n{0} gold and {1} xp", m_gold, m_exp);
			Board.SetActive(false);
			clicked = true;
		}
		else
		{
			QuestEvent.nextSceneID = 2;
			Service.Get<HUDService>().ChangeScene("EventScene");
			Service.Get<HUDService>().HUDControl.RemoveBattleHUD();
		}

	}


	public void Init(bool win)
	{
		Board = (win)? winboard : loseboard;
		Board.SetActive(true);
	}
}
