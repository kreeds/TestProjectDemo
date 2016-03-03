using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Item type to emit
public enum ItemType
{
	GOLD = 0,
	ENERGY,
	XP,
	STAR,

	MAX
};

public class Emitter : MonoBehaviour {

	Dictionary<ItemType, int> map;
	List<BoxCollider> colContainer = new List<BoxCollider>();

	// Use this for initialization
	void Start () {

		Debug.Log("Starting Emitter **********");
		map = new Dictionary<ItemType, int>();
		ItemType[] type = new ItemType[1];
		type[0] = ItemType.GOLD;
		int[] amt = new int[1];
		amt[0] = 10;

		Init(type, amt);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateItems(ItemType type, int amt)
	{
		for(int i = 0; i < amt; ++i)
		{
			GameObject obj =  Instantiate(Resources.Load("Prefabs/CollectableItems")) as GameObject;
			obj.transform.parent = gameObject.transform;
			obj.transform.localScale = new Vector3(60,60,60);
			obj.transform.localPosition = Vector3.zero;

			CollectableItems item = obj.GetComponent<CollectableItems>();
			item.Init(type);
			Rigidbody body = obj.GetComponent<Rigidbody>();
			colContainer.Add(obj.GetComponent<BoxCollider>());

			float randomx= Random.Range(-0.4f, 0.4f);
			float randomy = Random.Range(-1.5f, 1.5f);
			body.AddForce(new Vector3(randomx, randomy,0), ForceMode.Impulse);
		}

		for(int j = 0; j < colContainer.Count; ++j)
		{	
			for(int k = 0; k < colContainer.Count; ++k)
			{
				if(colContainer[j] == colContainer[k])
					continue;
				Physics.IgnoreCollision(colContainer[j], colContainer[k]); 
			}
		}

	}
	public void Init(ItemType[] type, int[] amt)
	{
		if(type.Length != amt.Length)
		{
			Debug.LogError("Total number of types and total number of amounts are different");
			return;
		}
	
		for(int i = 0; i < type.Length; ++i)
		{
			map.Add(type[i], amt[i]);
		}

		List<ItemType> keys = new List<ItemType>(map.Keys);


		foreach(ItemType tp in keys)
		{
			CreateItems(tp, map[tp]);
		}
	}
}
