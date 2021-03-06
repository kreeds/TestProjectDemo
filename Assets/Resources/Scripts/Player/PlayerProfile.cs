﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerProfile {

	public string playerName;
	public int playerModelIndex;
	public int playerHairIndex;
	public int playerClothesIndex;

	public int stamina;
	public int maxStamina;

	public int gold;
	public int gems;

	public int level;

	public int unreadNewsCnt;

	public const float recoverTime = 300f;

	public float recoverAmount;

	private List<NewsDataItem> newsList;

	private List<int> clearedEventList;

	public int currentEventSet;

	static PlayerProfile playerProfile;

	PlayerProfile()
	{
		playerName = "Ellie";
		playerHairIndex = 1;
		playerClothesIndex = 1;
		
		maxStamina = stamina = 30;
		gems = 500;
		gold = 500000;
		level = 1;

		recoverAmount = 0;

		currentEventSet = -1;
		
		newsList = new List<NewsDataItem> ();
		clearedEventList = new List<int> ();

		InitializeDummyNews ();
	}

	static public PlayerProfile Get(){
		if (playerProfile == null) {
			playerProfile = new PlayerProfile();

		}
	
		return playerProfile;
	}


	public List<NewsDataItem> GetNewsList()
	{
		return newsList;
	}

	void InitializeDummyNews()
	{
		//		newsList.Add (new NewsDataItem ("A new hero is in town!", "LK_portrait", "newspaper"));
		//		newsList.Add (new NewsDataItem ("Mysterious girl saves town! Who can she be?", "LK_portrait", "tvnews"));
		//		newsList.Add (new NewsDataItem ("[Caution] Fake Lady Knights", "LK_portrait", "tvnews"));
		//		newsList.Add (new NewsDataItem ("Thank you!", "portrait", "thankyounote"));
		
		newsList.Add (new NewsDataItem ("@Emi was in the newspapers", "newspaper", "newspaper", true));
		newsList.Add (new NewsDataItem ("@Weining was on the TV news", "tvnews", "tvnews", true));
		newsList.Add (new NewsDataItem ("[Caution] Fake Lady Knights", "Fakeladyknight", "Fakeladyknight", true));
		newsList.Add (new NewsDataItem ("@Jing got a thank you letter", "thankyounote", "thankyounote", true));
		
		newsList [0]._newsBody = "Emi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapers";
		newsList [1]._newsBody = "@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news";
		newsList [2]._newsBody = "[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights";
		newsList [3]._newsBody = "@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter";
		
		newsList.AddRange (newsList);
		newsList.AddRange (newsList);
		
		unreadNewsCnt = newsList.FindAll (delegate(NewsDataItem newsItem) {
			return newsItem._isRead == false;
		}).Count;
	}

	public void AddNews(NewsDataItem item)
	{
		newsList.Insert (0, item);
		if (item._isRead == false)
			unreadNewsCnt++;
	}

	public void SetNewsAsRead(int index){
		if (newsList [index]._isRead == false) {
			unreadNewsCnt--;
			newsList [index]._isRead = true;
		}
	}

	public bool IsActionAvailable(int cost){
		return stamina > cost;
	}

	public void StartAction(int cost){
		stamina -= cost;
		if (recoverAmount <= 0)
			recoverAmount = recoverTime;
	}

	public void AddClearedEvent(int eventID){
		clearedEventList.Add (eventID);
	}

	public bool IsEventCleared(int eventID){
		if (eventID == -1)
			return true;

		foreach (int id in clearedEventList) {
			if (id == eventID)
				return true;
		}
		return false;
	}

	public void UpdateTime(float deltaTime){
		if (stamina < maxStamina) {
			recoverAmount -= Time.fixedDeltaTime;
			if (recoverAmount <= 0){
				stamina++;
			}
		}
	}
}
