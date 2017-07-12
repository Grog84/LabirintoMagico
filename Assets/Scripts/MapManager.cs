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

    private List<Vector3> gridPositions = new List<Vector3>();
    private float tileSize;

    int[] GenerateInitialTiles()
    {
        int[] nbrOfTiles;
        nbrOfTiles = new int[4] { 0, 0, 0, 0 }; // 1st index is curves number, 2nd is straights, 3rd is ts, 4th is crosses
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                nbrOfTiles[i] = Random.Range(curvesCount.minimum, curvesCount.maximum+1);
            else if(i==1)
                nbrOfTiles[i] = Random.Range(straightCount.minimum, straightCount.maximum + 1);
            else if (i == 2)
                nbrOfTiles[i] = Random.Range(tCount.minimum, tCount.maximum + 1);
            else if (i == 3)
                nbrOfTiles[i] = Random.Range(crossCount.minimum, crossCount.maximum + 1);
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
            while (matrix.ContainsKey(idx)) { idx = r.Next(); }

            matrix.Add(idx, nbrOfTiles[i]);
        }

        matrix.Values.CopyTo(reshuffledArray, 0);

        return reshuffledArray;

    }

    void MapSetup()
    {
        int finalTilesNbr = columns * rows, generatedTilesNbr = 0;
        int[] randomIndex, tilesArray;
        int[] nbrOfTiles = GenerateInitialTiles();
        for (int i =0; i < nbrOfTiles.Length; i++)
            generatedTilesNbr += nbrOfTiles[i];

        if (generatedTilesNbr != finalTilesNbr)
        {
            if (generatedTilesNbr > finalTilesNbr)
            {
                while(generatedTilesNbr != finalTilesNbr)
                {
                    int indx;
                    indx = Random.Range(0, 4);
                    if (indx == 0 && nbrOfTiles[indx] > curvesCount.minimum)
                    {
                        nbrOfTiles[indx] -= 1;
                        generatedTilesNbr -= 1;
                    }
                    else if (indx == 1 && nbrOfTiles[indx] > straightCount.minimum)
                    {
                        nbrOfTiles[indx] -= 1;
                        generatedTilesNbr -= 1;
                    }
                    else if (indx == 2 && nbrOfTiles[indx] > tCount.minimum)
                    {
                        nbrOfTiles[indx] -= 1;
                        generatedTilesNbr -= 1;
                    }
                    else if (indx == 3 && nbrOfTiles[indx] > crossCount.minimum)
                    {
                        nbrOfTiles[indx] -= 1;
                        generatedTilesNbr -= 1;
                    }
                }
       
            }
            else
            {
                while (generatedTilesNbr != finalTilesNbr)
                {
                    int indx;
                    indx = Random.Range(0, 4);
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
        tilesArray = new int[finalTilesNbr];
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

        tilesArray = ReshuffleArray(tilesArray);

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject toInstantiate;

                if (i == 0 && j == 0)
                {
                    toInstantiate = startingPoint[2];
                }
                else if (i == columns-1 && j == 0)
                {
                    toInstantiate = startingPoint[3];
                }
                else if (i == 0 && j == rows-1)
                {
                    toInstantiate = startingPoint[0];
                }
                else if (i == columns-1 && j == rows-1)
                {
                    toInstantiate = startingPoint[1];
                }
                else if (i == columns / 2 && j == rows / 2)
                {
                    toInstantiate = goalPoint;
                }
                else
                {
                    //toInstantiate = tiles[Random.Range(0, tiles.Length)];
                    toInstantiate = tiles[tilesArray[i* rows + j]];
                }

                GameObject instance = (GameObject)Instantiate(toInstantiate, new Vector3(i, j, 0f) * tileSize, Quaternion.identity);
                instance.transform.SetParent(this.transform);
            }
        }
    }

    // Use this for initialization
    void Start () {

        tileSize = 5f;     
        MapSetup();
        transform.position += new Vector3(-(columns-1) * tileSize / 2.0f, -(rows-1) * tileSize / 2.0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
