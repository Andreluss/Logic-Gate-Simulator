using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static void Fill<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }

}
