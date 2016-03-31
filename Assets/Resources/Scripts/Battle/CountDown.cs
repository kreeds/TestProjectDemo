using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour {

	[SerializeField]float 		m_duration = 0.5f;
	//[SerializeField]TweenAlpha	m_alpha;

	Transform				m_HUD;
	BattleManager			m_bmgr;
	SpriteRenderer			m_renderer;

	Sprite[]				m_sprites;

	Color					m_originalColor;
	float					m_countdown;
	bool 					m_disable;					// Disable flag 

	// Use this for initialization
	void Start () {

		m_bmgr = BattleManager.Get();
		m_countdown = m_bmgr.m_specialCountDown; // Gets the seconds to count down

		m_disable = false;

		m_HUD = Service.Get<HUDService>().HUDControl.transform;
		m_sprites = Resources.LoadAll<Sprite>("Sprite/number");

		m_renderer = GetComponent<SpriteRenderer>();
		StartCoroutine(UpdateCount());

	}


	IEnumerator UpdateCount()
	{
		while(true)
		{
			ShowCount();
			yield return new WaitForSeconds(1.0f);
		}
	}

	void ShowCount()
	{
		if(m_countdown < 1)
			return;

		m_renderer.sprite = m_sprites[System.Convert.ToInt32(m_countdown-1)];
		m_countdown -= 1;

		StartCoroutine(Utility.AlphaInOut(this, m_renderer.material, m_duration, m_duration));

	}
}
