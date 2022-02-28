using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public static class Helper
{
    public static bool InRange(int x, int a, int b) 
    {
        //czy x jest w przedziale [a, b)
        return a <= x && x < b;
    }
    public static void Fill<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }

    public static Color Dim(this Color color, float amount)
    {
        Color newcolor = color * amount;
        newcolor.a = 1f;
        return newcolor;
    }
    public static Vector2 ToVector2(this (float, float) pair)
    {
        return new Vector2(pair.Item1, pair.Item2);
    }
    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }
    public static Vector3 ToVector3(this Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y, 0);
    }
    public static Color ColorFromHex(int hex)
    {
        return new Color((hex & 0xff0000 >> 16) / 255f,
                         (hex & 0x00ff00 >> 8) / 255f,
                         (hex & 0x0000ff) / 255f);
    }
    public static (float, float) ToFloat2(this Vector2 vector)
    {
        return (vector.x, vector.y);
    }
    public static void SaveClass<T>(this T _Class, string FilePath)
    {
        FileStream fileStream = File.Create(FilePath);
        new BinaryFormatter().Serialize(fileStream, _Class);
        fileStream.Close();
    }

    public static void DeleteClass(string FilePath)
    {
        if (!File.Exists(FilePath))
        {
            return;
        }
        File.Delete(FilePath);
    }

    public static T LoadClass<T>(string FilePath) where T : class
    {
        if (!File.Exists(FilePath))
        {
            return default(T);
        }
        FileStream fileStream = new FileStream(FilePath, FileMode.Open);
        T result = new BinaryFormatter().Deserialize(fileStream) as T;
        fileStream.Close();
        return result;
    }
}

[Serializable]
public struct Pair<T, U>
{
    public T st { get; set; }
    public U nd { get; set; }
    public Pair(T item1, U item2)
    {
        st = item1;
        nd = item2;
    }
}