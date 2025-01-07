using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 1) Загружаем машины
        var allCars = CarDataManager.LoadAllCarStats();
        Debug.Log($"[Bootstrap] Cars loaded: {allCars.Length}");

        // 2) Загружаем игрока
        var playerStats = PlayerDataManager.LoadPlayerStats();
        Debug.Log($"[Bootstrap] Player name = {playerStats.GetName()}, money = {playerStats.GetMoney()}");

        // 3) Переход на сцену через GlobalEventManager
        GlobalEventManager.TriggerSceneChanged(sceneToLoad);
    }
}