using System;
using System.IO;
using UnityEngine;

public static class CarDataManager
{
    private const string FileName = "cars_data.json";
    
    // Внутренний кэш CarStats, чтобы не парсить файл постоянно
    private static CarStats[] _cachedCars;

    public static CarStats[] LoadAllCarStats()
    {
        // Если у нас уже есть кэш, можно сразу вернуть
        if (_cachedCars != null && _cachedCars.Length > 0)
            return _cachedCars;

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
                _cachedCars = collection.Cars;
                return _cachedCars;
            }
            else
            {
                Debug.LogWarning("[CarDataManager] JSON parsed but cars is null. Returning empty array.");
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

    // Метод для сохранения всех характеристик машин
    public static void SaveAllCarStats(CarStats[] cars)
    {
        // Сохраняем в кэш
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

    // Новый метод для получения CarStats по ID
    public static CarStats GetCarStatsById(int carId)
    {
        // Загружаем все машины (или берём их из кэша)
        CarStats[] allCars = LoadAllCarStats();
        
        if (allCars == null || allCars.Length == 0)
            return null;

        // Перебираем и ищем нужный ID
        foreach (CarStats car in allCars)
        {
            if (car.ID == carId)
                return car;
        }
        return null;
    }

    private static void CreateDefaultCarFile(string path)
    {
        // Дефолтный JSON с дополнительными деталями, цветом покраски и ценой
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
            Debug.Log($"[CarDataManager] Created default {FileName} at {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CarDataManager] Could not create default file: {e}");
        }
    }
}
