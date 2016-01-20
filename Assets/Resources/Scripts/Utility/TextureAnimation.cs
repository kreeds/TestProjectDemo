using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureAnimation : MonoBehaviour {

	#region Fields
	[SerializeField] float 	m_scrollSpeed = 0.5f;
	[SerializeField] float	m_offset;
	#endregion

	#region Override MonoBehanviour
	// Use this for initialization
	void Start () {
		StartCoroutine(animationFlow());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	#endregion

	#region Public API
	public IEnumerator animationFlow()
	{
		while(true)
		{
			m_offset += (Time.deltaTime * m_scrollSpeed) / 10;
			renderer.material.SetTextureOffset("_MainTex", new Vector2(m_offset, 0));
			yield return null;
		}
	}
	#endregion
}
