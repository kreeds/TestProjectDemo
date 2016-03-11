﻿using UnityEngine;
using System.Collections;

public class BackGroundDragControl : MonoBehaviour {

	public UIPanel panel;

	// Use this for initialization
	void Start () {
		if(panel != null)
		{
			panel.clipRange = new Vector4(panel.clipRange.x, panel.clipRange.y, 640, 960);

			
			GameObject obj = Instantiate (Resources.Load ("Prefabs/CameraBar")) as GameObject;
			
			Service.Get<HUDService>().HUDControl.AttachMid(ref obj);
			obj.transform.localPosition = new Vector3 (-250f, -300f);
			obj.transform.localScale = Vector3.one;
			panel.GetComponent<UIDraggablePanel> ().AttachHorizontalScrollBar(obj.GetComponent<UIScrollBar> ());
		}
	}
}
