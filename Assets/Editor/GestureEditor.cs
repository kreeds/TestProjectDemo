//C# Example
using UnityEditor;
using UnityEngine;


public class GestureEditor : EditorWindow
{
	string Path, filename, gesturename;
    string miniLabel = "Please run the game and ensure gesture methods are registered";

    bool groupEnabled;
    bool  saveToggle;

    bool saveRegistered;
    float myFloat = 1.23f;
    
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Editor/My Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GestureEditor));
    }

    public GestureEditor()
    {
    	Path = filename = gesturename = "";
    }


    void DisplaySave()
    {
		groupEnabled = EditorGUILayout.BeginToggleGroup( "Save Settings", groupEnabled);
		saveToggle = EditorGUILayout.Toggle("Toggle", saveToggle);

		gesturename = EditorGUILayout.TextField ("Gesture Name: ", gesturename);

    	if(GUILayout.Button("Save"))
    	{
			if(gesturename == "")
			{
				EditorUtility.DisplayDialog("Error", "Please enter a name for the Gesture", "Ok");
				return;
			}

			filename = EditorUtility.SaveFilePanel("Save File as XML", "Assets/Resources/TextData/GestureSet", "Untitled", "xml");
    		if(InputManager.Save != null)
    		{
    			InputManager.Save(filename, gesturename);
			}
    	}

		EditorGUILayout.EndToggleGroup ();
	

    }

    void OnGUI()
    {
		GUILayout.Label ("Gesture Editor", EditorStyles.boldLabel);

		saveRegistered = (InputManager.Save != null)? true : false;
		if(!saveRegistered)
    		GUILayout.Label (miniLabel, EditorStyles.miniLabel);
    	
		DisplaySave();

		Path = EditorGUILayout.TextField ("Path Field: ", Path);

		if(GUILayout.Button("Load"))
    	{
			if(InputManager.Load != null)
				InputManager.Load(Path);
    	}

//        
//        
//        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
//            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
//            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
//        EditorGUILayout.EndToggleGroup ();
    }
}
