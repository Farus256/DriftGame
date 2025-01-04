using UnityEngine;
using System.IO;

public static class PlayerDataManager
{
    private static string _fileName = "player_stats.json";

    /// <summary>
    /// Путь к файлу с учётом платформы.
    /// Например: Application.persistentDataPath
    /// </summary>
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, _fileName);
    }

    /// <summary>
    /// Сохранить статистику игрока в JSON.
    /// </summary>
    public static void SavePlayerStats(PlayerStats stats)
    {
        try
        {
            string json = JsonUtility.ToJson(stats, true);
            File.WriteAllText(GetFilePath(), json);
            Debug.Log($"[PlayerDataManager] Player stats saved to: {GetFilePath()}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("SavePlayerStats error: " + e);
        }
    }

    /// <summary>
    /// Загрузить статистику игрока из JSON.
    /// Если файла нет — вернём новую статистику по умолчанию.
    /// </summary>
    public static PlayerStats LoadPlayerStats()
    {
        string path = GetFilePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("Player stats file not found, creating new stats...");
            // Возвращаем новые статы по умолчанию
            return new PlayerStats
            {
                playerName = "Player",
                money = 0,
                totalDriftPoints = 0,
                level = 1
            };
        }

        try
        {
            string json = File.ReadAllText(path);
            PlayerStats loadedStats = JsonUtility.FromJson<PlayerStats>(json);
            Debug.Log($"[PlayerDataManager] Player stats loaded from: {path}");
            return loadedStats;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadPlayerStats error: " + e);
            // Если ошибка, вернём что-нибудь по умолчанию
            return new PlayerStats();
        }
    }
}