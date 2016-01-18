using UnityEngine;
using System.Collections;

public class UIGauge : MonoBehaviour {

	float m_currentVal;			// Current integer value of gauge
	float m_totalVal;			// Total integer value of gauge
	float m_prevVal;			// Stores the previous value of the health

	public float speed;


	bool damaged = false;		
	[SerializeField]UISlider	m_sliderGreen;
	[SerializeField]UISlider	m_sliderRed;

	const float 	animTime = 0.5f;


	#region Mono

	// Update is called once per frame
	void Update ()  
	{
		if(m_prevVal > m_currentVal)
		{
			m_prevVal -= (m_totalVal/m_currentVal) * speed * Time.deltaTime;
			m_sliderGreen.sliderValue = m_currentVal/m_totalVal;
			m_sliderRed.sliderValue = m_prevVal/m_totalVal;
		}
		else
		{
			m_prevVal = m_currentVal;
			m_sliderRed.sliderValue = m_prevVal/m_totalVal;
		}


	}
	#endregion

	#region public
	public void Init(int curVal, int totalVal)
	{
		m_currentVal = curVal;
		m_totalVal = totalVal;
		m_prevVal = totalVal;
		speed = (m_totalVal / 10); // affects the health drainage
		if(m_totalVal != 0)
			m_sliderGreen.sliderValue = ((float)m_currentVal/m_totalVal);
	}

	public void reduce(int damage)
	{
		Debug.Log("Damaged");
		m_currentVal -= damage;
	}
	#endregion


}
