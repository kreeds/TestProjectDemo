using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureAnimation : MonoBehaviour {

	#region Fields
	[SerializeField] float 	m_scrollSpeed = 0.5f;
	[SerializeField] float	m_offset;
	[SerializeField] bool	usingNGUI = false;

	UITexture tex;
	#endregion

	#region Override MonoBehanviour
	// Use this for initialization
	void OnEnable () {
		tex = GetComponent<UITexture>();
		StartCoroutine(animationFlow());
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void Forward(bool forward)
	{
		m_scrollSpeed = (forward)? Mathf.Abs(m_scrollSpeed) : - Mathf.Abs(m_scrollSpeed);
	}

	public void Reset()
	{
		m_offset = 0;

		if(!usingNGUI)
			renderer.material.SetTextureOffset("_MainTex", Vector2.zero);
		else
			tex.uvRect = new Rect(0, 0, 1, 1);
	}
	#endregion

	#region Public API
	public IEnumerator animationFlow()
	{
		while(true)
		{
			m_offset += (Time.deltaTime * m_scrollSpeed) / 10;
			if(!usingNGUI)
				renderer.material.SetTextureOffset("_MainTex", new Vector2(m_offset, 0));
			else
				tex.uvRect = new Rect(m_offset, 0, 1, 1);
			yield return null;
		}
	}
	#endregion
}
