using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InsertArrow : MonoBehaviour {

    public Coordinate[] pointedTilesCoord;

    public Coordinate[] getPointedCoords()
    {
        return pointedTilesCoord;
    }

    int[] getRange(int a, int b) // includes b
    {
        int[] rangeArray;

        if (a > b)
        {
            int diff = a - b + 1;
            rangeArray = new int[diff];

            for (int i = 0; i < diff; i++)
            {
                rangeArray[i] = a - i;
            }
        }
        else if (b > a)
        {
            int diff = b - a + 1;
            rangeArray = new int[diff];

            for (int i = 0; i < diff; i++)
            {
                rangeArray[i] = a + i;
            }

        }
        else
        {
            rangeArray = new int[1];
            rangeArray[0] = a;
        }

        return rangeArray;
    }

    public void setPointedCoords(int xInit, int xFinal, int yInit, int yFinal)
    {
        int[] xComponents = new int[0], yComponents=new int[0];
        int coordArrayLength = Mathf.Max(Mathf.Abs(xFinal - xInit), Mathf.Abs(yFinal - yInit)) + 1;
        pointedTilesCoord = new Coordinate[coordArrayLength];

        if (xInit != xFinal) // horizontal arrow
        {
            xComponents = getRange(xInit, xFinal);
            yComponents = Enumerable.Repeat(yInit, coordArrayLength).ToArray();    
        }

        if (yInit != yFinal) // vertical arrow
        {
            yComponents = getRange(yInit, yFinal);
            xComponents = Enumerable.Repeat(xInit, coordArrayLength).ToArray();
        }

        for (int i = 0; i < coordArrayLength; i++)
        {
            pointedTilesCoord[i] = new Coordinate(xComponents[i], yComponents[i]);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
