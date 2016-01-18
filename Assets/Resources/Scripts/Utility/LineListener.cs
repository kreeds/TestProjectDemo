using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class Draws line via input points from mouse
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LineListener : MonoBehaviour {

	List<Vector3> 	m_points =  new List<Vector3>();
	LineRenderer	m_linerenderer;
	int				m_linecount; // count to track number of lines drawn
	Camera 			m_camera;
	Vector3 		m_lastPos = Vector3.one * float.MaxValue;

	public float	m_threshold = 0.1f; 		// 

	bool 			m_toCommence;
	 

	void Awake()
	{
		m_linerenderer = GetComponent<LineRenderer>();
		m_points = new List<Vector3>();
		m_camera = Camera.main;
		m_toCommence = true;
	}

	void Update()
	{
		if(!m_toCommence)
			return;

		if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosInScreen = Input.mousePosition;
			mousePosInScreen.z = m_camera.nearClipPlane;
			Vector3 mousePosInWorld = m_camera.ScreenToWorldPoint(mousePosInScreen);

			// Compute Distance
			float dist = Vector3.Distance(m_lastPos, mousePosInWorld);
			if(dist <= m_threshold)
				return;

			m_lastPos = mousePosInWorld;
			m_points.Add(mousePosInWorld);


			// Update Line Renderer
			UpdateLine();
		}
		else
		{
			m_toCommence = false;
		}
	}

	/// <summary>
	/// Method to activate Line Renderer to draw on screen
	/// </summary>
	void UpdateLine()
	{
		// Check if lineRenderer Exist
		if(m_linerenderer == null)
			return;

		// Initialize number of points in the line
		m_linerenderer.SetVertexCount(m_points.Count);

		for(int i = m_linecount; i < m_points.Count; ++i)
		{
			m_linerenderer.SetPosition(i, m_points[i]);
		}
		m_linecount = m_points.Count;
	}

		
}
