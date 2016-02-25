using UnityEngine;
using System.Collections;

public class CameraService : CSingleton {

	Camera m_camera;
	TweenPosition m_tweenPos;

	void Awake()
	{
		GameObject cameraObj = GameObject.Find("MainCamera");
		if(cameraObj != null)
			m_tweenPos = cameraObj.GetComponent<TweenPosition>();
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

	}
}
