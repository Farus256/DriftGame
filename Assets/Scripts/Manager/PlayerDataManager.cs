using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Менеджер для чтения и записи player_stats.json.
/// Если файла нет – создаём дефолтный.
/// Подходит для динамических данных игрока (деньги, уровень...).
/// </summary>
public static class PlayerDataManager
{
    private const string FileName = "player_stats.json";

    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, FileName);
    }

    /// <summary>
    /// Загружаем PlayerStats. Если файла нет, создаём дефолт и возвращаем его.
    /// </summary>
    public static PlayerStats LoadPlayerStats()
    {
        string path = GetFilePath();

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[PlayerDataManager] File not found at {path}, creating default...");
            CreateDefaultPlayerFile(path);
        }

        try
        {
            string json = File.ReadAllText(path);
            PlayerStats loadedStats = JsonUtility.FromJson<PlayerStats>(json);

            if (loadedStats == null)
            {
                Debug.LogWarning("[PlayerDataManager] JSON parsed but returned null. Using default stats.");
                loadedStats = CreateDefaultPlayerStats();
            }

            Debug.Log($"[PlayerDataManager] Loaded player_stats.json from {path}");
            return loadedStats;
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Load error: {e}");
            return CreateDefaultPlayerStats();
        }
    }

    /// <summary>
    /// Сохраняем текущее состояние PlayerStats (например, после изменения денег).
    /// </summary>
    public static void SavePlayerStats(PlayerStats stats)
    {
        string path = GetFilePath();
        try
        {
            string json = JsonUtility.ToJson(stats, true);

            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            File.WriteAllText(path, json);

            Debug.Log($"[PlayerDataManager] Saved player_stats.json to {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Save error: {e}");
        }
    }

    //--------------------------------------------------------------------------
    // Вспомогательные методы
    //--------------------------------------------------------------------------

    private static void CreateDefaultPlayerFile(string path)
    {
        PlayerStats def = CreateDefaultPlayerStats();
        string json = JsonUtility.ToJson(def, true);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            File.WriteAllText(path, json);
            Debug.Log($"[PlayerDataManager] Created default player_stats.json at {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Could not create default file: {e}");
        }
    }

    private static PlayerStats CreateDefaultPlayerStats()
    {
        return new PlayerStats
        {
            playerName = "Player",
            money = 1000.0f,
            totalDriftPoints = 2500.5f,
            level = 3
        };
    }
}
