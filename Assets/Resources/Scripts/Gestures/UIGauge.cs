﻿using UnityEngine;
using System.Collections;

public class UIGauge : MonoBehaviour {

	float m_currentVal;			// Current integer value of gauge
	float m_totalVal;			// Total integer value of gauge
	float m_prevVal;			// Stores the previous value of the health

	public float speed;

	[SerializeField]UISlider	m_sliderGreen;
	[SerializeField]UISlider	m_sliderRed;
	[SerializeField]UILabel		m_name;

	[SerializeField]UISprite	m_iconSprite;

	const float 	animTime = 0.5f;

	bool UIInitialized = false;



	#region Mono

	// Update is called once per frame
	void Update ()  
	{
		if(!UIInitialized )
			return;

		if(m_prevVal > m_currentVal)
		{
			m_prevVal -= (m_totalVal/m_currentVal) * speed * Time.deltaTime;
			m_sliderGreen.sliderValue = m_currentVal/m_totalVal;
			m_sliderRed.sliderValue = m_prevVal/m_totalVal;
		}
		else
		{
			m_prevVal = m_currentVal;
			m_sliderGreen.sliderValue = m_currentVal/m_totalVal;
			m_sliderRed.sliderValue = m_prevVal/m_totalVal;
		}


	}
	#endregion

	#region public
	public void Init(float curVal, float totalVal, string name = "")
	{
		UIInitialized = true;
		m_currentVal = curVal;
		m_totalVal = totalVal;
		m_prevVal = totalVal;
		speed = (m_totalVal / 10); // affects the health drainage
		if(m_totalVal != 0)
			m_sliderGreen.sliderValue = ((float)m_currentVal/m_totalVal);

		gameObject.name = name;
		if (m_name != null) {
			m_name.text = name;
			if (name.Contains("Mirror Monster")){
				m_iconSprite.spriteName = "Battle_Enemy_Portrait_Mirror";
			}else if (name.Contains("Rat Monster")){
				m_iconSprite.spriteName = "Battle_Enemy_Portrait_Rat";
			}
		}
	}

	public void reduce(float damage)
	{
		m_currentVal -= damage;
		if (m_currentVal < 0)
			m_currentVal = 0;
	}

	public void gain(float hp)
	{
		m_currentVal += hp;
	}

	#endregion


}
