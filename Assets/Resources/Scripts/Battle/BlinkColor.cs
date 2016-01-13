using UnityEngine;
using System.Collections;

public class BlinkColor : MonoBehaviour {

	public float[] 	m_duration;
	public float 	m_interval = 0.01f;
	public Color[]	m_color;

	SpriteRenderer 	m_spRenderer;
	float 			m_startTime;
	int				m_index;
	bool			m_flag;

	// Use this for initialization
	void Start () {
		m_startTime = Time.fixedTime;
		m_spRenderer = GetComponent<SpriteRenderer>();
		m_index = 0;
		m_spRenderer.color = m_color[0];
		InvokeRepeating("Blink", 0, m_interval);
		m_flag = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		float timeElpased = Time.time - m_startTime;
		if(m_index < m_duration.Length && timeElpased > m_duration[m_index])
		{
			Debug.Log("TimeElapsed: " + timeElpased);
			m_spRenderer.color = m_color[m_index];
			m_startTime = Time.time;
			m_index++;
		}
	}

	void Blink()
	{
		m_flag = !m_flag;
		if(m_index >= m_color.Length)
			return;

		if(m_flag)
			m_spRenderer.color = new Color(m_color[m_index].r, m_color[m_index].g, m_color[m_index].b, 0.0f);
		else
			m_spRenderer.color = new Color(m_color[m_index].r, m_color[m_index].g, m_color[m_index].b, 1.0f);
	}

}
