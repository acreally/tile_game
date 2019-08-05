using UnityEngine;
using System.Collections;

public class Instructions : MonoBehaviour {
    // Background for the instructions screen
    public Texture2D background;
    // The style for the back button
    public GUIStyle backButtonStyle;

    void OnGUI()
    {
        GUI.skin.box.normal.background = background;
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        if (GUI.Button(new Rect(
            Screen.width - 64, Screen.height - 32, 64, 32), "Back", 
            backButtonStyle))
        {
            // Return to the title screen
            Application.LoadLevel(0);
        }
    }
}
