using UnityEngine;
using System.Collections;

public class BBStart : MonoBehaviour {

	bool m_pressing = true;				// if flag is false, player has let go of touch
	[SerializeField]Camera m_camera;
	[SerializeField]Transform m_pos;
	[SerializeField]float	m_threshold = 0.12f;
	Vector2 cursorPos;


	BattleManager m_bmgr;
	PlayerManager m_pmgr;

	void Start()
	{
		m_bmgr = BattleManager.Get();
		m_pmgr = PlayerManager.Get();
		GameObject obj = GameObject.Find("MapCam");
		m_camera = obj.GetComponent<Camera>();
	}

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


			//GameObject obj = Instantiate(Resources.Load("Prefabs/Battle/HPBar")) as GameObject;

			// For some reason camera transform is not taken into account when transforming from screen pos to world pos;
			pos.x += Camera.main.transform.position.x;
//
//			obj.transform.position = pos;
//			obj.transform.parent = transform;
//			obj.transform.localScale = Vector3.one;

			if((pos-m_pos.position).magnitude < m_threshold)
			{
				Debug.Log("Within Radius");
				m_bmgr.Correct();
				m_pmgr.ShowSpecialEffect();
			}


		}
	}
	void OnPress(bool pressing)
	{
		Debug.Log("Pressing: " + pressing);
		m_pressing = pressing;
	}
}
