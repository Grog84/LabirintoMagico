using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public Count curvesCount = new Count(4, 8);
    public Count straightCount = new Count(4, 8);
    public Count tCount = new Count(4, 8);
    public Count crossCount = new Count(4, 8);
    public GameObject goalPoint;
    public GameObject[] startingPoint;
    public GameObject[] tiles;
    public bool detachSpawnPoints = true, detachCentralTile = true;

    private List<Vector3> gridPositions = new List<Vector3>();
    private float tileSize;

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straigh_V, Straight_H, T_B, T_L, T_T, T_R, Cross
    };

    int[] nbrOfTiles;
    int[] noBottom = { 2, 3, 5, 8 };
    int[] noRight = { 1, 3, 4, 7 };
    int[] noTop = { 0, 1, 5, 6 };
    int[] noLeft = { 0, 2, 4, 9 };

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

    void InitializeGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    int[] ReshuffleArray(int[] nbrOfTiles)
    {
        int[] reshuffledArray;
        var matrix = new SortedList();
        reshuffledArray = new int[nbrOfTiles.Length];

        var r = new System.Random();

        for (int i = 0; i < nbrOfTiles.Length; i++)
        {
            var idx = r.Next();
            // grants no duplicates idx
            while (matrix.ContainsKey(idx)) { idx = r.Next(); }

            matrix.Add(idx, nbrOfTiles[i]);
        }

        matrix.Values.CopyTo(reshuffledArray, 0);

        return reshuffledArray;

    }

    void InstantiateTile(GameObject tile, Vector3 coordinates, bool isAlternative = false)
    {
        GameObject instance = Instantiate(tile, coordinates, Quaternion.identity);
        instance.transform.SetParent(this.transform);
        if (isAlternative)
        {
            SpriteRenderer myRenderer = instance.GetComponent<SpriteRenderer>();
            Texture2D myTexture = (Texture2D)Resources.Load("TileProva/" + myRenderer.sprite.name + "_alt");
            Sprite mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
            myRenderer.sprite = mySprite;
        }

    }

    void GenerateSpawningAreas()
    {
        var toInstantiate = new GameObject[4];
        toInstantiate[0] = startingPoint[2];  // bot-left
        toInstantiate[1] = startingPoint[3];  // bot-right
        toInstantiate[2] = startingPoint[0];  // top-left
        toInstantiate[3] = startingPoint[1];  // top-right

        int myTile;

        GameObject instance;

        for (int i = 0; i < toInstantiate.Length; i++)
        {
            if (i == 0)
            {
                instance = Instantiate(toInstantiate[i], new Vector3(0, 0, 0f) * tileSize, Quaternion.identity);
                instance.transform.SetParent(this.transform);

                while (true)
                {
                    myTile = noBottom[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(0, 1, 0f) * tileSize);
                        break;
                    }

                }

                while (true)
                {
                    myTile = noLeft[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(1, 0, 0f) * tileSize);
                        break;                     
                    }

                }

            }
            else if (i == 1)
            {
                instance = Instantiate(toInstantiate[i], new Vector3(columns - 1, 0, 0f) * tileSize, Quaternion.identity);
                instance.transform.SetParent(this.transform);

                while (true)
                {
                    myTile = noBottom[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(columns - 1, 1, 0f) * tileSize);
                        break;
                    }

                }

                while (true)
                {
                    myTile = noRight[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(columns - 2, 0, 0f) * tileSize);
                        break;
                    }

                }
            }
            else if (i == 2)
            {
                instance = Instantiate(toInstantiate[i], new Vector3(0, rows - 1, 0f) * tileSize, Quaternion.identity);
                instance.transform.SetParent(this.transform);

                while (true)
                {
                    myTile = noLeft[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(1, rows - 1, 0f) * tileSize);
                        break;
                    }

                }

                while (true)
                {
                    myTile = noTop[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(0, rows - 2, 0f) * tileSize);
                        break;
                    }

                }

            }
            else
            {
                instance = Instantiate(toInstantiate[i], new Vector3(columns - 1, rows - 1, 0f) * tileSize, Quaternion.identity);
                instance.transform.SetParent(this.transform);

                while (true)
                {
                    myTile = noRight[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(columns - 2, rows - 1, 0f) * tileSize);
                        break;
                    }

                }

                while (true)
                {
                    myTile = noTop[Random.Range(0, 4)];
                    bool isInRange = checkTileRange(myTile);
                    if (isInRange)
                    {
                        InstantiateTile(tiles[myTile], new Vector3(columns - 1, rows - 2, 0f) * tileSize);
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

        int idx = 0;
        for (int i = centralX - 1; i < centralX +2; i++)
        {
            for (int j = centralY - 1; j < centralY + 2; j++)
            {
                myCoordinate = new Coordinate(i, j);
                if (myCoordinate.isEqual(centralX - 1, centralY - 1))
                {
                    InstantiateTile(tiles[2], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX - 1, centralY))
                {
                    InstantiateTile(tiles[9], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX - 1, centralY+1))
                {
                    InstantiateTile(tiles[0], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX, centralY - 1))
                {
                    InstantiateTile(tiles[8], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX, centralY))
                {
                    InstantiateTile(goalPoint, new Vector3(i, j, 0f) * tileSize);
                }
                else if (myCoordinate.isEqual(centralX, centralY+1))
                {
                    InstantiateTile(tiles[6], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX+1, centralY - 1))
                {
                    InstantiateTile(tiles[3], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX + 1, centralY))
                {
                    InstantiateTile(tiles[7], new Vector3(i, j, 0f) * tileSize, true);
                }
                else if (myCoordinate.isEqual(centralX + 1, centralY+1))
                {
                    InstantiateTile(tiles[1], new Vector3(i, j, 0f) * tileSize, true);
                }
            }
        }    

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

    void MapSetup()
    {
        int finalTilesNbr = columns * rows, randomTilesNbr = finalTilesNbr - 21, generatedTilesNbr = 0;
        int[] tilesArray;

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
        tilesArray = ReshuffleArray(tilesArray);

        // Places the tiles
        tmpIdx = 0;

        GenerateSpawningAreas();
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
                    toInstantiate = tiles[tilesArray[tmpIdx]];
                    InstantiateTile(toInstantiate, new Vector3(i, j, 0f) * tileSize, false);
                    tmpIdx++;
                }
                
            }
        }
    }

    
    void Start ()
    {
        tileSize = 5f;     
        MapSetup();
        transform.position += new Vector3(-(columns-1) * tileSize / 2.0f, -(rows-1) * tileSize / 2.0f, 0f);
    }
	
	
	void Update ()
    {
		
	}
}

public class Coordinate
{
    int x, y;
    public Coordinate(int x, int y)
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
}