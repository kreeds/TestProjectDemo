using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureAnimation : MonoBehaviour {

	#region Fields
	[SerializeField] float 	m_scrollSpeed = 0.5f;
	[SerializeField] float	m_offset;
	[SerializeField] bool	m_usingNGUI = false;

	[SerializeField] Vector2 m_dir = new Vector2(1,0);
	[SerializeField] bool 	m_isHorizontal = true;
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

		if(!m_usingNGUI)
		{
			renderer.material.SetTextureOffset("_MainTex", Vector2.zero);
		}
		else
		{
			tex.uvRect = new Rect(0, 0, 1, 1);
		}
	}
	#endregion

	#region Public API
	public IEnumerator animationFlow()
	{
		while(true)
		{
			m_offset += (Time.deltaTime * m_scrollSpeed) / 10;
			if(!m_usingNGUI)
			{
				renderer.material.SetTextureOffset("_MainTex", m_dir * m_offset);
			}
			else
			{
				Vector2 vec = m_dir*m_offset;
				tex.uvRect = new Rect(vec.x, vec.y, 1, 1);
			}
			yield return null;
		}
	}
	#endregion
}
