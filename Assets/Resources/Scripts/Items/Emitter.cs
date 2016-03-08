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

	public enum EmitType
	{
		Impulse = 0,
		Flow,
		Max
	};

	Dictionary<ItemType, int> map;
	List<BoxCollider> colContainer = new List<BoxCollider>();
	EmitType m_type;

	// Use this for initialization
	void Start () {

		Debug.Log("Starting Emitter **********");
		map = new Dictionary<ItemType, int>();
		ItemType[] type = new ItemType[1];
		type[0] = ItemType.GOLD;
		int[] amt = new int[1];
		amt[0] = 100;

		Init(type, amt, EmitType.Flow);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void IgnoreCollision()
	{
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

	void ImpulseFire(ItemType type, int amt)
	{
		for(int i = 0; i < amt; ++i)
		{
			GameObject obj =  Instantiate(Resources.Load("Prefabs/CollectableItems")) as GameObject;
			obj.transform.parent = gameObject.transform;
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;

			CollectableItems item = obj.GetComponent<CollectableItems>();
			item.Init(type);
			Rigidbody body = obj.GetComponent<Rigidbody>();
			colContainer.Add(obj.GetComponent<BoxCollider>());

			float randomx= Random.Range(-1f, 1f);
			//float randomy = Random.Range(-3f, 3f);
			body.AddForce(new Vector3(randomx, 0,0), ForceMode.Impulse);
		}

		IgnoreCollision();

	}

	IEnumerator CreateItemsWithInterval(ItemType type, int amt)
	{
		int i = 0;
		while(i < amt)
		{	
			i++;

			GameObject obj =  Instantiate(Resources.Load("Prefabs/CollectableItems")) as GameObject;
			obj.transform.parent = gameObject.transform;
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;

			CollectableItems item = obj.GetComponent<CollectableItems>();
			item.Init(type);
			Rigidbody body = obj.GetComponent<Rigidbody>();
			colContainer.Add(obj.GetComponent<BoxCollider>());

			float randomx= Random.Range(0f, 30f);
			float randomy = Random.Range(-10f, 10f);
			body.AddForce(new Vector3(randomx, randomy,0), ForceMode.Force);

			IgnoreCollision();

			yield return new WaitForSeconds(0.01f);
		}


	}

	public void Init(ItemType[] type, int[] amt, EmitType emit = EmitType.Impulse)
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

		m_type = emit;


		foreach(ItemType tp in keys)
		{
			if(m_type == EmitType.Impulse)
				ImpulseFire(tp, map[tp]);
			else
				StartCoroutine(CreateItemsWithInterval(tp, map[tp]));
		}

	
	}
}
