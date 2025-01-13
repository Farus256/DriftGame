using UnityEngine;
using Photon.Pun;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Game Settings")]
    public float LevelDuration = 120f;
    public float DriftPoints { get; private set; }
    public float BaseReward { get; private set; }
    public bool LevelOver { get; private set; }
    public CarController LocalPlayerCar { get; private set; }

    private float _timeLeft;

    public delegate void GameEventHandler();
    public event GameEventHandler OnLevelEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Подписываемся на событие спавна локального автомобиля в мультиплеере
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            MultiPlayerCarSpawner.OnLocalCarSpawned += HandleLocalCarSpawned;
        }
        else
        {
            // Подписываемся на событие спавна автомобиля в однопользовательском режиме
            GameCarSpawner.OnSinglePlayerCarSpawned += HandleSinglePlayerCarSpawned;
        }
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Отписываемся от события при уничтожении объекта в мультиплеере
            MultiPlayerCarSpawner.OnLocalCarSpawned -= HandleLocalCarSpawned;
        }
        else
        {
            // Отписываемся от события при уничтожении объекта в однопользовательском режиме
            GameCarSpawner.OnSinglePlayerCarSpawned -= HandleSinglePlayerCarSpawned;
        }
    }

    private void Start()
    {
        ResetGame();
    }

    private void Update()
    {
        if (LevelOver)
        {
            // Здесь можно добавить логику для окончания уровня (например, показать UI)
            return;
        }

        // Таймер
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            EndLevel();
        }

        // Логика подсчёта очков
        if (LocalPlayerCar != null && LocalPlayerCar.IsDrifting)
        {
            float speedFactor = LocalPlayerCar.CurrentSpeedKmh;
            DriftPoints += speedFactor * 5f * Time.deltaTime;
        }
    }

    /// <summary>
    /// Обработка спавна локального автомобиля в мультиплеере
    /// </summary>
    /// <param name="localCar">Заспавненный локальный автомобиль</param>
    private void HandleLocalCarSpawned(GameObject localCar)
    {
        AssignLocalPlayerCar(localCar);
    }

    /// <summary>
    /// Обработка спавна автомобиля в однопользовательском режиме
    /// </summary>
    /// <param name="car">Заспавненный автомобиль</param>
    private void HandleSinglePlayerCarSpawned(GameObject car)
    {
        AssignLocalPlayerCar(car);
    }

    /// <summary>
    /// Назначение CarController из переданного объекта автомобиля
    /// </summary>
    /// <param name="car">Объект автомобиля</param>
    private void AssignLocalPlayerCar(GameObject car)
    {
        LocalPlayerCar = car.GetComponent<CarController>();
        if (LocalPlayerCar == null)
        {
            Debug.LogWarning($"[GameController] CarController не найден на объекте: {car.name}");
        }
        else
        {
            Debug.Log($"[GameController] Локальный автомобиль назначен: {car.name}");
        }
    }

    /// <summary>
    /// Возвращает оставшееся время уровня
    /// </summary>
    /// <returns>Оставшееся время в секундах</returns>
    public float GetTimeLeft()
    {
        return _timeLeft;
    }

    /// <summary>
    /// Сброс параметров игры
    /// </summary>
    public void ResetGame()
    {
        _timeLeft = LevelDuration;
        DriftPoints = 0f;
        LevelOver = false;
    }

    /// <summary>
    /// Завершение уровня
    /// </summary>
    private void EndLevel()
    {
        LevelOver = true;
        BaseReward = Mathf.Floor(DriftPoints / 10f);

        if (LocalPlayerCar != null)
        {
            LocalPlayerCar.CanDrive = false;
        }

        OnLevelEnd?.Invoke();

        // Здесь можно добавить логику награждения, перехода на следующий уровень и т.д.
    }
}
