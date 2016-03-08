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

	float m_rangeMinX;
	float m_rangeMaxX;

	float m_rangeMinY;
	float m_rangeMaxY;

	// Use this for initialization
	void Start () {

//		Debug.Log("Starting Emitter **********");
//		map = new Dictionary<ItemType, int>();
//		ItemType[] type = new ItemType[1];
//		type[0] = ItemType.GOLD;
//		int[] amt = new int[1];
//		amt[0] = 100;
//
//		Init(type, amt, EmitType.Flow);

	}

	void Awake()
	{
		map = new Dictionary<ItemType, int>();
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

			float randomx= Random.Range(m_rangeMinX, m_rangeMaxX);
			float randomy = Random.Range(m_rangeMinY, m_rangeMaxY);
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

			float randomx= Random.Range(m_rangeMinX, m_rangeMaxX);
			float randomy = Random.Range(m_rangeMinY, m_rangeMaxY);
			body.AddForce(new Vector3(randomx, randomy,0), ForceMode.Force);

			IgnoreCollision();

			yield return new WaitForSeconds(0.01f);
		}


	}

	/// <summary>
	/// Init the specified type, amt, rangeminx, rangemaxx, rangeminy, rangemaxy and emit.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="amt">Amount.</param>
	/// <param name="rangeminx">Minimum Range of X value.</param>
	/// <param name="rangemaxx">Maximum Range of X value.</param>
	/// <param name="rangeminy">Minimum Range of Y value.</param>
	/// <param name="rangemaxy">Minimum Range of Y value.</param>
	/// <param name="emit">Type of Emission.</param>
	public void Init(ItemType[] type, int[] amt, float rangeminx, float rangemaxx, float rangeminy, float rangemaxy, 
						EmitType emit = EmitType.Impulse)
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

		m_rangeMinX = rangeminx;
		m_rangeMaxX = rangemaxx;
		m_rangeMinY = rangeminy;
		m_rangeMaxY = rangemaxy;


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
