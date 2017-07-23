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

}