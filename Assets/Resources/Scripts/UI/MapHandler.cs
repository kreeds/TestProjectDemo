using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapHandler : MonoBehaviour {

	[SerializeField]UITexture m_background; 			// background of current Map

	List<AreaNode> m_areaNodeList;					// Container to store area nodes

	void LoadArea()
	{	
		m_areaNodeList = new List<AreaNode>();
		m_areaNodeList.Add(CreateAreaNode(1, false, "1-4", new Vector3(661.0f, 50.0f, -10.0f)) );
		m_areaNodeList.Add(CreateAreaNode(1, true, "1-3", new Vector3(204.6f, 84.9f, -10.0f)) );
	}

	AreaNode CreateAreaNode(int aid, bool locked, string name, Vector3 pos)
	{
		GameObject obj = Instantiate(Resources.Load("Prefabs/AreaNode")) as GameObject;
		obj.transform.parent = gameObject.transform;
		obj.layer = gameObject.layer;
		AreaNode an =  obj.GetComponent<AreaNode>();
		an.Init(aid, locked, name, pos);

		return an;
	}

	// Use this for initialization
	void Start () {
		LoadArea();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
