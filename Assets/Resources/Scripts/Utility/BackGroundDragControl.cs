using UnityEngine;
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
			hudHandler.AttachBottom(ref obj);
			hudHandler.SetCameraScrollBar(scrollbar);

			obj.transform.localPosition = new Vector3 (-145, 65f); 
			obj.transform.localScale = new Vector3(0.5f, 0.5f);
			panel.GetComponent<UIDraggablePanel> ().AttachHorizontalScrollBar(scrollbar);
		}
	}
}
