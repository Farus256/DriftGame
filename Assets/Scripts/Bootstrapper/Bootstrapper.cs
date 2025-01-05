using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    private void Awake()
    {
        // 1) Загружаем машины
        CarStats[] allCars = CarDataManager.LoadAllCarStats();
        Debug.Log($"[Bootstrap] Cars loaded: {allCars.Length}");

        // 2) Загружаем игрока
        PlayerStats playerStats = PlayerDataManager.LoadPlayerStats();
        Debug.Log($"[Bootstrap] Player name = {playerStats.playerName}, money = {playerStats.money}");

        // Дальше переход в сцену Меню
        SceneManager.LoadScene("MenuScene");
    }
}