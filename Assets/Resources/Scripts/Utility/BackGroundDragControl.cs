using UnityEngine;
using System.Collections;

public class BackGroundDragControl : MonoBehaviour {

	public UIPanel panel;

	// Use this for initialization
	void Start () {
		if(panel != null)
		{
			panel.clipRange = new Vector4(panel.clipRange.x, panel.clipRange.y, Screen.width, 0);
		}
	}
}
