using UnityEngine;
using Photon.Pun;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

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
    }

    private void Start()
    {
        ResetGame();
        FindLocalPlayerCar();
    }

    public void ResetGame()
    {
        _timeLeft = LevelDuration;
        DriftPoints = 0f;
        LevelOver = false;
    }

    private void Update()
    {
        if (LevelOver)
        {
            CheckForDoubleReward();
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

    private void FindLocalPlayerCar()
    {
        GameObject[] playerCars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject car in playerCars)
        {
            // Для мультиплеера проверяем PhotonView
            if (PhotonNetwork.IsConnectedAndReady)
            {
                var photonView = car.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    AssignLocalPlayerCar(car);
                    return;
                }
            }
            else
            {
                // В синглплеере берём первую найденную машину
                AssignLocalPlayerCar(car);
                return;
            }
        }
        Debug.LogWarning("[GameController] Local player car not found!");
    }

    private void AssignLocalPlayerCar(GameObject car)
    {
        LocalPlayerCar = car.GetComponent<CarController>();
        if (LocalPlayerCar == null)
        {
            Debug.LogWarning($"[GameController] CarController not found on: {car.name}");
        }
    }

    public float GetTimeLeft()
    {
        return _timeLeft;
    }

    private void EndLevel()
    {
        
        LevelOver = true;
        BaseReward = Mathf.Floor(DriftPoints / 10f);
        
        LocalPlayerCar.CanDrive = false;
        OnLevelEnd?.Invoke();
        GlobalEventManager.TriggerLevelEnd();
    }

    private void CheckForDoubleReward()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                Debug.Log("[GameUIManager] Showing Rewarded Ad...");
                IronSource.Agent.showRewardedVideo();
                
                UpdatePlayerMoney(BaseReward *= 2);
            }
            else
            {
                Debug.Log("[GameUIManager] Rewarded Ad not available.");
                UpdatePlayerMoney(BaseReward);
            }
        }
    }

    private void UpdatePlayerMoney(float reward)
    {
        // Загружаем текущие данные игрока
        PlayerStats stats = PlayerDataManager.LoadPlayerStats();

        // Добавляем награду к текущему балансу
        stats.AddMoney(reward);

        // Сохраняем обновлённые данные
        PlayerDataManager.SavePlayerStats(stats);

        Debug.Log($"[GameController] Player money updated: +${reward}. Total: ${stats.GetMoney()}");
    }
}
