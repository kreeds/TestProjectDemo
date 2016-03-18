using UnityEngine;
using System.Collections;

public class BBStart : MonoBehaviour {

	bool m_pressing = true;				// if flag is false, player has let go of touch
	[SerializeField]Camera m_camera;
	[SerializeField]Transform m_pos;
	[SerializeField]float	m_threshold = 0.12f;
	Vector2 cursorPos;

	void Update()
	{
		cursorPos = new Vector2();

		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);	
			if(touch.phase == TouchPhase.Ended)
				cursorPos = touch.position;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			cursorPos = Input.mousePosition;

		}
		if(!m_pressing)
		{
			
			m_pressing = true; // reset flag
			// Compute Radius Check

			Vector3 pos = m_camera.ScreenToWorldPoint(cursorPos);
			Vector3 diff = pos - m_pos.position;

			if((pos-m_pos.position).magnitude < m_threshold)
			{
				Debug.Log ("Within Radius");
			}


		}
	}
	void OnPress(bool pressing)
	{
		Debug.Log("Pressing: " + pressing);
		m_pressing = pressing;
	}
}
