using UnityEngine;
using System.Collections;

public class WardrobeItem : MonoBehaviour {

	public enum IconType
	{
		GOLD,
		PREMIUM,
		LOCKED,
	};

	[SerializeField]UITexture			_previewTexture;

	[SerializeField]UISprite			_itemCurrencyIcon;
	[SerializeField]UILabel				_itemCost;

	private GameObject					_rootObject;

	private int							_itemID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(IconType iconType, int itemCost, string resourceLocation, int itemID, GameObject rootObject){
		_previewTexture.mainTexture = Resources.Load (resourceLocation) as Texture;

		if (iconType != IconType.LOCKED)
			_itemCost.text = itemCost.ToString ();
		else
			_itemCost.text = "LV" + itemCost;

		_rootObject = rootObject;

		if (iconType == IconType.GOLD) {
			_itemCurrencyIcon.spriteName = "icon02";
		} else if (iconType == IconType.PREMIUM) {
			_itemCurrencyIcon.spriteName = "icon03";
		} else if (iconType == IconType.LOCKED) {
			_itemCurrencyIcon.spriteName = "UI_cos_lock";
			collider.enabled = false;
		}

		_itemID = itemID;
	}

	void OnClick()
	{
		_rootObject.SendMessage ("OnCostumeItem", _itemID);
	}
}
