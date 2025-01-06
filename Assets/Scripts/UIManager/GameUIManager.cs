using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text speedLabel;
    public TMP_Text driftPointsLabel;
    public TMP_Text timerLabel;
    public TMP_Text rewardLabel;

    [Header("Timer Settings")]
    public float levelDuration = 120f;

    private bool _levelOver;
    private float _timeLeft;
    private float _driftPoints;
    private float _baseReward;

    private CarController _carController;

    private void Start()
    {
        _timeLeft = levelDuration;
        _driftPoints = 0f;
        _levelOver = false;

        FindLocalPlayerCar();
    }
    
    private void FindLocalPlayerCar()
    {
        GameObject[] playerCars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject car in playerCars)
        {
            var photonView = car.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                _carController = car.GetComponent<CarController>();
                if (_carController != null)
                {
                    _carController.OnDriftEvent += HandleDriftEvent;
                }
                Debug.Log($"[GameUIManager] Found local CarController: {car.name}");
                return;
            }
        }
        Debug.LogWarning("[GameUIManager] Local player car not found!");
    }

    private void Update()
    {
        if (_levelOver)
        {
            CheckForDoubleReward();
            return;
        }

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            EndLevel();
        }

        if (_carController != null && _carController.IsDrifting)
        {
            float speedFactor = _carController.CurrentSpeedKmh;
            _driftPoints += speedFactor * 5f * Time.deltaTime;
        }

        UpdateHUD();
    }

    private void HandleDriftEvent(bool started, float duration)
    {
        if (started)
        {
            Debug.Log("[GameUIManager] Дрифт начался (локальный)!");
        }
        else
        {
            Debug.Log($"[GameUIManager] Дрифт закончился! Длился {duration:F2} с");
        }
    }

    private void EndLevel()
    {
        _levelOver = true;

        if (_carController != null)
        {
            _carController.CanDrive = false;
        }

        _baseReward = Mathf.Floor(_driftPoints / 10f);

        if (rewardLabel != null)
        {
            rewardLabel.text =
                $"Level Over!\n" +
                $"Drift Points: {_driftPoints:0}\n" +
                $"Reward: ${_baseReward}\n" +
                "Press [D] to DOUBLE reward (watch ad)";
        }
    }

    private void UpdateHUD()
    {
        if (timerLabel != null)
        {
            timerLabel.text = $"Time: {_timeLeft:0.0}";
        }

        if (speedLabel != null && _carController != null)
        {
            speedLabel.text = $"Speed: {_carController.CurrentSpeedKmh:0.0} km/h";
        }

        if (driftPointsLabel != null)
        {
            driftPointsLabel.text = $"Drift: {_driftPoints:0}";
        }
    }

    private void CheckForDoubleReward()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                Debug.Log("[GameUIManager] Showing Rewarded Ad...");
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("[GameUIManager] Rewarded Ad not available.");
                if (rewardLabel != null)
                {
                    rewardLabel.text = $"No ads available to double reward.";
                }
            }
        }
    }

    private void OnRewardedAdCompleted(IronSourcePlacement placement)
    {
        float doubledReward = _baseReward * 2f;
        if (rewardLabel != null)
        {
            rewardLabel.text =
                $"Reward Doubled!\nBase = ${_baseReward}\nDoubled = ${doubledReward}";
        }
        Debug.Log($"[GameUIManager] Rewarded Ad Completed. Reward: ${doubledReward}");
    }

    private void OnRewardedAdClosed()
    {
        Debug.Log("[GameUIManager] Rewarded Ad Closed.");
    }

    private void OnRewardedAdAvailabilityChanged(bool available)
    {
        Debug.Log($"[GameUIManager] Rewarded Ad Availability Changed: {available}");
    }
}
