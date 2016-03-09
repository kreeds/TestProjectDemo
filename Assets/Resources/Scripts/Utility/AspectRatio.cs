using UnityEngine;
using System.Collections;

public class AspectRatio : MonoBehaviour {

	int designedHeight = 960;
	int designedWidth = 640;
	void Awake()
    {
    	UnityEngine.Debug.Log("RUNNING ASPECT RATIOÍ");
        var uiroot = transform.parent.GetComponent<UIRoot>();
        if(uiroot != null)
        {
        	uiroot.manualHeight = RootScreenHeight();
        }
        		
    }

	public int RootScreenHeight()
	{
		if ((float)Screen.height / (float)Screen.width > (float)designedHeight / (float)designedWidth)
		{
			return Mathf.FloorToInt((float)designedWidth / Screen.width * Screen.height);
		}
		else
		{
			return designedHeight;
		}
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
