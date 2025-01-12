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
    
    private GameController gameController;

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

    public void OnEndLevelButton(bool isDouble)
    {
        PlayerStats playerstats = PlayerDataManager.LoadPlayerStats();

        if (isDouble)
        {
            Debug.Log("[GameUIManager] Showing Rewarded Ad...");
            playerstats.AddMoney(GameController.Instance.BaseReward * 2);
        }
        
        else playerstats.AddMoney(GameController.Instance.BaseReward);
        
        GlobalEventManager.TriggerLevelEnd();
    }
}