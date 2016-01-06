using UnityEngine;
using System.Collections;

public class UIGauge : MonoBehaviour {

	int m_currentVal;			// Current integer value of gauge
	int m_totalVal;				// Total integer value of gauge
	int m_val;					// Value to store damage or health restored


	bool damaged;			
	[SerializeField]UISlider	m_sliderGreen;
	[SerializeField]UISlider	m_sliderRed;

	const float 	animTime = 0.5f;


	#region Mono
	// Use this for initialization
	void Start () 
	{
		damaged = false;
	}
	
	// Update is called once per frame
	void Update ()  
	{
		if(damaged)
		{
			Debug.Log("******DAMAGED*****");
			m_currentVal -= m_val;
			m_sliderGreen.sliderValue = (m_currentVal/(float)m_totalVal);
			damaged = false;
		}
	}
	#endregion

	#region public
	public void Init(int curVal, int totalVal)
	{
		m_currentVal = curVal;
		m_totalVal = totalVal;
		if(m_totalVal != 0)
			m_sliderGreen.sliderValue = ((float)m_currentVal/m_totalVal);
	}

	public void reduce(int damage)
	{
		m_val = damage;
		damaged = true;
	}
	#endregion


}
