using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour {

	[SerializeField]TweenAlpha alpha;

	public void PlayTween(bool forward)
	{
		alpha.Play(forward);
	}
}
