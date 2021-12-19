using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public static class Helpers
{
    public static void Fill<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
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
    public static T LoadClass<T>(this T _Class, string FilePath) where T : class
    {
        if (!File.Exists(FilePath))
        {
            return default(T);
        }
        FileStream fileStream = new FileStream(FilePath, FileMode.Open);
        _Class = (new BinaryFormatter().Deserialize(fileStream) as T);
        fileStream.Close();
        return _Class;
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
