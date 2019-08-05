using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour {
    // The background for the title menu
    public Texture2D background;
    // The style for the buttons on the title menu
    public GUIStyle buttonStyle;

    void OnGUI()
    {
        GUI.skin.box.normal.background = background; 
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        if (GUI.Button(new Rect(Screen.width * 0.5f - 128, 
            Screen.height * 0.5f, 256, 96), "Play", buttonStyle))
        {
            Application.LoadLevel(1);
        }
        if (GUI.Button(new Rect(Screen.width * 0.5f - 128, 
            Screen.height *0.5f + 100, 256, 96), "Instructions", 
            buttonStyle))
        {
            Application.LoadLevel(2);
        }
    }
}
