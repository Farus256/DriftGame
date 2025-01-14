using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerStatsCollection
{
    [SerializeField] private PlayerStats playerStats;
    public PlayerStats PlayerStats => playerStats;
    public void SetPlayerStats(PlayerStats stats) => playerStats = stats;
}

public static class PlayerDataManager
{
    private const string FileName = "player_data.json";
    private static PlayerStats cachedPlayerStats; // Кэшируемый экземпляр PlayerStats

    public static PlayerStats LoadPlayerStats()
    {
        if (cachedPlayerStats != null)
            return cachedPlayerStats; // Используем кэшированный экземпляр

        string path = Path.Combine(Application.persistentDataPath, FileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[PlayerDataManager] No file at {path}, creating default...");
            CreateDefaultPlayerFile(path);
        }
        try
        {
            string json = File.ReadAllText(path);
            PlayerStatsCollection collection = JsonUtility.FromJson<PlayerStatsCollection>(json);
            if (collection != null && collection.PlayerStats != null)
            {
                cachedPlayerStats = collection.PlayerStats;
                return cachedPlayerStats;
            }
            return CreateAndCacheDefaultPlayerStats();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Load error: {e}");
            return CreateAndCacheDefaultPlayerStats();
        }
    }

    public static void SavePlayerStats(PlayerStats stats)
    {
        string path = Path.Combine(Application.persistentDataPath, FileName);
        PlayerStatsCollection collection = new PlayerStatsCollection();
        collection.SetPlayerStats(stats);
        string json = JsonUtility.ToJson(collection, true);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
            File.WriteAllText(path, json);
            
            cachedPlayerStats = stats;
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Save error: {e}");
        }
    }

    private static void CreateDefaultPlayerFile(string path)
    {
        PlayerStats defaultStats = CreateAndCacheDefaultPlayerStats();
        SavePlayerStats(defaultStats);
    }

    private static PlayerStats CreateAndCacheDefaultPlayerStats()
    {
        cachedPlayerStats = new PlayerStats("DefaultPlayer", 10000f, 1, 100);
        return cachedPlayerStats;
    }
}
