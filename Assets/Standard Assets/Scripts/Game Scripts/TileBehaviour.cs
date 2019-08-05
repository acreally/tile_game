using UnityEngine;
using System.Collections;

public class TileBehaviour : MonoBehaviour
{
    // Prefab used to instantiate a new number square object
    public GameObject numberSquare;
    // The transform of this game object
    Transform thisTransform;
    // The mesh renderer of this game object
    MeshRenderer meshRenderer;
    // Red tile material
    public Material redTile;
    // Blue tile material
    public Material blueTile;
    // The score of this tile in relation to its neighbours
    //int score = 0;
    // The player that owns this tile.
    int owner;
    // An array of the number squares attached to this game tile
    GameObject[] numberSquares = new GameObject[4];
    // Unique identifiaction number for a game square
    int idNum;
    // Colour used in depth-first search of game tiles
    // 0 = white, 1 = black
    int colour;

    /*
     * Property for colour field.
     */
    public int Colour
    {
        get { return colour; }
        set { colour = value; }
    }

    /*
     * Property for numberSquares field.
     */
    public GameObject[] NumberSquares
    {
        get { return numberSquares; }
        set { numberSquares = value; }
    }

    /*
     * Property for idNum field. 
     */
    public int IDNum
    {
        get { return idNum; }
        set { idNum = value; }
    }

    /*
     * Property for owner field.
     */
    public int Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    void Awake()
    {
        // Do not want to have to search for transform every time
        // we need to access it
        thisTransform = transform;
        // Same as above
        meshRenderer = this.GetComponent<MeshRenderer>();
    }

    void Start()
    {
        // The positions of the number squares on this game tile.
        Vector3 topPos = thisTransform.position + (
            Vector3.forward * 35.5f) + Vector3.up;
        Vector3 botPos = thisTransform.position + 
            (Vector3.forward * (-35.5f)) + Vector3.up;
        Vector3 leftPos = thisTransform.position + 
            (Vector3.right * (-35.5f)) + Vector3.up;
        Vector3 rightPos = thisTransform.position + 
            (Vector3.right * 35.5f) + Vector3.up;

        numberSquares[0] = CreateNumberSquare(topPos);
        numberSquares[1] = CreateNumberSquare(rightPos);
        numberSquares[2] = CreateNumberSquare(botPos);
        numberSquares[3] =  CreateNumberSquare(leftPos);
    }

    /*
     * Return a new number square as a child of this game tile.
     */
    GameObject CreateNumberSquare(Vector3 pos)
    {
        GameObject number;
        number = (GameObject)Instantiate(numberSquare, pos, 
            Quaternion.identity);        
        number.transform.parent = thisTransform;
        return number;
    }

    void Update()
    {

    }

    /*
     * Update the score of this game tile. If the score is less than
     * or equal to -1, change the owner of this tile.
     */
    public void UpdateScore()
    {
        int score = 0;
        // For each number square attached to this game tile,
        // update the value of score
        foreach (GameObject numSqur in numberSquares) 
        {
            // This number square
            NumberSquareBehaviour numSqurScript =
                numSqur.GetComponent<NumberSquareBehaviour>();
            if (numSqurScript.AdjacentNumSqur)
            {
                // The number square on the adjacent game tile
                NumberSquareBehaviour adjSqurScript =
                    numSqurScript.AdjacentNumSqur.GetComponent<
                    NumberSquareBehaviour>();
                if (adjSqurScript.GetOwner() == owner)
                {
                    if (numSqurScript.Val < adjSqurScript.Val)
                    {
                        // Increase score
                        score += adjSqurScript.Val - numSqurScript.Val;
                    }
                }
                else
                {
                    if (numSqurScript.Val < adjSqurScript.Val)
                    {
                        // Decrease score
                        score += numSqurScript.Val - adjSqurScript.Val;
                    }                                      
                }
            }            
        }
        
        if (score <= -1)
        {
            if (owner == 1)
            {
                meshRenderer.material = blueTile;
                owner = 2;
            }
            else if (owner == 2)
            {
                meshRenderer.material = redTile;
                owner = 1;
            }
            else
            {
                Debug.Log("Error: incorrect player number: " + owner);
            }
        }
        //Debug.Log("UpdateScore of " + idNum + " : " + score);
    }

    /*
     * Change the material of the mesh renderer. 
     */
    public void ChangeColour(int player)
    {
        if (player == 1)
        {
            meshRenderer.material = redTile;            
        }
        else
        {
            meshRenderer.material = blueTile;
        }
    }
}