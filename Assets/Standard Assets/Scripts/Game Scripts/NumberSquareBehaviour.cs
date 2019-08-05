using UnityEngine;
using System.Collections;

public class NumberSquareBehaviour : MonoBehaviour 
{
    // The transform of this game object
    Transform thisTransform;
    // The value, from 1 to 4, inclusive, of this side of the tile.
    int val;
    // The materials that show the value of this number square.
    public Material one;
    public Material two;
    public Material three;
    public Material four;
    // Store all possible materials in an array and assign one material
    // to this number square, depending on its value.
    Material[] skins = new Material[4];
    // The number square, belonging to another game tile, that is 
    // adjacent to this number square
    GameObject adjacentNumSqur = null;

    /*
     * Property for adjacentNumSqur.
     */
    public GameObject AdjacentNumSqur
    {
        get { return adjacentNumSqur; }
        set { adjacentNumSqur = value; }
    }

    /*
     * Property for val field. 
     */
    public int Val
    {
        get { return val; }
        set { val = value; }
    }

    void Awake()
    {
        thisTransform = transform;
        val = Random.Range(1, 5);
        skins[0] = one;
        skins[1] = two;
        skins[2] = three;
        skins[3] = four;
    }

	void Start() 
    {
        // Set the material of this number square to correspond with
        // the random value
        this.GetComponent<MeshRenderer>().material = skins[val-1];
	}

    /*
     * Return the owner, player 1 or player 2, of the game tile this 
     * number square is attached to. 
     */
    public int GetOwner()
    {
        return thisTransform.parent.gameObject.GetComponent<
            TileBehaviour>().Owner;
    }

    /*
     * Return the ID number of the parent of this number square.
     */
    public int GetIDNumber()
    {
        return thisTransform.parent.gameObject.GetComponent<
            TileBehaviour>().IDNum;
    }

    /*
     * Check to see if this number square collides with another number
     * square, and if so, store a reference to the other number square.
     */
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Number"))
        {
            adjacentNumSqur = collision.gameObject;
            //Debug.Log("OnCollisionEnter of " + GetIDNumber());
        }
    }
}
