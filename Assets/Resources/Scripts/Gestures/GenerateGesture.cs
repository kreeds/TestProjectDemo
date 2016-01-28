using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DataGestures))]
public class GenerateGesture : MonoBehaviour
{
	int 					m_gestureIndex = 0;
	public int 				getGestureIndex
	{
		get{return m_gestureIndex;}
	}

	DataGestures 			m_mgData;

	GameObject 				m_guide;

	Sprite[]				m_sprites;

	public Transform 		m_parent;

	// Need to be refined with proper texture. This is a temporary solution 
    public int[]			m_easySpriteIndex;
    public int[]			m_hardSpriteIndex;


	void Awake()
	{
		m_mgData = GetComponent<DataGestures>();
	}

	void Start()
	{
		m_sprites = Resources.LoadAll<Sprite>("Sprite/multistrokes");
	}

	void CreateGestureObject()
	{
		// create the object
		m_guide = Instantiate(Resources.Load("Prefabs/GestureImage")) as GameObject;
		m_guide.GetComponent<SpriteRenderer>().sprite = m_sprites[m_gestureIndex];

		Debug.Log("Generated Gesture: " + m_gestureIndex + " guide: " + m_guide );

		m_guide.layer = LayerMask.NameToLayer("UI");
		m_guide.transform.SetParent(m_parent);
		m_guide.transform.localPosition = new Vector3( m_guide.transform.localPosition.x, m_guide.transform.localPosition.y, -20);
	}

	public void GenerateEasyGesture()
	{
		Debug.Log("Generate Easy Gestures");
		int random = UnityEngine.Random.Range(0, m_easySpriteIndex.Length);
		m_gestureIndex = m_easySpriteIndex[random];

		CreateGestureObject();

	}

	public void GenerateHardGesture()
	{
		Debug.Log("Generate Hard Gestures");
		int random = UnityEngine.Random.Range(0, m_hardSpriteIndex.Length);
		m_gestureIndex = m_hardSpriteIndex[random];
		CreateGestureObject();
	}

	public void GenerateRandomGesture()
    {

    	Debug.Log("Generate Random Gestures");
		m_gestureIndex = UnityEngine.Random.Range(0,m_mgData.GetGestureCount());

		// create the object
		m_guide = Instantiate(Resources.Load("Prefabs/GestureImage")) as GameObject;
		m_guide.GetComponent<SpriteRenderer>().sprite = m_sprites[m_gestureIndex];

		CreateGestureObject();
    }

	public void DestroyGesture()
    {
    	Debug.Log("Destoying Gesture " + m_guide);
		if(m_guide != null)
			Destroy(m_guide);
    }

}
