using UnityEngine;
using System.Collections;

public class AspectRatio : MonoBehaviour {

	int designedHeight = 640;
	int designedWidth = 960;
	void Awake()
    {
    	UnityEngine.Debug.Log("RUNNING ASPECT RATIOÍ");
        var uiroot = transform.parent.GetComponent<UIRoot>();
        if(uiroot != null)
        {
        	uiroot.manualHeight = RootScreenHeight();
        }
        		
    }

    int RootScreenHeight()
    {
    	return designedHeight;
    }

    int RootScreenWidth()
    {
        if ((float)Screen.width / (float)Screen.height > (float)designedWidth / (float)designedHeight)
        {
            return Mathf.FloorToInt((float)designedHeight / Screen.height * Screen.width);
        }
        else
        {
            return designedWidth;
        }
    }
}
