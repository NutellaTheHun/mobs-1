using System;
using System.Collections.Generic;


public static class Utilities
{
    private static Random random = new Random();

    public static int GetRandomNumber(int min, int max)
    {
        return random.Next(min, max);
    }
    //Given an array, creates list of smallest value elements and returns one randomly
    public static int getRandomSmallestElement(int[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            throw new ArgumentException("Array null or empty.");
        }

        List<int> sameSizeElements = new List<int>();
        int smallVal = arr[0];

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] < smallVal)
            {
                smallVal = arr[i];
                sameSizeElements.Clear();
                sameSizeElements.Add(i);
            }
            else if (arr[i] == smallVal)
            {
                sameSizeElements.Add(i);
            }
        }

        int listSize = sameSizeElements.Count;

        int chance = GetRandomNumber(0, listSize);

        return sameSizeElements[chance];
    }

}
