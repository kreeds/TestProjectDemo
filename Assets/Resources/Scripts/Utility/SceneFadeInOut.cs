using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour
{
    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
    private bool sceneStarting = true;      // Whether or not the scene is still fading in.

    [SerializeField]UITexture guiTex;
    
    
    void Awake ()
    {
		if(guiTex != null)
       	// Set the texture so that it is the the size of the screen and covers it.
       	guiTex.transform.localScale = new Vector3(Screen.width+10, Screen.height, 1);
    }
    
    
    void Update ()
    {
        // If the scene is starting...
        if(sceneStarting)
            // ... call the StartScene function.
            StartScene();
    }
    
    
    void FadeToClear ()
    {
        // Lerp the colour of the texture between itself and transparent.
		guiTex.color = Color.Lerp(guiTex.color, Color.clear, fadeSpeed * Time.deltaTime);
    }
    
    
    void FadeToBlack ()
    {
        // Lerp the colour of the texture between itself and black.
		guiTex.color = Color.Lerp(guiTex.color, Color.black, fadeSpeed * Time.deltaTime);
    }
    
    
    void StartScene ()
    {
        // Fade the texture to clear.
        FadeToClear();
        
        // If the texture is almost clear...
		if(guiTex.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the GUITexture.
			guiTex.color = Color.clear;
			guiTex.enabled = false;
            
            // The scene is no longer starting.
            sceneStarting = false;
        }
    }
    
    
    public void ChangeScene (string name)
    {
        // Make sure the texture is enabled.
		guiTex.enabled = true;
        
        // Start fading towards black.
        FadeToBlack();
        
        // If the screen is almost black...
		if(guiTex.color.a >= 0.95f)
            // ... reload the level.
			Application.LoadLevel(name);
    }

    public void ChangeScene(int sceneIndex)
    {
		// Make sure the texture is enabled.
		guiTex.enabled = true;
        
        // Start fading towards black.
        FadeToBlack();
        
        // If the screen is almost black...
		if(guiTex.color.a >= 0.95f)
            // ... reload the level.
			Application.LoadLevel(sceneIndex);
    }
}