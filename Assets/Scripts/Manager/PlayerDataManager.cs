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

    public static PlayerStats LoadPlayerStats()
    {
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
                return collection.PlayerStats;
            return CreateDefaultPlayerStats();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Load error: {e}");
            return CreateDefaultPlayerStats();
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
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Save error: {e}");
        }
    }

    private static void CreateDefaultPlayerFile(string path)
    {
        PlayerStats defaultStats = CreateDefaultPlayerStats();
        SavePlayerStats(defaultStats);
    }

    private static PlayerStats CreateDefaultPlayerStats()
    {
        return new PlayerStats("DefaultPlayer", 1000f, 1);
    }
}
