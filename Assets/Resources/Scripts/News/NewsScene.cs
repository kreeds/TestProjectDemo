﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewsDataItem
{
	public string _headLine;
	public string _newsBody;

	public string _iconTextureName;
	public string _mainTextureName;

	public int 	_newsTime;
	public string _location;

	public NewsDataItem(string headLine, string iconTextureName, string mainTextureName, string newsBody = "", int newsTime = 120, string location = "Asia")
	{
		_headLine = headLine;
		_iconTextureName = iconTextureName;
		_mainTextureName = mainTextureName;

		_newsBody = newsBody;
	}

}

public class NewsScene : MonoBehaviour {

	[SerializeField]UITable				_listTable;
	[SerializeField]UISprite			_newsTexture;

	[SerializeField]UILabel				_headingText;
	[SerializeField]UILabel				_bodyText;

	[SerializeField]GameObject			_detailGroup;
	[SerializeField]GameObject			_listGroup;

	private List<NewsDataItem>			_newsDataList;
	SoundService m_soundService;
	AudioClip	m_bgm;

	// Use this for initialization
	void Start () {
		Service.Init();	
		Service.Get<HUDService>().StartScene();
		_newsDataList = new List<NewsDataItem> ();

		m_bgm = Resources.Load("Music/headlinesmusic") as AudioClip;
		m_soundService = Service.Get<SoundService>();
		m_soundService.PlayMusic(m_bgm, true);

		InitializeDummy ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeDummy()
	{
//		_newsDataList.Add (new NewsDataItem ("A new hero is in town!", "LK_portrait", "newspaper"));
//		_newsDataList.Add (new NewsDataItem ("Mysterious girl saves town! Who can she be?", "LK_portrait", "tvnews"));
//		_newsDataList.Add (new NewsDataItem ("[Caution] Fake Lady Knights", "LK_portrait", "tvnews"));
//		_newsDataList.Add (new NewsDataItem ("Thank you!", "portrait", "thankyounote"));

		_newsDataList.Add (new NewsDataItem ("@Emiko was in the newspapers", "LK_portrait", "newspaper"));
		_newsDataList.Add (new NewsDataItem ("@Weining was on the TV news", "LK_portrait", "tvnews"));
		_newsDataList.Add (new NewsDataItem ("[Caution] Fake Lady Knights", "LK_portrait", "Fakeladyknight"));
		_newsDataList.Add (new NewsDataItem ("@Jing got a thank you letter", "portrait", "thankyounote"));

		_newsDataList [0]._newsBody = "@Emiko was in the newspapers@Emiko was in the newspapers@Emiko was in the newspapers@Emiko was in the newspapers@Emiko was in the newspapers@Emiko was in the newspapers";
		_newsDataList [1]._newsBody = "@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news";
		_newsDataList [2]._newsBody = "[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights";
		_newsDataList [3]._newsBody = "@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter";

		InitializeNews ();
	}

	void InitializeNews()
	{
		int count = _newsDataList.Count;

		int id = 0;

		GameObject obj = NGUITools.AddChild (_listTable.gameObject, Resources.Load ("Prefabs/News/HeadLineNewsItem") as GameObject);
		obj.name = "NewsItem" + id;

		HeadLineNewsItem headLineNews = obj.GetComponent<HeadLineNewsItem> ();
		headLineNews.Initialize (id, gameObject, _newsDataList [id]._iconTextureName, _newsDataList [id]._headLine);
		 
		_newsTexture.spriteName = _newsDataList [id]._mainTextureName;
		id++;

		for (; id < count; ++id) {
			
			obj = NGUITools.AddChild (_listTable.gameObject, Resources.Load ("Prefabs/News/NewsItem") as GameObject);
			obj.name = "NewsItem" + id;
			
			NewsItem newsItem = obj.GetComponent<NewsItem> ();
			newsItem.Initialize (id, gameObject, _newsDataList [id]._iconTextureName, _newsDataList [id]._headLine);
		}
	}

	void OnNewsItemClicked(int id){
//		_newsTexture.mainTexture = Resources.Load (_newsDataList [id]._mainTextureName) as Texture; 
		_newsTexture.spriteName = _newsDataList [id]._mainTextureName;

		_detailGroup.SetActive (true);
		_listTable.gameObject.SetActive (false);

		_bodyText.text = _newsDataList [id]._newsBody;
		_headingText.text = _newsDataList [id]._headLine;
	}

	void OnBack()
	{
		if (_detailGroup.activeSelf) {
			_newsTexture.spriteName = "newspaper";
			_listTable.gameObject.SetActive(true);
			_detailGroup.SetActive(false);
		} else {
			Service.Get<HUDService> ().ReturnToHome ();
			//go back to previous scene
		}
	}
}
