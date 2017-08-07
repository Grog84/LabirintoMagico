using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GeneralMethods
{
    static public int FindElementIdx(int[] intArray, int value)
    {
        int idx = -1;
        for (int i = 0; i < intArray.Length; i++)
        {
            if (intArray[i] == value)
                return i;
        }
        return idx;
    }

    static public int FindElementIdx(Coordinate[] intArray, Coordinate value)
    {
        int idx = -1;
        for (int i = 0; i < intArray.Length; i++)
        {
            if (intArray[i].isEqual(value))
                return i;
        }
        return idx;
    }


    static public Coordinate[] ReverseArray(Coordinate[] coordArray)
    {
        var reverseArray = new Coordinate[coordArray.Length];
        for (int i = 0; i < reverseArray.Length; i++)
        {
            reverseArray[reverseArray.Length - 1 - i] = coordArray[i];
        }
        return reverseArray;
    }

    static public int[] ReshuffleArray(int[] myArray)
    {
        int[] reshuffledArray;
        var matrix = new SortedList();
        reshuffledArray = new int[myArray.Length];

        var r = new System.Random();
        for (int i = 0; i < myArray.Length; i++)
        {
            var idx = r.Next();
            // grants no duplicates idx
            while (matrix.ContainsKey(idx)) { idx = r.Next(); }

            matrix.Add(idx, myArray[i]);
        }

        matrix.Values.CopyTo(reshuffledArray, 0);
        return reshuffledArray;

    }

    static public float[] CreateBins(float binDimension, float initialValue, int numberOfBins)
    {
        var bins = new float[numberOfBins];
        bins[0] = initialValue;
        for (int i = 1; i < numberOfBins; i++)
        {
            bins[i] = bins[i - 1] + binDimension;
        }
        return bins;

    }

    static public int FindValInBins(float[] bins, float val)
    {
        int indx = -1;
        for (int i = 1; i < bins.Length; i++)
        {
            if (val >= bins[i - 1] && val < bins[i])
            {
                indx = i - 1;
                break;
            }

        }

        return indx;
    }

    static public Vector3 GetVect3Midpoint(Vector3 vec1, Vector3 vec2)
    {
        var mid = Vector3.zero;
        mid.x = (vec1.x + vec2.x) / 2f;
        mid.y = (vec1.y + vec2.y) / 2f;
        mid.z = (vec1.z + vec2.z) / 2f;
        return mid;
    }

}