using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class MapManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 0;
    public int rows = 0;
    public float tileSize;
    public Count curvesCount = new Count(4, 8);
    public Count straightCount = new Count(4, 8);
    public Count tCount = new Count(4, 8);
    public Count crossCount = new Count(4, 8);
    public GameObject tile, player, insertArrow, diamond;
    public TurnManager turnManager;
    public Camera camera;
    

    // pubic to make it accessible from the turnmanager
    public GameObject[] allPlayers;
    public GameObject[,] myMap;
    public Tile[,] myMapTiles;

    public bool detachSpawnPoints = true, detachCentralTile = true;

    private List<Vector3> gridPositions = new List<Vector3>();
    private Vector3 finalShift;
    private GameObject[] allInsertArrows;
    private SpriteRenderer[] allInsertArrowsRenderer;
    public GameObject myDiamondInstance;
    public Coordinate diamondCoords;

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross,
        Curve_BR_alt, Curve_LB_alt, Curve_RT_alt, Curve_TL_alt, T_B_alt, T_L_alt, T_T_alt, T_R_alt, Goal
    };

    enum slideDirection
    {
        leftToRight, botToTop, rightToLeft, topToBot
    };

    int[] nbrOfTiles;
    int[] noBottom = { 2, 3, 5, 8 };
    int[] noRight = { 1, 3, 4, 7 };
    int[] noTop = { 0, 1, 5, 6 };
    int[] noLeft = { 0, 2, 4, 9 };


    // Map Generation At game start

    public void CreatePlayers()
    {
        for (int i = 1; i < 5; i++)
        {
            InstantiatePlayer(i);
        }
    }

    public void CreateInsertArrows()
    {
        allInsertArrows = new GameObject[2*columns + 2*rows];
        allInsertArrowsRenderer = new SpriteRenderer[2 * columns + 2 * rows];
        int indx = 0;
        float arrowZOrder = -2f;
        GameObject arrowInstance;

        for (int i = 0; i < columns; i++) // bot arrows
        {
            arrowInstance = Instantiate(insertArrow, new Vector3((i * tileSize), -tileSize, arrowZOrder), Quaternion.identity);
            arrowInstance.transform.Rotate(Vector3.forward * 90);
            arrowInstance.GetComponent<InsertArrow>().setPointedCoords(i, i, 0, columns-1);
            arrowInstance.transform.SetParent(transform);
            allInsertArrows[i] = arrowInstance;
            allInsertArrowsRenderer[i] = arrowInstance.GetComponent<SpriteRenderer>();
            allInsertArrowsRenderer[i].color = Color.clear;
            indx++;
        }
        for (int i = 0; i < rows; i++) // right arrows
        {
            arrowInstance = Instantiate(insertArrow, new Vector3(columns * tileSize, (i * tileSize), arrowZOrder), Quaternion.identity);
            arrowInstance.transform.Rotate(Vector3.forward * 180);
            arrowInstance.GetComponent<InsertArrow>().setPointedCoords(columns-1, 0, i, i);
            arrowInstance.transform.SetParent(transform);
            allInsertArrows[i+columns] = arrowInstance;
            allInsertArrowsRenderer[i + columns] = arrowInstance.GetComponent<SpriteRenderer>();
            allInsertArrowsRenderer[i + columns].color = Color.clear;
            indx++;
        }
        for (int i = 0; i < columns; i++) // top arrows
        {
            arrowInstance = Instantiate(insertArrow, new Vector3((columns-1) * tileSize - ((i * tileSize)), rows * tileSize , arrowZOrder), Quaternion.identity);
            arrowInstance.transform.Rotate(Vector3.forward * -90);
            arrowInstance.GetComponent<InsertArrow>().setPointedCoords(columns - 1 - i, columns - 1 - i, rows-1, 0);
            arrowInstance.transform.SetParent(transform);
            allInsertArrows[i+(columns + rows)] = arrowInstance;
            allInsertArrowsRenderer[i + (columns + rows)] = arrowInstance.GetComponent<SpriteRenderer>();
            allInsertArrowsRenderer[i + (columns + rows)].color = Color.clear;
            indx++;
        }
        for (int i = 0; i < rows; i++) // left arrows
        {
            arrowInstance = Instantiate(insertArrow, new Vector3(-tileSize, (rows-1) * tileSize - ((i * tileSize) ), arrowZOrder), Quaternion.identity);
            arrowInstance.GetComponent<InsertArrow>().setPointedCoords(0, columns-1,rows-1- i, rows-1-i);
            arrowInstance.transform.SetParent(transform);
            allInsertArrows[i+ (2*columns + rows)] = arrowInstance;
            allInsertArrowsRenderer[i + (2 * columns + rows)] = arrowInstance.GetComponent<SpriteRenderer>();
            allInsertArrowsRenderer[i + (2 * columns + rows)].color = Color.clear;
            indx++;
        }

    }

    public void SetInsertArrowsVisible(bool status)
    {
        if (status)
            foreach (var arrowRender in allInsertArrowsRenderer)
            {
                arrowRender.color = Color.white;
            }
        else
            foreach (var arrowRender in allInsertArrowsRenderer)
            {
                arrowRender.color = Color.clear;
            }
    }

    public void SetInsertArrowVisible(bool status, int idx)
    {
        if (status)
            allInsertArrowsRenderer[idx].color = Color.white;
        else
            allInsertArrowsRenderer[idx].color = Color.clear;

    }

    int[] GenerateInitialTiles()
    {
        // Array storing all the information about the number of tiles per type
        int[] nbrOfTiles;

        nbrOfTiles = new int[4] { 0, 0, 0, 0 }; // 1st index is curves number, 2nd is straights, 3rd is ts, 4th is crosses

        //Defines the number of tiles per type
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                nbrOfTiles[i] = Random.Range(curvesCount.minimum, curvesCount.maximum + 1); // nbr of curve tiles
            else if (i == 1)
                nbrOfTiles[i] = Random.Range(straightCount.minimum, straightCount.maximum + 1); // nbr of straight tiles
            else if (i == 2)
                nbrOfTiles[i] = Random.Range(tCount.minimum, tCount.maximum + 1); // nbr of T tiles
            else if (i == 3)
                nbrOfTiles[i] = Random.Range(crossCount.minimum, crossCount.maximum + 1); // nbr of cross tiles
        }

        return nbrOfTiles;
    }

    void GenerateSpawningAreas()
    {
        var toInstantiate = new int[4];
        toInstantiate[0] = (int)tileTypes.Curve_RT;  // bot-left
        toInstantiate[1] = (int)tileTypes.Curve_TL;  // bot-right
        toInstantiate[2] = (int)tileTypes.Curve_BR;  // top-left
        toInstantiate[3] = (int)tileTypes.Curve_LB;  // top-right

        int myTile;

        for (int i = 0; i < toInstantiate.Length; i++)
        {
            InstantiateStartingPos(i + 1);

            if (i == 0)
            {
                // InstantiateTile(toInstantiate[i], new Coordinate(0, 0), false);

                while (true)
                {
                    myTile = noBottom[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(0, 1));
                        break;
                    }

                }

                while (true)
                {
                    myTile = noLeft[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(1, 0));
                        break;                     
                    }

                }

            }
            else if (i == 1)
            {
                //InstantiateTile(toInstantiate[i], new Coordinate(columns - 1, 0), false);

                while (true)
                {
                    myTile = noBottom[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(columns - 1, 1));
                        break;
                    }

                }

                while (true)
                {
                    myTile = noRight[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(columns - 2, 0));
                        break;
                    }

                }
            }
            else if (i == 2)
            {
                //InstantiateTile(toInstantiate[i], new Coordinate(0, rows - 1), false);

                while (true)
                {
                    myTile = noLeft[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(1, rows - 1));
                        break;
                    }

                }

                while (true)
                {
                    myTile = noTop[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(0, rows - 2));
                        break;
                    }

                }

            }
            else
            {
                //InstantiateTile(toInstantiate[i], new Coordinate(columns - 1, rows - 1), false);

                while (true)
                {
                    myTile = noRight[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(columns - 2, rows - 1));
                        break;
                    }

                }

                while (true)
                {
                    myTile = noTop[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(myTile, new Coordinate(columns - 1, rows - 2));
                        break;
                    }

                }

            }

        }
    }

    void GenerateCentralArea()
    {
        int centralX = columns / 2, centralY = rows / 2;
        Coordinate myCoordinate;

        for (int i = centralX - 1; i < centralX +2; i++)
        {
            for (int j = centralY - 1; j < centralY + 2; j++)
            {
                myCoordinate = new Coordinate(i, j);
                if (myCoordinate.isEqual(centralX - 1, centralY - 1))
                {
                    InstantiateTile((int)tileTypes.Curve_RT, myCoordinate, true);
                }
                else if (myCoordinate.isEqual(centralX - 1, centralY))
                {
                    InstantiateTile((int)tileTypes.T_R, myCoordinate, true);
                }
                else if (myCoordinate.isEqual(centralX - 1, centralY+1))
                {
                    InstantiateTile((int)tileTypes.Curve_BR, myCoordinate, true);
                }
                else if (myCoordinate.isEqual(centralX, centralY - 1))
                {
                    InstantiateTile((int)tileTypes.T_T, myCoordinate, true);
                }
                else if (myCoordinate.isEqual(centralX, centralY))
                {
                    InstantiateTile((int)tileTypes.Goal, myCoordinate, false);
                    InstantiateDiamond(myCoordinate);
                }
                else if (myCoordinate.isEqual(centralX, centralY+1))
                {
                    InstantiateTile((int)tileTypes.T_B, myCoordinate, true);   
                }
                else if (myCoordinate.isEqual(centralX+1, centralY - 1))
                {
                    InstantiateTile((int)tileTypes.Curve_TL, myCoordinate, true);
                }
                else if (myCoordinate.isEqual(centralX + 1, centralY))
                {
                    InstantiateTile((int)tileTypes.T_L, myCoordinate, true);
                }
                else if (myCoordinate.isEqual(centralX + 1, centralY+1))
                {
                    InstantiateTile((int)tileTypes.Curve_LB, myCoordinate, true);
                }
            }
        }    

    }

    private void PlaceNeutralTraps()
    {
        int distanceFromBorder = 2;

        var topLeft = new Coordinate(Random.Range(distanceFromBorder, columns / 2 - 2), Random.Range(rows / 2 + 2, rows - distanceFromBorder));
        var topRight = new Coordinate(Random.Range(columns / 2 + 2, columns - distanceFromBorder), Random.Range(rows / 2 + 2, rows - distanceFromBorder));
        var botLeft = new Coordinate(Random.Range(distanceFromBorder, columns / 2 - 2), Random.Range(distanceFromBorder, rows / 2 - 2));
        var botRight = new Coordinate(Random.Range(columns / 2 + 2, columns - distanceFromBorder), Random.Range(distanceFromBorder, rows / 2 - 2));

        var myCoords = new Coordinate[4] { topLeft, topRight, botLeft, botRight};

        foreach (var coord in myCoords)
        {
            Tile tile = PickTileComponent(coord);
            tile.SetTrap(0);
        }

    }

    public void MapSetup()
    {
        int finalTilesNbr = columns * rows, randomTilesNbr = finalTilesNbr - 21, generatedTilesNbr = 0;
        int[] tilesArray;
        finalShift = new Vector3(-(columns - 1) * tileSize / 2.0f, -(rows - 1) * tileSize / 2.0f, 0f);

        nbrOfTiles = GenerateInitialTiles();

        // Check of the number of tiles generated
        for (int i =0; i < nbrOfTiles.Length; i++)
            generatedTilesNbr += nbrOfTiles[i];

        // Adds or Removes the tiles in order to match the wanted amount
        if (generatedTilesNbr != randomTilesNbr)
        {
            if (generatedTilesNbr > randomTilesNbr)
            {
                while(generatedTilesNbr != randomTilesNbr)
                {
                    int indx = Random.Range(0, 4); // defines the tile type to me removed

                    if (indx == 0 && nbrOfTiles[indx] > curvesCount.minimum) // removes one curve
                    {
                        nbrOfTiles[indx]--;
                        generatedTilesNbr--;
                    }
                    else if (indx == 1 && nbrOfTiles[indx] > straightCount.minimum) // removes one straight
                    {
                        nbrOfTiles[indx]--;
                        generatedTilesNbr--;
                    }
                    else if (indx == 2 && nbrOfTiles[indx] > tCount.minimum) // removes one T
                    {
                        nbrOfTiles[indx]--;
                        generatedTilesNbr--;
                    }
                    else if (indx == 3 && nbrOfTiles[indx] > crossCount.minimum) // removes one cross
                    {
                        nbrOfTiles[indx]--;
                        generatedTilesNbr--;
                    }
                }
       
            }
            else
            {
                while (generatedTilesNbr != randomTilesNbr)
                {
                    int indx = Random.Range(0, 4);

                    if (indx == 0 && nbrOfTiles[indx] < curvesCount.maximum)
                    {
                        nbrOfTiles[indx]++;
                        generatedTilesNbr++;
                    }
                    else if (indx == 1 && nbrOfTiles[indx] < straightCount.maximum)
                    {
                        nbrOfTiles[indx]++;
                        generatedTilesNbr++;
                    }
                    else if (indx == 2 && nbrOfTiles[indx] < tCount.maximum)
                    {
                        nbrOfTiles[indx]++;
                        generatedTilesNbr++;
                    }
                    else if (indx == 3 && nbrOfTiles[indx] < crossCount.maximum)
                    {
                        nbrOfTiles[indx]++;
                        generatedTilesNbr++;
                    }
                }
            }
        }

        int tmp = 0, tmpIdx = 0;

        // Creates the tiles inside the array containing them all by index
        tilesArray = new int[randomTilesNbr];
        for (int i = 0; i < nbrOfTiles.Length; i++)
        {
            for (int j = 0; j < nbrOfTiles[i]; j++)
            {
                if (i == 0)
                    tmp = Random.Range(0, 4);
                else if (i == 1)
                    tmp = Random.Range(4, 6);
                else if (i == 2)
                    tmp = Random.Range(6, 10);
                else if (i == 3)
                    tmp = 10;

                tilesArray[tmpIdx] = tmp;
                tmpIdx++;
            } 
        }

        // Scramble the tiles inside the array
        tilesArray = GeneralMethods.ReshuffleArray(tilesArray);

        // Places the tiles
        tmpIdx = 0;

        // Generates the 4 corners of the map
        GenerateSpawningAreas();

        // Generates the central area
        GenerateCentralArea();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject toInstantiate;
                int randomTmp = Random.Range(0, 4);

                bool isSpecial;

                isSpecial = (i <= 1 && j == 0) || // bot-left
                            (i == 0 && j <= 1) ||
                            (i <= 1 && j == rows-1) || // top-left
                            (i == 0 && j >= rows-2) ||
                            (i >= columns - 2 && j == rows - 1) || // top-right
                            (i == columns -1 && j >= rows - 2) ||
                            (i >= columns - 2 && j == 0) || // bot-right
                            (i == columns - 1 && j <= 1) ||
 
                            (i >= columns / 2 - 1 && i <= columns / 2 + 1 && j >= rows / 2 -1 && j <= rows / 2 + 1); // central core 

                if (!isSpecial)
                {
                    InstantiateTile(tilesArray[tmpIdx], new Coordinate(i, j));
                    tmpIdx++;
                }
                
            }
        }

        CreatePlayers();

        //updateTilesConnection();
        CreateInsertArrows();
        PlaceNeutralTraps();

        transform.position = finalShift;

        //updateTilesConnection();

    }

    //Objects Instance

    void InstantiateStartingPos(int playerNbr)
    {
        Coordinate coordinate = null;
        Tile myTileComponent = null;
        Animator myAnimator = null;
        GameObject tileInstance = null;

        if (playerNbr == 1)
        {
            coordinate = new Coordinate(0, rows-1);
            tileInstance = Instantiate(tile, coordinate.getVect3WithZ(), Quaternion.identity);
            tileInstance.transform.SetParent(transform);

            myTileComponent = tileInstance.GetComponent<Tile>();
            myTileComponent.myTexture = (Texture2D)Resources.Load("Tiles/tile_spawn_1P");
            myTileComponent.SetPossibleConnections(0);

            myAnimator = tileInstance.GetComponent<Animator>();
            myAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("TilesAnimators/tile_spawn_1P");

        }
        else if (playerNbr == 2)
        {
            coordinate = new Coordinate(columns-1, rows - 1);
            tileInstance = Instantiate(tile, coordinate.getVect3WithZ(), Quaternion.identity);
            tileInstance.transform.SetParent(transform);

            myTileComponent = tileInstance.GetComponent<Tile>();

            myTileComponent.myTexture = (Texture2D)Resources.Load("Tiles/tile_spawn_2P");
            myTileComponent.SetPossibleConnections(1);

            myAnimator = tileInstance.GetComponent<Animator>();
            myAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("TilesAnimators/tile_spawn_2P");
        }
        else if (playerNbr == 3)
        {
            coordinate = new Coordinate(0, 0);
            tileInstance = Instantiate(tile, coordinate.getVect3WithZ(), Quaternion.identity);
            tileInstance.transform.SetParent(transform);

            myTileComponent = tileInstance.GetComponent<Tile>();

            myTileComponent.myTexture = (Texture2D)Resources.Load("Tiles/tile_spawn_3P");
            myTileComponent.SetPossibleConnections(2);

            myAnimator = tileInstance.GetComponent<Animator>();
            myAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("TilesAnimators/tile_spawn_3P");
        }
        else
        {
            coordinate = new Coordinate(columns-1, 0);
            tileInstance = Instantiate(tile, coordinate.getVect3WithZ(), Quaternion.identity);
            tileInstance.transform.SetParent(transform);

            myTileComponent = tileInstance.GetComponent<Tile>();

            myTileComponent.myTexture = (Texture2D)Resources.Load("Tiles/tile_spawn_4P");
            myTileComponent.SetPossibleConnections(3);

            myAnimator = tileInstance.GetComponent<Animator>();
            myAnimator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("TilesAnimators/tile_spawn_4P");
        }

        myTileComponent.mySprite = Sprite.Create(myTileComponent.myTexture,
            new Rect(0, 0, myTileComponent.myTexture.width, myTileComponent.myTexture.height),
            new Vector2(0.5f, 0.66f));
        myTileComponent.myRenderer.sprite = myTileComponent.mySprite;
        myTileComponent.canBeMoved = false;
        myTileComponent.myCoord = coordinate;
        myTileComponent.SetPlayerChild();
        myMap[coordinate.getX(), coordinate.getY()] = tileInstance;
        myMapTiles[coordinate.getX(), coordinate.getY()] = tileInstance.GetComponent<Tile>();
    }

    void InstantiateTile(int tileType, Coordinate coordinate, bool canBeMoved = true)
    {
        GameObject tileInstance = Instantiate(tile, coordinate.getVect3WithZ(), Quaternion.identity);
        tileInstance.transform.SetParent(transform);

        Tile myTileComponent = tileInstance.GetComponent<Tile>();
        myTileComponent.SetSprite(tileType);
        // TODO modificare type per diventare come quello non speciale
        myTileComponent.canBeMoved = canBeMoved;
        myTileComponent.myCoord = coordinate;
        myTileComponent.SetPossibleConnections(tileType);
        myTileComponent.SetPlayerChild();
        myTileComponent.SetTurnManager();

        myMap[coordinate.getX(), coordinate.getY()] = tileInstance;
        myMapTiles[coordinate.getX(), coordinate.getY()] = tileInstance.GetComponent<Tile>();

    }

    public Tile InstantiateTileLive(int tileType, Coordinate coordinate, bool isTrapped = false, bool canBeMoved = true)
    {
        GameObject tileInstance = Instantiate(tile, coordinate.getVect3() + transform.position, Quaternion.identity);
        tileInstance.transform.SetParent(transform);

        Tile myTileComponent = tileInstance.GetComponent<Tile>();
        myTileComponent.SetTurnManager();

        myTileComponent.SetSprite(tileType);
        // TODO modificare type per diventare come quello non speciale
        myTileComponent.canBeMoved = canBeMoved;
        myTileComponent.myCoord = coordinate;
        myTileComponent.SetPossibleConnections(tileType);

        if (isTrapped)
        {
            turnManager.insertedTrapFromSlide = true;
            myTileComponent.SetTrap(turnManager.GetActivePlayer());
            turnManager.AddToActivateTrapList(myTileComponent.GetTrap());
        }

        myMap[coordinate.getX(), coordinate.getY()] = tileInstance;
        myMapTiles[coordinate.getX(), coordinate.getY()] = tileInstance.GetComponent<Tile>();

        return myTileComponent;
    }

    void InstantiateDiamond(Coordinate coordinate)
    {
        Vector3 diamondPosition = coordinate.getVect3();
        diamondPosition.z = -10;
        myDiamondInstance = Instantiate(diamond, diamondPosition, Quaternion.identity);
        myDiamondInstance.transform.SetParent(myMap[coordinate.getX(), coordinate.getY()].transform);

        diamondCoords = new Coordinate(coordinate.getX(), coordinate.getY());

        Texture2D myTexture = (Texture2D)Resources.Load("TileProva/diamond");
        Sprite mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.66f));
        myDiamondInstance.GetComponent<SpriteRenderer>().sprite = mySprite;

    }

    void InstantiatePlayer(int playerNbr)
    {
        GameObject playerInstance;

        if (playerNbr == 1)
        {
            playerInstance = Instantiate(player, new Vector3(0f, rows - 1, -1f) * tileSize, Quaternion.identity);
            playerInstance.GetComponent<Player>().coordinate = new Coordinate (0, rows - 1);
            playerInstance.transform.SetParent(myMap[0, rows-1].transform);
            playerInstance.GetComponent<Player>().playerNbr = 1;
            myMapTiles[0, rows - 1].SetPlayerChild(playerInstance.GetComponent<Player>());

        }
        else if (playerNbr == 2)
        {
            playerInstance = Instantiate(player, new Vector3(columns - 1, rows - 1, -1f) * tileSize, Quaternion.identity);
            playerInstance.GetComponent<Player>().coordinate = new Coordinate(columns - 1, rows - 1);
            playerInstance.transform.SetParent(myMap[columns - 1, rows - 1].transform);
            playerInstance.GetComponent<Player>().playerNbr = 2;
            myMapTiles[columns - 1, rows - 1].SetPlayerChild(playerInstance.GetComponent<Player>());
            playerInstance.GetComponent<Player>().InvertTransformX();
        }
        else if (playerNbr == 3)
        {
            playerInstance = Instantiate(player, new Vector3(0f, 0f, -1f) * tileSize, Quaternion.identity);
            playerInstance.GetComponent<Player>().coordinate = new Coordinate(0, 0);
            playerInstance.transform.SetParent(myMap[0, 0].transform);
            playerInstance.GetComponent<Player>().playerNbr = 3;
            myMapTiles[0, 0].SetPlayerChild(playerInstance.GetComponent<Player>());
        }
        else
        {
            playerInstance = Instantiate(player, new Vector3(columns - 1, 0f, -1f) * tileSize, Quaternion.identity);
            playerInstance.GetComponent<Player>().coordinate = new Coordinate(columns - 1, 0);
            playerInstance.transform.SetParent(myMap[columns - 1, 0].transform);
            playerInstance.GetComponent<Player>().playerNbr = 4;
            myMapTiles[columns - 1, 0].SetPlayerChild(playerInstance.GetComponent<Player>());
            playerInstance.GetComponent<Player>().InvertTransformX();
        }

        allPlayers[playerNbr-1] = playerInstance;
        var myPlayer = playerInstance.GetComponent<Player>();
        myPlayer.playerNbr = playerNbr;
        myPlayer.setPlayerSprite();
        myPlayer.SetStartingPoint();

    }

    //Terraforming

    Coordinate slideCoordinate(Coordinate myCoord, int slideDir)
    {
        var newCoord = new Coordinate(myCoord.getX(), myCoord.getY());

        switch (slideDir)
        {
            case (int)slideDirection.leftToRight:
                newCoord.setCoordinate(newCoord.getX() + 1, newCoord.getY());
                break;
            case (int)slideDirection.botToTop:
                newCoord.setCoordinate(newCoord.getX(), newCoord.getY() + 1);
                break;
            case (int)slideDirection.rightToLeft:
                newCoord.setCoordinate(newCoord.getX() - 1, newCoord.getY());
                break;
            case (int)slideDirection.topToBot:
                newCoord.setCoordinate(newCoord.getX(), newCoord.getY() - 1);
                break;
            default:
                break;
        }

        return newCoord;
    }

    void DestroyTile(GameObject myTile) // checks whether or not a player is present on the tile
    {
        // Should be obsolete, the turn manager is taking care of this part now
        //for (int i = 0; i < myTile.transform.childCount; i++)
        //{
        //    Transform childTrans = myTile.transform.GetChild(i);
        //    if (childTrans.gameObject.tag == "Player")
        //    {
        //        childTrans.parent = null;
        //        childTrans.gameObject.GetComponent<Player>().TeleportOffScreen();
        //    }
        //}

        Destroy(myTile);
    }

    public IEnumerator SlideLine(Coordinate[] myCoords, int mySlideDirection)
    {
        Tile tmpTile;
        GameObject tmpTileObj;

        float animationTime = 0.5f;

        DestroyTile(PickTileObject(myCoords[myCoords.Length - 1])); // destroys the last movable tile, could become a coroutine

        var myMovement = new Vector2(0, 0);
        camera.DOShakePosition(0.9f, 1);

        for (int i = myCoords.Length - 2; i >= 0; i--)
        {
            tmpTile = PickTileComponent(myCoords[i]);
            tmpTileObj = PickTileObject(myCoords[i]);
            myMovement[0] = PickTileObject(myCoords[i+1]).transform.position.x - tmpTileObj.transform.position.x;
            myMovement[1] = PickTileObject(myCoords[i + 1]).transform.position.y - tmpTileObj.transform.position.y;
            StartCoroutine(tmpTile.MoveToPosition(myMovement, animationTime));
            tmpTile.setCoordinates(myCoords[i + 1].getX(), myCoords[i + 1].getY());

            myMap[myCoords[i + 1].getX(), myCoords[i + 1].getY()] = tmpTileObj;
            myMapTiles[myCoords[i + 1].getX(), myCoords[i + 1].getY()] = tmpTile;

        }

        yield return new WaitForSeconds(animationTime);

        //float waitingTime = 0;
        //while (waitingTime < animationTime + 0.5f)
        //{
        //    waitingTime += Time.deltaTime;
        //    if (waitingTime > animationTime)
        //        turnManager.isSliding = false;
        //    yield return null;
        //}

        //yield return null;
    }

    public IEnumerator RotateTiles(Coordinate[] selectedCoords, int rotationDirection) // 1 clockwise, -1 counterclockwise
    {
        selectedCoords = KeepMovableTiles(selectedCoords);

        if (rotationDirection == -1)
        {
            selectedCoords = GeneralMethods.ReverseArray(selectedCoords);
        }

        Tile tmpTile;
        var tmpTileMatrix = new Tile[selectedCoords.Length];
        GameObject tmpTileObj;
        var tmpTileObjMatrix = new GameObject[selectedCoords.Length];

        float animationTime = 0.5f;
        var myMovement = new Vector2(0, 0);

        for (int i = 0; i < selectedCoords.Length; i++)
        {
            tmpTile = PickTileComponent(selectedCoords[i]);
            tmpTileObj = PickTileObject(selectedCoords[i]);

            myMovement[0] = PickTileObject(selectedCoords[(i + 1) % (selectedCoords.Length)]).transform.position.x - tmpTileObj.transform.position.x;
            myMovement[1] = PickTileObject(selectedCoords[(i + 1) % (selectedCoords.Length)]).transform.position.y - tmpTileObj.transform.position.y;

            StartCoroutine(tmpTile.MoveToPosition(myMovement, animationTime));
            tmpTile.setCoordinates(selectedCoords[(i + 1) % (selectedCoords.Length)].getX(), selectedCoords[(i + 1) % (selectedCoords.Length)].getY());

            tmpTileMatrix[i] = tmpTile;
            tmpTileObjMatrix[i] = tmpTileObj;

        }

        for (int i = 0; i < selectedCoords.Length; i++)
        {
            myMap[selectedCoords[(i + 1) % (selectedCoords.Length)].getX(), selectedCoords[(i + 1) % (selectedCoords.Length)].getY()] = tmpTileObjMatrix[i];
            myMapTiles[selectedCoords[(i + 1) % (selectedCoords.Length)].getX(), selectedCoords[(i + 1) % (selectedCoords.Length)].getY()] = tmpTileMatrix[i];
        }

        //float waitingTime = 0;

        //while (waitingTime < animationTime + 0.5f)
        //{
        //    waitingTime += Time.deltaTime;
        //    if (waitingTime > animationTime)
        //        turnManager.isRotating = false;
        //    yield return null;
        //}

        yield return new WaitForSeconds(animationTime);

    }

    //Tiles managing utilities

    public GameObject PickTileObject(Coordinate myCoord)
    {
        return myMap[myCoord.getX(), myCoord.getY()];
    }

    bool checkTileRange(int tileIdx)  // checks wether or not the tile respects the given boundaries
    {
        
        if (0 <= tileIdx && tileIdx < 4)
        {
            if (nbrOfTiles[0] < curvesCount.maximum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        else if (4 <= tileIdx && tileIdx < 6)
        {
            if (nbrOfTiles[1] < straightCount.maximum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        else
        {
            if (nbrOfTiles[2] < tCount.maximum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void ResetEffectiveConnections()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                myMapTiles[i, j].resetEffectiveConnectionMap();
            }
        }
    }

    public void ResetEffectiveConnections(Coordinate topLeft, Coordinate botRight) // works only in a squared area defined by the 2 coordinates included
    {
        for (int i = topLeft.getX(); i <= botRight.getX(); i++)
        {
            for (int j = botRight.getY(); j <= topLeft.getY(); j++)
            {
                myMapTiles[i, j].resetEffectiveConnectionMap();
            }
        }
    }

    public void updateTilesConnection(int playerPlayingNbr)
    {
        ResetEffectiveConnections();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Tile questa = myMap[i, j].GetComponent<Tile>();

                if (i - 1 >= 0)
                {
                    questa.CheckConnections(myMap[i - 1, j].GetComponent<Tile>(), 3, playerPlayingNbr);
                }
                
                if (j - 1 >= 0)
                {
                    questa.CheckConnections(myMap[i, j - 1].GetComponent<Tile>(), 2, playerPlayingNbr);
                }
                
                if (j + 1 < rows)
                {
                    questa.CheckConnections(myMap[i, j + 1].GetComponent<Tile>(), 0, playerPlayingNbr);
                }
                
                if (i + 1 < columns)
                {
                    questa.CheckConnections(myMap[i + 1, j].GetComponent<Tile>(), 1, playerPlayingNbr);
                }
                
            }
        }

    }

    public void updateTilesConnection(Coordinate topLeft, Coordinate botRight)
    {
        ResetEffectiveConnections(topLeft, botRight);

        for (int i = topLeft.getX(); i <= botRight.getX(); i++)
        {
            for (int j = botRight.getY(); j <= topLeft.getY(); j++)
            {
                Tile questa = myMap[i, j].GetComponent<Tile>();

                if (i - 1 > 0)
                {
                    questa.CheckConnections(myMap[i - 1, j].GetComponent<Tile>(), 3);
                }

                if (j - 1 > 0)
                {
                    questa.CheckConnections(myMap[i, j - 1].GetComponent<Tile>(), 2);
                }

                if (j + 1 < rows)
                {
                    questa.CheckConnections(myMap[i, j + 1].GetComponent<Tile>(), 0);
                }

                if (i + 1 < columns)
                {
                    questa.CheckConnections(myMap[i + 1, j].GetComponent<Tile>(), 1);
                }

            }
        }
    }

    public Tile PickTileComponent(Coordinate myCoord)
    {
        return myMapTiles[myCoord.getX(), myCoord.getY()];
    }

    public Coordinate[] KeepMovableTiles(Coordinate[] myCoords)
    {
        List<Coordinate> movableCoordsList = new List<Coordinate>();
        for (int i = 0; i < myCoords.Length; i++)
        {
            if (PickTileComponent(myCoords[i]).canBeMoved)
                movableCoordsList.Add(myCoords[i]);
        }

        return movableCoordsList.ToArray();
    }

    public void UpdateTilesZOrder()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                myMapTiles[i, j].UpdateZOrder();
            }
        }
    }

    // Others

    public Coordinate GetDiamondCoords()
    {
        return diamondCoords;
    }

    public GameObject[] getAllInstancedArrows()
    {
        return allInsertArrows;
    }

    // Unity Specific methods

    void Awake ()
    {
        myMap = new GameObject[columns, rows];
        myMapTiles = new Tile[columns, rows];
        allPlayers = new GameObject[4];
    }
	
	
	void Update ()
    {
        
    }
}

