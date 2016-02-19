using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour
{
    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
    private bool sceneStarting = true;      // Whether or not the scene is still fading in.
    private bool changeScene = false;

    private string sceneName = "";
    private int sceneIndex = 0;

    [SerializeField]UITexture guiTex;
    
    
    void Awake ()
    {
		if(guiTex != null)
       	// Set the texture so that it is the the size of the screen and covers it.
       	guiTex.transform.localScale = new Vector3(640, 960, 1);
    }

    IEnumerator FadeToClear ()
    {
        // Lerp the colour of the texture between itself and transparent.
        while(guiTex.color.a > 0.05f)
        {
			guiTex.color = Color.Lerp(guiTex.color, Color.clear, fadeSpeed * Time.deltaTime);
			yield return null;
		}

		// ... set the colour to clear and disable the GUITexture.
		guiTex.color = Color.clear;
		guiTex.enabled = false;
        
        // The scene is no longer starting.
        sceneStarting = false;
    }
    
    
    IEnumerator FadeToBlack (string name)
    {
    	while(guiTex.color.a < 0.95f)
    	{
	        // Lerp the colour of the texture between itself and black.
			guiTex.color = Color.Lerp(guiTex.color, Color.black, fadeSpeed * Time.deltaTime);
			yield return null;
		}
		Application.LoadLevel(name);
    }
	IEnumerator FadeToBlack (int index)
    {
    	while(guiTex.color.a < 0.95f)
    	{
	        // Lerp the colour of the texture between itself and black.
			guiTex.color = Color.Lerp(guiTex.color, Color.black, fadeSpeed * Time.deltaTime);
			yield return null;
		}
		Application.LoadLevel(index);
    }
    
    
    public void StartScene ()
    {
        // Fade the texture to clear.
        StartCoroutine(FadeToClear());
    }
    
    
    public void ChangeScene (string name)
    {
        // Make sure the texture is enabled.
		guiTex.enabled = true;
		sceneName = name;
		// Start fading towards black.
        StartCoroutine(FadeToBlack(name));
    }

    public void ChangeScene(int index)
    {
		// Make sure the texture is enabled.
		guiTex.enabled = true;
		sceneIndex = index;

		StartCoroutine(FadeToBlack(index));
    }
}