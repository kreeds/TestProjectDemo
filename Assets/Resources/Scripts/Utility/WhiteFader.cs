using UnityEngine;
using System.Collections;

public class WhiteFader : MonoBehaviour {

	[SerializeField]float fadeSpeed = 1.0f;
	[SerializeField]UITexture	texture;

	// Use this for initialization
	void Start () {

		texture.color = Color.clear;
		StartCoroutine(FadeToWhite());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator FadeToWhite ()
    {
		while(texture.color.a < 0.95f)
    	{
	        // Lerp the colour of the texture between itself and black.
			texture.color = Color.Lerp(texture.color, Color.white, fadeSpeed * Time.deltaTime);
			yield return null;
		}

		yield return StartCoroutine(FadeToClear());
    }
    IEnumerator FadeToClear()
    {
		while(texture.color.a > 0.05f)
        {
			texture.color = Color.Lerp(texture.color, Color.clear, fadeSpeed * Time.deltaTime);
			yield return null;
		}

		// ... set the colour to clear and disable the GUITexture.
		texture.color = Color.clear;

		Destroy(this.gameObject);
    }
}