[Serializable]
public class Coordinate
{
    public int x, y;

    float tileSize = 10.0f;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

    public void setCoordinate (int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool isEqual(int x, int y)
    {
        if (this.x == x && this.y == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isEqual(Coordinate other)
    {
        if ((other.getX() == x) && (other.getY() == y))
            return true;
        else
            return false;           
    }

    public Vector3 getVect3()
    {
        return new Vector3((float)x, (float)y, 0f) * tileSize;
    }

    public Vector3 getVect3WithZ()
    {
        return new Vector3((float)x, (float)y, 0.0001f * (float)y) * tileSize;
    }

    public Vector3 GetPositionFromCoords(int columns, int rows)
    {
        var relativePosition = getVect3();
        float xShift = -(columns-1) * tileSize / 2f;
        float yShift = -(rows-1) * tileSize / 2f;

        var position = new Vector3(relativePosition.x + xShift, relativePosition.y + yShift, 0f);
        return position;
    }

    public void GetCoordsFromPosition(Vector3 position, int columns, int rows)
    {
        float[] x_bin = GeneralMethods.CreateBins(tileSize, -columns * tileSize / 2f, columns + 1);
        float[] y_bin = GeneralMethods.CreateBins(tileSize, -rows * tileSize / 2f, rows + 1);

        x = GeneralMethods.FindValInBins(x_bin, position.x);
        y = GeneralMethods.FindValInBins(y_bin, position.y);
    }
}