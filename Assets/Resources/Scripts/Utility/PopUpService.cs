using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopUpService : CSingleton {

	Stack<GameObject>		m_popupStack = new Stack<GameObject>();	
	int 					m_currentLayer = 14; // popup layer
	int 					m_currentDepth = 5;

	#region private
	GameObject CreatePopup(string prefabPath, bool aboveEverthing = false)
	{
		var popup = Instantiate( Resources.Load("Prefabs/Popup/" + prefabPath) ) as GameObject;
		
		if(!popup.activeSelf)
		{
			popup.SetActive(true);
		}	
		var camera 		= popup.GetComponentInChildren<Camera>();
		var uiCamera	= popup.GetComponentInChildren<UICamera>();
			
		if(camera != null)
		{
			foreach (Transform trans in popup.GetComponentsInChildren<Transform>(true))
			{
				trans.gameObject.layer = m_currentLayer;
			}
			camera.cullingMask = (1 << m_currentLayer);
			camera.depth	   = aboveEverthing ? 56 : m_currentDepth; // highest path
			camera.clearFlags  = CameraClearFlags.Depth;
		}

		if(uiCamera != null)
		{
			uiCamera.eventReceiverMask = (1 << m_currentLayer);
		}

		m_currentLayer++;
		m_currentDepth++;

		m_popupStack.Push(popup); 

		return popup;
	}
	#endregion

	#region public
//	public void ShowGeneralPopup(string title,
//	                             string message,
//	                             ErrorType errorType = ErrorType.OtherErrors,
//	                             string buttonName1 = "",
//	                             GumiAction buttonAction1 = null,
//	                             string buttonName2 = "",
//	                             GumiAction buttonAction2 = null,
//	                             bool autoClose = false ,
//	                             bool top = false)
//	{
//		var popup = CreatePopup("General_Popup", top);
//
//		if(popup != null)
//		{
//			var handler = popup.GetComponentInChildren<GeneralPopupHandler>();
//			handler.Init(title, message, errorType, buttonName1, buttonAction1, buttonName2, buttonAction2, autoClose);
//		}
//	}

	public void ShowShop(ShopType type = ShopType.Gems)
	{
		GameObject popup = CreatePopup("ShopPopup", false);

		if(popup != null)
		{
		 	popup.GetComponentInChildren<ShopHandler>().Init(type);
		}
	}

	public void CloseTopestPopup()
	{
		if (m_popupStack != null && m_popupStack.Count > 0)
		{
			var popup = m_popupStack.Pop();
			if (popup != null)
			{
				m_currentDepth--;
				m_currentLayer--;
				Destroy(popup);
				popup = null;
			}
		}
	}
	public void CloseAllPopups()
	{
		while(m_popupStack.Count > 0)
		{
			var popup = m_popupStack.Pop();
			m_currentDepth--;
			m_currentLayer--;
			Destroy(popup);
		}
	}
	public GameObject GetTopestPopup()
	{
		if(m_popupStack.Count == 0)
			return null;
		else
			return m_popupStack.Peek();
	}

	#endregion

}
