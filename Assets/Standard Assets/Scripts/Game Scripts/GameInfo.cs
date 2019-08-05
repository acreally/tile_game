// Author: Andrew Creally
// Project: Confusing Square Game

using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour
{
    // Prefab used to instantiate new tile object when player clicks
    // on a game square
    public GameObject tile;
    // True if it is player one's turn to place a tile
    bool playerOneTurn = false;
    // True if it is player two's turn to place a tile
    bool playerTwoTurn = false;
    // True if a new tile for player one should be instantiated
    bool makePlayerOneTile = true;
    // True if a new tile for player one should be instantiated
    bool makePlayerTwoTile = false;
    // True if a tile was placed on a game square in the previous frame
    bool tilePlaced = false;
    // The main camera for the scene
    Camera mainCamera;
    // The layer mask of the game squares
    int gameSquareLayer;
    // The current square that the cursor is above
    GameObject currSquare;
    // The previous square that the cursor was above
    GameObject prevSquare;
    // A new game tile object
    GameObject newTile;
    // The number of game squares occupied
    int squaresOccupied = 0;
    // The total number of game squares on the playing surface
    int totalSquares = 25;
    // Adjacency list used to perform breadth-first search when placing
    // a tile.
    GameObject[] adjacencyList = new GameObject[25];
    // True if the final tile has been placed on the board
    bool gameOver = false;
    // The first player's score
    int playerOneScore = 0;
    // The second player's score
    int playerTwoScore = 0;
    // Background for when player one wins
    public Texture2D redWin;
    // Background for when player two wins
    public Texture2D blueWin;
    // Used to leave a delay between when the last tile is placed
    // and the end game screen is displayed
    float timer = 0.0f;
    // The text displaying the score of player one
    public TextMesh playerOneScoreText;
    // The text displaying the score of player two
    public TextMesh playerTwoScoreText;
    // The style for the end game buttons
    public GUIStyle buttonStyle;

    void Awake()
    {
        playerOneScoreText.renderer.material.color = Color.red;
        playerTwoScoreText.renderer.material.color = Color.blue;
    }

    void Start()
    {
        // Do not want to have to find the main camera every time
        // we need to access it
        mainCamera = Camera.main;
        // Need to bit shift layer mask.
        gameSquareLayer = 1 << LayerMask.NameToLayer("GameSquare");
    }

    /*
     * Make a new tile.  Get the cursor position in world coordinates
     * and hightlight a game square, if applicable.  Place a game tile on
     * a game square when the user clicks the left mouse button.
     * Need to do all this in fixed update so that collision detection
     * of number squares occurs before scores of tiles are calculated.
     */
    void FixedUpdate()
    {
        // Create a new tile for the appropriate player
        if (makePlayerOneTile)
        {
            MakeNewTile(1);
        }
        else if (makePlayerTwoTile)
        {
            MakeNewTile(2);
        }

        // Get the position of the cursor in world coordinates
        Vector3 cursorPos = mainCamera.ScreenToWorldPoint(new
            Vector3(Input.mousePosition.x, Input.mousePosition.y,
            mainCamera.farClipPlane));
        cursorPos.y = 10;

        // Check if the cursor is above a game square
        RaycastHit hit;
        if (Physics.Raycast(cursorPos, -Vector3.up, out hit,
            100.0f, gameSquareLayer))
        {
            prevSquare = currSquare;
            currSquare = hit.transform.gameObject;
            HighlightSquare();
            // When the player clicks the left mouse button, place a game
            // tile on an unoccupied game sqaure.
            if (Input.GetMouseButtonDown(0))
            {
                if (newTile)
                {
                    GameSquareBehaviour squareScript =
                        currSquare.GetComponent<GameSquareBehaviour>();
                    if (!squareScript.Occupied)
                    {
                        PlaceTile(squareScript);
                    }
                }
            }
        }
    }

    /*
     * Update the score of the tiles if a tile was placed.
     * Switch turns if not all game squares are over. Otherwise,
     * move to the game over state.
     */
    void Update()
    {
        if (tilePlaced)
        {
            tilePlaced = false;
            DepthFirstSearch();             
            newTile = null;
            playerOneScore = 0;
            playerTwoScore = 0;
            CalculateScore();
            playerOneScoreText.text = "" + playerOneScore;
            playerTwoScoreText.text = "" + playerTwoScore;
            if (squaresOccupied != totalSquares)
            {
                SwitchTurns();
            }
            else
            {
                gameOver = true;
            }
        }
        
    }

    /*
     * Place a game tile on an unoccupied game square and update the
     * adjacency list of game tiles.
     */
    void PlaceTile(GameSquareBehaviour squareScript)
    {
        newTile.transform.position =
                            currSquare.transform.position + Vector3.up;
        adjacencyList[squareScript.IDNum] = newTile;
        newTile.GetComponent<TileBehaviour>().IDNum =
            squareScript.IDNum;
        squareScript.Occupied = true;
        squaresOccupied++;
        tilePlaced = true;
    }

    /*
     * Initialize each tile in the adjacency list for a depth first search.
     */
    void DepthFirstSearch()
    {
        foreach (GameObject gameTile in adjacencyList) 
        {
            if (gameTile)
            {
                TileBehaviour tileScript = gameTile.GetComponent<
                    TileBehaviour>();
                tileScript.Colour = 0;
            }            
        }
        DFSVisit(newTile.GetComponent<TileBehaviour>());
    }

    /*
     * Visit all the neighbouring tiles of the root tile,
     * if the neighbours belong to the other player, in a depth first
     * search fashion.
     */
    void DFSVisit(TileBehaviour tileScript)
    {
        // Will switch the owner of the game tile, if necessary
        tileScript.UpdateScore();
        // Create an array of the list of neighbouring game tiles
        GameObject[] neighbours = new GameObject[4];
        int i = 0;
        foreach (GameObject numSqur in tileScript.NumberSquares)
        {
            GameObject adjSqur = numSqur.GetComponent<
                NumberSquareBehaviour>().AdjacentNumSqur;
            if (adjSqur)
            {
                neighbours[i] = adjacencyList[adjSqur.GetComponent<
                NumberSquareBehaviour>().GetIDNumber()];
            }
            else
            {
                neighbours[i] = null;
            }            
            i++;            
        }
        tileScript.Colour = 1;
        // If the owner of the neighbouring game tile is not the owner
        // of this game tile, update its score
        foreach (GameObject neighbour in neighbours)
        {         
            if (neighbour)
            {                
                TileBehaviour nextTile = neighbour.GetComponent<
                    TileBehaviour>();
                if (nextTile.Owner != tileScript.Owner && 
                    nextTile.Colour == 0)
                {
                    DFSVisit(nextTile);
                }
            }
        }
    }

    /*
     * Switch the boolean values that determine whose turn it is. 
     */
    void SwitchTurns()
    {
        if (playerOneTurn)
        {
            makePlayerTwoTile = true;
            playerOneTurn = false;
        }
        else if (playerTwoTurn)
        {
            makePlayerOneTile = true;
            playerTwoTurn = false;
        }
    }

    /*
     * Make a new game tile object for the appropriate player and place
     * it in the GUI.
     */
    void MakeNewTile(int player)
    {
        Vector3 pos;
        if (player == 1)
        {
            pos = new Vector3(-180, 4, 320);
            playerOneTurn = true;
            makePlayerOneTile = false;
        }
        else if (player == 2)
        {
            pos = new Vector3(-180, 4, -330);
            playerTwoTurn = true;
            makePlayerTwoTile = false;
        }
        else
        {
            Debug.Log("Invalid player");
            return;
        }

        newTile = (GameObject)
                Instantiate(tile, pos, Quaternion.identity);
        newTile.GetComponent<TileBehaviour>().ChangeColour(player);
        newTile.GetComponent<TileBehaviour>().Owner = player;
    }

    /*
     * Highlight the game square below the cursor. 
     */
    void HighlightSquare()
    {
        if (prevSquare)
        {
            if (prevSquare != currSquare)
            {
                prevSquare.GetComponent<GameSquareBehaviour>().Unhighlight();
                currSquare.GetComponent<GameSquareBehaviour>().Highlight();
            }
        }
        else
        {
            currSquare.GetComponent<GameSquareBehaviour>().Highlight();
        }
        
    }

    /*
     * Once all game squares are occupied, display a screen declaring
     * which player won.  Allow players to either play again or quit
     * the game.
     */
    void OnGUI()
    {
        if (gameOver)
        {            
            if (timer >= 2.0f)
            {
                Texture2D endGameBG;
                CalculateScore();
                if (playerOneScore > playerTwoScore)
                {
                    endGameBG = redWin;
                }
                else
                {
                    endGameBG = blueWin;
                }
                GUI.skin.box.normal.background = endGameBG;
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
                if (GUI.Button(new Rect(Screen.width * 0.5f - 128, 
                    Screen.height * 0.5f, 256, 96), "Play Again",
                    buttonStyle))
                {
                    Application.LoadLevel(1);
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f - 128,
                    Screen.height * 0.5f + 100, 256, 96), "Quit",
                    buttonStyle))
                {
                    Application.Quit();
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    /*
     * Determine the score of both players.
     */
    void CalculateScore()
    {
        foreach (GameObject tile in adjacencyList)
        {
            if (tile)
            {
                if (tile.GetComponent<TileBehaviour>().Owner == 1)
                {
                    playerOneScore++;
                }
                else if (
                    tile.GetComponent<TileBehaviour>().Owner == 2)
                {
                    playerTwoScore++;
                }
                else
                {
                    Debug.Log("Incorrect player number when trying " +
                        "to calculate final score.");
                }
            }
        }
    }
}