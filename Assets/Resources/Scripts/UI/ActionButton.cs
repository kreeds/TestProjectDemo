using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour {

	[SerializeField]TweenAlpha alpha;
	[SerializeField]TweenScale scale;
	[SerializeField]BoxCollider box;

	public void PlayTween(bool forward)
	{
		box.enabled = forward;
		alpha.Play(forward);
		scale.Play(forward);

		if(!forward)
		{
			scale.eventReceiver = this.gameObject;
			scale.callWhenFinished = "OnShrink";
		}
	}

	public void OnShrink()
	{
		scale.eventReceiver = null;
		scale.callWhenFinished = null;
		transform.localScale = new Vector3(0,0,0);

	}
}
