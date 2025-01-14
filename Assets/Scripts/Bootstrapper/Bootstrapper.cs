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
        //Загружаем машины
        var allCars = CarDataManager.LoadAllCarStats();
        Debug.Log($"[Bootstrap] Cars loaded: {allCars.Length}");

        //Загружаем игрока
        var playerStats = PlayerDataManager.LoadPlayerStats();
        Debug.Log($"[Bootstrap] Player name = {playerStats.PlayerName}, money = {playerStats.Money}");

        //Переход на сцену через GlobalEventManager
        GlobalEventManager.TriggerSceneChanged(sceneToLoad);
    }
}