using UnityEngine;
using Photon.Pun;

//Класс для контроля действий при запущенном уровне (singl/multiplayer)
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
        
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            MultiPlayerCarSpawner.OnLocalCarSpawned += HandleLocalCarSpawned;
        }
        else
        {
            GameCarSpawner.OnSinglePlayerCarSpawned += HandleSinglePlayerCarSpawned;
        }
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            MultiPlayerCarSpawner.OnLocalCarSpawned -= HandleLocalCarSpawned;
        }
        else
        {
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
    
    private void HandleLocalCarSpawned(GameObject localCar)
    {
        AssignLocalPlayerCar(localCar);
    }
    
    private void HandleSinglePlayerCarSpawned(GameObject car)
    {
        AssignLocalPlayerCar(car);
    }
   
    //Проверка назначения локального авто
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
    
    public float GetTimeLeft()
    {
        return _timeLeft;
    }
    
    public void ResetGame()
    {
        _timeLeft = LevelDuration;
        DriftPoints = 0f;
        LevelOver = false;
    }
    
    private void EndLevel()
    {
        LevelOver = true;
        BaseReward = Mathf.Floor(DriftPoints / 10f);

        if (LocalPlayerCar != null)
        {
            LocalPlayerCar.CanDrive = false;
        }

        OnLevelEnd?.Invoke();
    }
}
