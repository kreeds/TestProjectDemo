﻿using UnityEngine;
using System.Collections;

public class BackGroundDragControl : MonoBehaviour {

	public UIPanel panel;

	// Use this for initialization
	void Start () {
		if(panel != null)
		{
			panel.clipRange = new Vector4(panel.clipRange.x, panel.clipRange.y, 640, 1152);

			GameObject obj = Instantiate (Resources.Load ("Prefabs/CameraBar")) as GameObject;

			UIScrollBar scrollbar = obj.GetComponent<UIScrollBar> ();

			HUDHandler hudHandler = Service.Get<HUDService>().HUDControl;
			hudHandler.AttachMid(ref obj);
			hudHandler.SetCameraScrollBar(scrollbar);

			obj.transform.localPosition = new Vector3 (-288f, -420f); 
			obj.transform.localScale = Vector3.one;
			panel.GetComponent<UIDraggablePanel> ().AttachHorizontalScrollBar(scrollbar);
		}
	}
}
