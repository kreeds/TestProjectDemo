using UnityEngine;
using System.Collections;

public class EffectsController : MonoBehaviour {

	public float lifetime = 2.0f;

	public TweenAlpha alpha;

	void OnFadeInComplete()
	{
		StartCoroutine(Destruction());
	}

	// Update is called once per frame
	void Update()
	{

	}
	IEnumerator Destruction()
	{
		yield return new WaitForSeconds(lifetime);
		Destroy(gameObject);
	}
}
	
