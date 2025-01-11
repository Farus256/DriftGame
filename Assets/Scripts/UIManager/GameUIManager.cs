using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }
    
    public TMP_Text speedLabel;
    public TMP_Text driftPointsLabel;
    public TMP_Text timerLabel;
    public TMP_Text rewardLabel;

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
            gameController.OnLevelEnd += UpdateRewardLabel;
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

    private void UpdateRewardLabel()
    {
        if (!rewardLabel) return;
        rewardLabel.text = 
            $"Level Over!\n" +
            $"Drift Points: {gameController.DriftPoints:0}\n" +
            $"Reward: ${gameController.BaseReward}\n" +
            "Press [D] to DOUBLE reward (watch ad)";
    }
}