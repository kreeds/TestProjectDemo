using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Type of item drops
/// </summary>
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

	public float spread = 50.0f;

	Dictionary<ItemType, int> map = new Dictionary<ItemType, int>();
	List<BoxCollider> colContainer = new List<BoxCollider>();
	EmitType m_type;

	float m_rangeMinX;
	float m_rangeMaxX;

	float m_rangeMinY;
	float m_rangeMaxY;

	void Awake()
	{
		map = new Dictionary<ItemType, int>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Ignores the collision between items
	/// </summary>
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

	/// <summary>
	/// Impulse Emission.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="amt">Amt.</param>
	void ImpulseFire(ItemType type, int amt)
	{
		float lifetime = 0.0f;
		for(int i = 0; i < amt; ++i)
		{
			GameObject obj =  Instantiate(Resources.Load("Prefabs/CollectableItems")) as GameObject;
			obj.transform.parent = gameObject.transform;
			obj.transform.localScale = Vector3.one;
			float rangeXY = Random.Range(-spread, spread);

			obj.transform.localPosition = new Vector3(rangeXY, rangeXY, 0);

			CollectableItems item = obj.GetComponent<CollectableItems>();
			lifetime = item.LifeTime;
			item.Init(type);
			Rigidbody body = obj.GetComponent<Rigidbody>();
			colContainer.Add(obj.GetComponent<BoxCollider>());

			float randomx= Random.Range(m_rangeMinX, m_rangeMaxX);
			float randomy = Random.Range(m_rangeMinY, m_rangeMaxY);
			body.AddForce(new Vector3(randomx, randomy, randomx), ForceMode.Impulse);
		}

		IgnoreCollision();

		StartCoroutine(Utility.DelayInSeconds(lifetime, (res) =>{ Destroy(this); } ));

	}

	/// <summary>
	/// Creates the items with interval.
	/// </summary>
	/// <returns>The items with interval.</returns>
	/// <param name="type">Type.</param>
	/// <param name="amt">Amt.</param>
	IEnumerator CreateItemsWithInterval(ItemType type, int amt)
	{
		int i = 0;
		float lifetime = 0.0f;
		while(i < amt)
		{	
			i++;

			GameObject obj =  Instantiate(Resources.Load("Prefabs/CollectableItems")) as GameObject;
			obj.transform.parent = gameObject.transform;
			obj.transform.localScale = Vector3.one;
			float rangeXY = Random.Range(-spread, spread);

			obj.transform.localPosition = new Vector3(rangeXY, rangeXY, 0);

			CollectableItems item = obj.GetComponent<CollectableItems>();
			lifetime = item.LifeTime;
			item.Init(type);
			Rigidbody body = obj.GetComponent<Rigidbody>();
			colContainer.Add(obj.GetComponent<BoxCollider>());

			float randomx= Random.Range(m_rangeMinX, m_rangeMaxX);
			float randomy = Random.Range(m_rangeMinY, m_rangeMaxY);
			body.AddForce(new Vector3(randomx, randomy, -10.0f), ForceMode.Force);

			IgnoreCollision();

			yield return new WaitForSeconds(0.005f);
		}

		yield return StartCoroutine(Utility.DelayInSeconds(lifetime, (res) =>{ Destroy(this); } ));
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
