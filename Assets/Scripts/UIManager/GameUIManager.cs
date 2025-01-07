using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text speedLabel;
    public TMP_Text driftPointsLabel;
    public TMP_Text timerLabel;
    public TMP_Text rewardLabel;

    private GameController _gameController;

    private void Start()
    {
        _gameController = GameController.Instance;
        if (_gameController != null)
        {
            _gameController.OnLevelEnd += UpdateRewardLabel;
        }
    }

    private void Update()
    {
        if (_gameController == null) return;

        // Обновляем HUD
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // Таймер
        if (timerLabel != null)
        {
            timerLabel.text = $"Time: {_gameController.GetTimeLeft():0.0}";
        }

        // Очки дрифта
        if (driftPointsLabel != null)
        {
            driftPointsLabel.text = $"Drift: {_gameController.DriftPoints:0}";
        }

        // Скорость машины
        if (speedLabel != null && _gameController.LocalPlayerCar != null)
        {
            speedLabel.text = $"Speed: {_gameController.LocalPlayerCar.CurrentSpeedKmh:0.0} km/h";
        }
    }

    private void UpdateRewardLabel()
    {
        if (rewardLabel != null)
        {
            rewardLabel.text =
                $"Level Over!\n" +
                $"Drift Points: {_gameController.DriftPoints:0}\n" +
                $"Reward: ${_gameController.BaseReward}\n" +
                "Press [D] to DOUBLE reward (watch ad)";
        }
    }
}