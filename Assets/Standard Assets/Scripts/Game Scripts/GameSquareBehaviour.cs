// Author: Andrew Creally
// Project: Confusing Square Game

using UnityEngine;
using System.Collections;

public class GameSquareBehaviour : MonoBehaviour 
{
    // True iff a game tile is placed above this game square
    bool occupied = false;
    // The mesh renderer of this game square
    MeshRenderer meshRenderer;
    // The unhighlighted material of a game square
    public Material normalMaterial;
    // The highlighted material of a game square
    public Material highlightMaterial;
    // Unique identifiaction number for a game square
    int idNum;
    // Static variable used to increment idNum of each game sqaure
    static int counter;

    /*
     * Property for idNum field. 
     */
    public int IDNum
    {
        get { return idNum; }
    }

    /*
     * Property for occupied field.
     */
    public bool Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }

    void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        counter = 0;
    }
    
    void Start() 
    {
        idNum = counter++;
	}

    /*
     * Change the material of the mesh renderer to the highlighted 
     * material.  Play a sound when cursor enters an unoccupied
     * game square.
     */
    public void Highlight()
    {        
        if (!occupied)
        {
            meshRenderer.material = highlightMaterial;
            // Audio clip from http://www.flashkit.com/soundfx/Interfaces/Beeps/Low_Beep-Public_D-136/index.php
            // public domain clip
            audio.Play();
        }        
    }

    /*
     * Change the material of the mesh renderer to the unhighlighted 
     * material.
     */
    public void Unhighlight()
    {
        meshRenderer.material = normalMaterial;
    }
}