using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public CarController carController;   // Ссылка на скрипт CarController
    public TMP_Text speedLabel;           // UI для скорости
    public TMP_Text driftPointsLabel;     // UI для очков
    public TMP_Text timerLabel;           // UI для таймера
    public TMP_Text rewardLabel;          // UI для итоговой награды

    [Header("Timer Settings")]
    public float levelDuration = 120f; // 2 минуты

    private float _timeLeft;
    private bool  _levelOver;

    // Подсчёт очков
    private float _driftPoints;
    private float _baseReward;

    private void Start()
    {
        _timeLeft = levelDuration;
        _driftPoints = 0f;
        _levelOver = false;

        // Подпишемся на события дрифта, чтобы знать, когда начался/закончился
        if (carController != null)
        {
            carController.OnDriftEvent += HandleDriftEvent;
        }
    }

    private void Update()
    {
        if (!_levelOver)
        {
            // Считаем таймер
            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0f)
            {
                _timeLeft = 0f;
                EndLevel();
            }

            // Если машина дрифтует, прибавляем очки (можно по формуле)
            if (carController != null && carController.IsDrifting)
            {
                // Пример: очки зависят от скорости
                float speedFactor = carController.CurrentSpeedKmh;
                _driftPoints += speedFactor * 5f * Time.deltaTime;
            }
        }

        // Обновляем UI:
        UpdateHUD();
    }

    private void HandleDriftEvent(bool started, float duration)
    {
        if (started)
        {
            Debug.Log("Дрифт начался!");
        }
        else
        {
            Debug.Log($"Дрифт закончился! Длился {duration:F2} с");
        }
    }

    private void EndLevel()
    {
        _levelOver = true;
        // Блокируем управление:
        if (carController != null)
        {
            carController.CanDrive = false;
        }

        // Считаем награду
        _baseReward = Mathf.Floor(_driftPoints / 10f);

        if (rewardLabel != null)
        {
            rewardLabel.text = 
                $"Level Over!\n" +
                $"Drift Points: {_driftPoints:0}\n" +
                $"Reward: ${_baseReward}\n" +
                "Press [D] to DOUBLE reward (simulated)";
        }
    }

    private void UpdateHUD()
    {
        // Таймер
        if (timerLabel != null)
        {
            timerLabel.text = $"Time: {_timeLeft:0.0}";
        }

        // Скорость
        if (speedLabel != null && carController != null)
        {
            speedLabel.text = $"Speed: {carController.CurrentSpeedKmh:0.0} km/h";
        }

        // Очки дрифта
        if (driftPointsLabel != null)
        {
            driftPointsLabel.text = $"Drift: {_driftPoints:0}";
        }
    }

    private void LateUpdate()
    {
        // Если уровень закончился, ждём нажатия клавиши D (для удвоения награды)
        if (_levelOver && Input.GetKeyDown(KeyCode.D))
        {
            // Показываем рекламу (пример)
            // IronSource.Agent.showRewardedVideo();

            // В реальном проекте нужно подписаться на колбэк onRewardedVideoAdRewardedEvent.
            // Здесь же просто имитируем успех:
            SimulateRewardedAdSuccess();
        }
    }

    private void SimulateRewardedAdSuccess()
    {
        float doubled = _baseReward * 2f;
        if (rewardLabel != null)
        {
            rewardLabel.text = 
                $"Reward AD success!\nBase = ${_baseReward}\nDoubled = ${doubled}";
        }
    }
}
