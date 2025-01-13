using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    private static GameUIManager Instance { get; set; }
    
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text driftPointsLabel;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private TMP_Text rewardLabel;
    
    [SerializeField] private GameObject endGamePanel;
    
    [SerializeField] private IronSourceLevelPlayManager ironSourceManager;
    
    private GameController gameController;

    private bool shouldDoubleReward = false;

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
        gameController = GameController.Instance;
        if (gameController != null)
            gameController.OnLevelEnd += UpdateRewardPanel;
        
        if (ironSourceManager == null)
        {
            ironSourceManager = FindObjectOfType<IronSourceLevelPlayManager>();
            if (ironSourceManager == null)
            {
                Debug.LogError("IronSourceLevelPlayManager не найден в сцене.");
            }
        }

        // Подписка на глобальное событие награждения
        GlobalEventManager.OnRewardedVideoCompleted += HandleRewarded;
    }

    private void Update()
    {
        if (gameController == null) return;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (timerLabel)
            timerLabel.text = $"Time: {gameController.GetTimeLeft():0.0}";
        if (driftPointsLabel)
            driftPointsLabel.text = $"Drift: {gameController.DriftPoints:0}";
        if (speedLabel && gameController.LocalPlayerCar)
            speedLabel.text = $"Speed: {gameController.LocalPlayerCar.CurrentSpeedKmh:0.0} km/h";
    }

    private void UpdateRewardPanel()
    {
        if (!endGamePanel) return;
        endGamePanel.SetActive(true);
        
        if (!rewardLabel) return;
        rewardLabel.text = 
            $"Level Over!\n" +
            $"Drift Points: {gameController.DriftPoints:0}\n" +
            $"Reward: ${gameController.BaseReward}\n" +
            "Press to DOUBLE reward";
    }

    /// <summary>
    /// Метод, вызываемый при нажатии кнопки завершения уровня
    /// </summary>
    /// <param name="isDouble">Должна ли быть награда удвоена</param>
    public void OnEndLevelButton(bool isDouble)
    {
        if (isDouble)
        {
            Debug.Log("[GameUIManager] Showing Rewarded Ad...");
            shouldDoubleReward = true;
            ironSourceManager.ShowRewardedVideo();
            // Не добавляем деньги здесь, ждем события награждения
        }
        else
        {
            RewardPlayer(false);
        }
    }

    /// <summary>
    /// Обработка события награждения после просмотра рекламы
    /// </summary>
    void HandleRewarded()
    {
        RewardPlayer(shouldDoubleReward);
        shouldDoubleReward = false;
    }

    /// <summary>
    /// Метод для награждения игрока
    /// </summary>
    /// <param name="isDouble">Должна ли быть награда удвоена</param>
    void RewardPlayer(bool isDouble)
    {
        PlayerStats playerstats = PlayerDataManager.LoadPlayerStats();

        if (isDouble)
        {
            playerstats.AddMoney(GameController.Instance.BaseReward * 2);
            Debug.Log("Награда удвоена после просмотра рекламы.");
        }
        else
        {
            playerstats.AddMoney(GameController.Instance.BaseReward);
        }

        if(PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        
        GlobalEventManager.TriggerLevelEnd();
    }

    void OnDestroy()
    {
        // Отписка от глобального события награждения
        GlobalEventManager.OnRewardedVideoCompleted -= HandleRewarded;

        // Отписка от события окончания уровня, если необходимо
        if (gameController != null)
        {
            gameController.OnLevelEnd -= UpdateRewardPanel;
        }
    }
}
