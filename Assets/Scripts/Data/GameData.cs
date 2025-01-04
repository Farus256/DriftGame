using UnityEngine;

/// <summary>
/// Центральное хранилище данных, загружаемых при старте проекта.
/// Имеет ссылки на PlayerStats, CarStats и т.д.
/// </summary>
public static class GameData
{
    /// <summary>
    /// Текущая статистика игрока (загружается из JSON, например).
    /// </summary>
    public static PlayerStats PlayerStats { get; set; }

    /// <summary>
    /// Список всех машин, которые существуют в игре (загружаются из JSON).
    /// </summary>
    public static CarStats[] AllCarStats { get; set; }

    /// <summary>
    /// Текущая выбранная машина (если нужно запомнить выбор из меню).
    /// </summary>
    public static CarStats SelectedCar { get; set; }

    /// <summary>
    /// Инициализация GameData: 
    /// - Загружаем PlayerStats
    /// - Загружаем AllCarStats
    /// - Выбираем по умолчанию какую-то машину (при желании)
    /// </summary>
    public static void Initialize()
    {
        // 1) Загружаем статистику игрока
        PlayerStats = PlayerDataManager.LoadPlayerStats();

        // 2) Загружаем характеристики всех машин
        AllCarStats = CarDataManager.LoadAllCarStats();

        // 3) Допустим, выберем нулевую машину по умолчанию
        if (AllCarStats != null && AllCarStats.Length > 0)
        {
            SelectedCar = AllCarStats[0];
        }
        else
        {
            SelectedCar = null;
        }
        
        Debug.Log("[GameData] Initialized.");
    }

    /// <summary>
    /// Сохраняем всю нужную информацию (например, статистику игрока).
    /// </summary>
    public static void SaveAll()
    {
        // Сохраняем PlayerStats
        if (PlayerStats != null)
        {
            PlayerDataManager.SavePlayerStats(PlayerStats);
        }

        // Если нужно, можем сохранять и данные о машинах,
        // но обычно характеристики машин хранятся в одном JSON и не меняются на ходу.
        // Если у вас сценарий, где данные машин меняются (апгрейды), 
        // можно сделать CarDataManager.SaveAllCarStats(AllCarStats) 
        // и реализовать его.
        
        Debug.Log("[GameData] SaveAll() called.");
    }
}
