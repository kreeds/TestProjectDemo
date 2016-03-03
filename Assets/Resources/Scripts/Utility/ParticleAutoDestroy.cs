using UnityEngine;
using System.Collections;

public class ParticleAutoDestroy : MonoBehaviour {

	[SerializeField]ParticleSystem ps;
	// Use this for initialization
	void Start () {
		Destroy(this.gameObject, ps.duration);
	}
}
