using UnityEngine;
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

	// Use this for initialization
	void Start () {
		_newsDataList = new List<NewsDataItem> ();

		InitializeDummy ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeDummy()
	{
		_newsDataList.Add (new NewsDataItem ("Although, it is quite the feat that you survived this long. ", "portrait", "newspaper", "After seeing the boost in my levees from the bishopric, I revoke the city title as well. " +
			"The maximum levees I can field now are doubled from just a couple years ago. We now outnumber several of the smaller realms around."));
		_newsDataList.Add (new NewsDataItem ("Given how much his son hates him, I'm surprised nothing has come of it.", "portrait", "newspaper"));
		_newsDataList.Add (new NewsDataItem ("[CAUTION]Fake lady knight", "portrait", "newspaper"));
		_newsDataList.Add (new NewsDataItem ("Thank you", "portrait", "note"));

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
			_listTable.gameObject.SetActive(true);
			_detailGroup.SetActive(false);
		} else {
			//go back to previous scene
		}
	}
}
