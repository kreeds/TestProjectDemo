using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapHandler : MonoBehaviour {

	[SerializeField]UITexture m_background; 			// background of current Map

	List<AreaNode> m_areaNodeList;					// Container to store area nodes

	void LoadArea()
	{	
		m_areaNodeList = new List<AreaNode>();

		AreaNode node = CreateAreaNode (1, false, "1-4", new Vector3 (1312.0f, 147.0f, -10.0f));
		node.callback = "OnDressShop";
		node.receiver = gameObject;
		m_areaNodeList.Add(node);
		m_areaNodeList.Add(CreateAreaNode(1, true, "1-3", new Vector3(147, 170, -10.0f)) );
	}

	AreaNode CreateAreaNode(int aid, bool locked, string name, Vector3 pos)
	{
		GameObject obj = Instantiate(Resources.Load("Prefabs/AreaNode")) as GameObject;
		obj.transform.parent = gameObject.transform;
		obj.layer = gameObject.layer;
		AreaNode an =  obj.GetComponent<AreaNode>();
		an.Init(aid, locked, false, name, pos);

		return an;
	}

	// Use this for initialization
	void Start () {
		LoadArea();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDressShop()
	{
		QuestEvent.nextSceneID = 2;
		Service.Get<HUDService> ().ChangeScene ("EventScene");
	}
}
