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
	public bool _isRead;

	public string _location;

	public NewsDataItem(string headLine, string iconTextureName, string mainTextureName, bool isRead = false, string newsBody = "", int newsTime = 120, string location = "Asia")
	{
		_headLine = headLine;
		_iconTextureName = iconTextureName;
		_mainTextureName = mainTextureName;

		_newsBody = newsBody;

		_isRead = isRead;
	}

}

public class NewsScene : MonoBehaviour {

	[SerializeField]UITable				_listTable;
	[SerializeField]UISprite			_newsTexture;
	[SerializeField]UISprite			_detailedNewsTexture;

	[SerializeField]UILabel				_headingText;
	[SerializeField]UILabel				_bodyText;

	[SerializeField]GameObject			_detailGroup;
	[SerializeField]GameObject			_listGroup;

	private List<NewsDataItem>			_newsDataList;

	private UITweener					_detailTween;
	SoundService m_soundService;
	AudioClip	m_bgm;

	// Use this for initialization
	void Start () {
		Service.Init();	
		Service.Get<HUDService>().StartScene();
//		_newsDataList = new List<NewsDataItem> ();

		m_bgm = Resources.Load("Music/headlinesmusic") as AudioClip;
		m_soundService = Service.Get<SoundService>();
		m_soundService.PlayMusic(m_bgm, true);

//		InitializeDummy ();
		InitializeNews ();
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

//		_newsDataList.Add (new NewsDataItem ("@Emi was in the newspapers", "LK_portrait", "newspaper"));
//		_newsDataList.Add (new NewsDataItem ("@Weining was on the TV news", "LK_portrait", "tvnews"));
//		_newsDataList.Add (new NewsDataItem ("[Caution] Fake Lady Knights", "LK_portrait", "Fakeladyknight"));
//		_newsDataList.Add (new NewsDataItem ("@Jing got a thank you letter", "portrait", "thankyounote"));
//
//		_newsDataList [0]._newsBody = "Emi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapersEmi was in the newspapers";
//		_newsDataList [1]._newsBody = "@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news@Weining was on the TV news";
//		_newsDataList [2]._newsBody = "[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights[Caution] Fake Lady Knights";
//		_newsDataList [3]._newsBody = "@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter@Jing got a thank you letter";
//
//		_newsDataList.AddRange (_newsDataList);
//		_newsDataList.AddRange (_newsDataList);
//
//		InitializeNews ();
	}

	void InitializeNews()
	{
		_newsDataList = PlayerProfile.Get ().GetNewsList();

		int count = _newsDataList.Count;

		int id = 0;

		GameObject obj = NGUITools.AddChild (_listTable.gameObject, Resources.Load ("Prefabs/News/HeadLineNewsItem") as GameObject);
		obj.name = "NewsItem" + id;

		HeadLineNewsItem headLineNews = obj.GetComponent<HeadLineNewsItem> ();
		headLineNews.Initialize (id, gameObject, _newsDataList [id]._iconTextureName, _newsDataList [id]._headLine, _newsDataList[id]._isRead);
		 
		_newsTexture.spriteName = _newsDataList [id]._mainTextureName;
		id++;

		for (; id < count; ++id) {
			
			obj = NGUITools.AddChild (_listTable.gameObject, Resources.Load ("Prefabs/News/NewsItem") as GameObject);
			obj.name = "NewsItem" + id;
			
			NewsItem newsItem = obj.GetComponent<NewsItem> ();
			newsItem.Initialize (id, gameObject, _newsDataList [id]._iconTextureName, _newsDataList [id]._headLine, _newsDataList[id]._isRead);
		}

		_listTable.Reposition ();

		_detailTween = _detailGroup.GetComponent<UITweener> ();
		_detailTween.Play (false);
	}

	void OnNewsItemClicked(int id){
		if (_detailTween == null)
			return;

//		_newsTexture.mainTexture = Resources.Load (_newsDataList [id]._mainTextureName) as Texture; 
		_newsTexture.spriteName = _newsDataList [id]._mainTextureName;
		_detailedNewsTexture.spriteName = _newsTexture.spriteName;

//		_detailGroup.SetActive (true);
//		_listTable.gameObject.SetActive (false);

		_detailTween.Play (true);

		_bodyText.text = _newsDataList [id]._newsBody;
		_headingText.text = _newsDataList [id]._headLine;

		PlayerProfile.Get ().SetNewsAsRead (id);
	}

	void OnBack()
	{
//		if (_detailGroup.activeSelf) {
//			_newsTexture.spriteName = "newspaper";
//			_listTable.gameObject.SetActive(true);
//			_detailGroup.SetActive(false);
//		} else {
//			Service.Get<HUDService> ().ReturnToHome ();
//			//go back to previous scene
//		}
		if (_detailTween == null)
			return;

		_detailTween.Play (false);
	}
}
