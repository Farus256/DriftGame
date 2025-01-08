using System;
using System.IO;
using UnityEngine;

public static class CarDataManager
{
    private const string FileName = "cars_data.json";

    public static CarStats[] LoadAllCarStats()
    {
        string path = Path.Combine(Application.persistentDataPath, FileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[CarDataManager] File not found: {path}, creating default...");
            CreateDefaultCarFile(path);
        }

        try
        {
            string json = File.ReadAllText(path);
            CarStatsCollection collection = JsonUtility.FromJson<CarStatsCollection>(json);
            if (collection != null && collection.Cars != null)
            {
                Debug.Log($"[CarDataManager] Loaded {collection.Cars.Length} cars from {path}");
                return collection.Cars;
            }
            else
            {
                Debug.LogWarning("[CarDataManager] JSON parsed but cars is null. Returning empty array.");
                return Array.Empty<CarStats>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CarDataManager] Load error: {e}");
            return Array.Empty<CarStats>();
        }
    }
    // Метод для сохранения всех характеристик машин
    public static void SaveAllCarStats(CarStats[] cars)
    {
        CarStatsCollection collection = new CarStatsCollection();
        collection.SetCars(cars);
        string json = JsonUtility.ToJson(collection, true);
        string path = Path.Combine(Application.persistentDataPath, FileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            File.WriteAllText(path, json);
            Debug.Log($"[CarDataManager] Saved {cars.Length} cars to {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CarDataManager] Save error: {e}");
        }
    }
    private static void CreateDefaultCarFile(string path)
    {
        // Минимальный дефолтный JSON
        string defaultJson = @"{
    ""cars"": [
        {
            ""id"": 1,
            ""carName"": ""Drift King"",
            ""mass"": 1000,
            ""motorPower"": 2000,
            ""brakeForce"": 2500,
            ""prefabName"": ""Samurai_Green_A""
        },
        {
            ""id"": 2,
            ""carName"": ""Samurai Blue"",
            ""mass"": 1200,
            ""motorPower"": 1600,
            ""brakeForce"": 2000,
            ""prefabName"": ""Samurai_Blue_A""
        }
    ]
}";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            File.WriteAllText(path, defaultJson);
            Debug.Log($"[CarDataManager] Created default {FileName} at {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CarDataManager] Could not create default file: {e}");
        }
    }
}

