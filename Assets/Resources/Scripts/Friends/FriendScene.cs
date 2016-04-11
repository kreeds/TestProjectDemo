using UnityEngine;
using System.Collections;

public class FriendScene : MonoBehaviour {

	[SerializeField]UIGrid 		friendGrid;
	[SerializeField]UIGrid 		allyGrid;

	// Use this for initialization
	void Start () {
		
		Service.Init();	
		Service.Get<HUDService>().StartScene();
		PutDummyList ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PutDummyList()
	{
		string[] names = new string[] {"Margie", "Clarice", "Noelle", "Kylie"};
		string[] userNames = new string[] {"Jing", "Bobo", "Emiko", "Jira", "Yinhuey"};
		int[] values = new int[] {16, 5, 12, 48, 47};

		InitializeFriendList (names, values);
		InitializeFriendList (names, values);
		InitializeFriendList (names, values);
		InitializeFriendList (names, values);
		InitializeFriendList (names, values);
//		InitializeAllyList (userNames, values);
	}

	public void InitializeFriendList(string[] names, int[] relValues)
	{
		int count = Mathf.Min (names.Length, relValues.Length);
		for (int i = 0; i < count; ++i) {
			GameObject obj = NGUITools.AddChild(friendGrid.gameObject, Resources.Load ("Prefabs/Friends/FriendItem") as GameObject);
			FriendListItem friendListItem = obj.GetComponent<FriendListItem>();
			friendListItem.Initialize (names[i], relValues[i]);
		}
		friendGrid.Reposition ();
	}

	public void InitializeAllyList(string[] names, int[] relValues)
	{
		int count = Mathf.Max (names.Length, relValues.Length);
		for (int i = 0; i < count; ++i) {
			GameObject obj = NGUITools.AddChild(friendGrid.gameObject, Resources.Load ("Prefabs/Friends/FriendItem") as GameObject);
			FriendListItem friendListItem = obj.GetComponent<FriendListItem>();
			friendListItem.Initialize (names[i], relValues[i]);
		}

		allyGrid.Reposition ();
	}
}

