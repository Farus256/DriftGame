using System;
using System.IO;
using UnityEngine;

public static class CarDataManager
{
    private const string FileName = "cars_data.json";
    private static CarStats[] _cachedCars;

    public static CarStats[] LoadAllCarStats()
    {
        if (_cachedCars != null && _cachedCars.Length > 0) return _cachedCars;
        string path = Path.Combine(Application.persistentDataPath, FileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[CarDataManager] No file at {path}, creating default...");
            CreateDefaultCarFile(path);
        }
        try
        {
            string json = File.ReadAllText(path);
            CarStatsCollection collection = JsonUtility.FromJson<CarStatsCollection>(json);
            if (collection != null && collection.Cars != null)
            {
                _cachedCars = collection.Cars;
                Debug.Log($"[CarDataManager] Loaded {_cachedCars.Length} cars");
                return _cachedCars;
            }
            else
            {
                _cachedCars = Array.Empty<CarStats>();
                return _cachedCars;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CarDataManager] Load error: {e}");
            _cachedCars = Array.Empty<CarStats>();
            return _cachedCars;
        }
    }

    public static void SaveAllCarStats(CarStats[] cars)
    {
        _cachedCars = cars;
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

    public static CarStats GetCarStatsById(int carId)
    {
        CarStats[] allCars = LoadAllCarStats();
        if (allCars == null || allCars.Length == 0) return null;
        foreach (CarStats car in allCars)
            if (car.ID == carId)
                return car;
        return null;
    }

    private static void CreateDefaultCarFile(string path)
    {
        string defaultJson = @"{
    ""cars"": [
        {
            ""id"": 1,
            ""carName"": ""Drift King"",
            ""mass"": 1000,
            ""motorPower"": 2000,
            ""brakeForce"": 2500,
            ""prefabName"": ""Samurai_Green_A"",
            ""availableExtraParts"": [""Spoiler"", ""SideSkirts""],
            ""activeExtraParts"": [],
            ""paintColor"": ""#FFFFFF"",
            ""cost"": 8000
        },
        {
            ""id"": 2,
            ""carName"": ""Samurai Blue"",
            ""mass"": 1200,
            ""motorPower"": 1600,
            ""brakeForce"": 2000,
            ""prefabName"": ""Samurai_Blue_A"",
            ""availableExtraParts"": [""Spoiler"", ""SideSkirts""],
            ""activeExtraParts"": [],
            ""paintColor"": ""#FFFFFF"",
            ""cost"": 6000
        }
    ]
}";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            File.WriteAllText(path, defaultJson);
            Debug.Log($"[CarDataManager] Created default file at {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CarDataManager] Could not create default file: {e}");
        }
    }
}
