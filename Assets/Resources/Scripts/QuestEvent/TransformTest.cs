using UnityEngine;
using System.Collections;

public class TransformTest : MonoBehaviour {
	
	HUDService m_hudService;
	// Use this for initialization
	void Start () {
		Service.Init();	
		m_hudService = Service.Get<HUDService>();
		m_hudService.StartScene();
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTest(){
		GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/TransformAnim")) as GameObject;
		m_hudService.HUDControl.AttachMid(ref obj);
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3(0, 0, -5);
	}

	void OnBattleTest(){
		GameObject obj = Instantiate( Resources.Load ("Prefabs/Event/BattleStart")) as GameObject;
		m_hudService.HUDControl.AttachMid(ref obj);
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3(0, 0, -5);
		
		BattleStart battleDialog = obj.GetComponent<BattleStart>();
		battleDialog.Initialize(gameObject, "Mirror Monster");
	}
}
