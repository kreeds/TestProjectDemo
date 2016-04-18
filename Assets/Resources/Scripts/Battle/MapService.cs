using UnityEngine;
using System.Collections;

public class MapService : CSingleton {

	GameObject m_cameraObj;
	TweenPosition m_tweenPos;
	TextureAnimation m_effectFx;


	public void Init()
	{
		m_cameraObj = Camera.main.gameObject;

		m_effectFx = m_cameraObj.GetComponentInChildren<TextureAnimation>();
		m_effectFx.gameObject.SetActive(false);

		if(m_cameraObj != null)
			m_tweenPos = m_cameraObj.GetComponent<TweenPosition>();
	}

	public void TweenPos(Vector3 from, Vector3 to, TweenPosition.Method method, TweenPosition.Style style, GameObject obj, string eventmethod)
	{
		if(m_tweenPos == null)
			return;

		m_tweenPos.Reset();

		// Set Up New Tween
		m_tweenPos.from = from;
		m_tweenPos.to = to;
		m_tweenPos.method = method;
		m_tweenPos.style = style;
		m_tweenPos.eventReceiver = obj;
		m_tweenPos.callWhenFinished = eventmethod;

		// Play Tween
		m_tweenPos.Play(true);

		Vector3 diff = to - from; 

		// Create FX
		if(m_effectFx != null)
		{
			m_effectFx.Reset();

			if(diff.x < 0)
			{
				m_effectFx.Forward(false);
			}
			else
			{
				m_effectFx.Forward(true);
			}
			m_effectFx.gameObject.SetActive(true);
		}


	
	}
}
