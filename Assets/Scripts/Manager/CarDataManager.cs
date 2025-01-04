using System;
using System.IO;
using UnityEngine;

public static class CarDataManager
{
    private static readonly string FileName = "cars_data.json";

    public static CarStats[] LoadAllCarStats()
    {
        string path = Path.Combine(Application.persistentDataPath, FileName);

        if (!File.Exists(path))
        {
            Debug.LogError("Car stats file not found: " + path);
            return Array.Empty<CarStats>();
        }

        try
        {
            string json = File.ReadAllText(path);
            CarStatsCollection collection = JsonUtility.FromJson<CarStatsCollection>(json);
            return collection.cars;
        }
        catch (Exception e)
        {
            Debug.LogError("Loading error" + e);
            return Array.Empty<CarStats>();
        }
    }
}

[System.Serializable]
public class CarStatsCollection
{
    public CarStats[] cars;
}