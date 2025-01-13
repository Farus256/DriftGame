using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class CarControlsUIManager : MonoBehaviour
{
    private CarController carController;

    [SerializeField] private GameObject mobileUIRoot;

    // Кнопки управления
    [SerializeField] private GameObject accelerateButton;
    [SerializeField] private GameObject brakeButton;
    [SerializeField] private GameObject steerLeftButton;
    [SerializeField] private GameObject steerRightButton;
    [SerializeField] private GameObject handbrakeButton;
    [SerializeField] private Button flipCarButton;

    private void Awake()
    {
        // Активируем мобильный UI, если он назначен
        if (mobileUIRoot != null)
            mobileUIRoot.SetActive(true);
    }

    private void OnEnable()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Подписываемся на событие спавна локального автомобиля в мультиплеере
            MultiPlayerCarSpawner.OnLocalCarSpawned += HandleLocalCarSpawned;
        }
        else
        {
            // Подписываемся на событие спавна автомобиля в однопользовательском режиме
            GameCarSpawner.OnSinglePlayerCarSpawned += HandleSinglePlayerCarSpawned;
        }
    }

    private void OnDisable()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Отписываемся от события при отключении в мультиплеере
            MultiPlayerCarSpawner.OnLocalCarSpawned -= HandleLocalCarSpawned;
        }
        else
        {
            // Отписываемся от события при отключении в однопользовательском режиме
            GameCarSpawner.OnSinglePlayerCarSpawned -= HandleSinglePlayerCarSpawned;
        }
    }

    private void Start()
    {
        SetupButtonListeners();
    }

    /// <summary>
    /// Обработка спавна локального автомобиля в мультиплеере
    /// </summary>
    /// <param name="localCar">Заспавненный локальный автомобиль</param>
    private void HandleLocalCarSpawned(GameObject localCar)
    {
        AssignCarController(localCar);
    }

    /// <summary>
    /// Обработка спавна автомобиля в однопользовательском режиме
    /// </summary>
    /// <param name="car">Заспавненный автомобиль</param>
    private void HandleSinglePlayerCarSpawned(GameObject car)
    {
        AssignCarController(car);
    }

    /// <summary>
    /// Назначение CarController из переданного объекта автомобиля
    /// </summary>
    /// <param name="car">Объект автомобиля</param>
    private void AssignCarController(GameObject car)
    {
        carController = car.GetComponent<CarController>();
        if (carController == null)
        {
            Debug.LogError($"[CarControlsUIManager] CarController не найден на объекте: {car.name}");
            return;
        }

        Debug.Log($"[CarControlsUIManager] CarController назначен на: {car.name}");
    }

    /// <summary>
    /// Настройка слушателей для UI кнопок
    /// </summary>
    private void SetupButtonListeners()
    {
        if (accelerateButton != null)
        {
            AddEventTrigger(accelerateButton, EventTriggerType.PointerDown, OnAcceleratePressedDown);
            AddEventTrigger(accelerateButton, EventTriggerType.PointerUp, OnAcceleratePressedUp);
        }

        if (brakeButton != null)
        {
            AddEventTrigger(brakeButton, EventTriggerType.PointerDown, OnBrakePressedDown);
            AddEventTrigger(brakeButton, EventTriggerType.PointerUp, OnBrakePressedUp);
        }

        if (steerLeftButton != null)
        {
            AddEventTrigger(steerLeftButton, EventTriggerType.PointerDown, OnSteerLeftPressedDown);
            AddEventTrigger(steerLeftButton, EventTriggerType.PointerUp, OnSteerLeftPressedUp);
        }

        if (steerRightButton != null)
        {
            AddEventTrigger(steerRightButton, EventTriggerType.PointerDown, OnSteerRightPressedDown);
            AddEventTrigger(steerRightButton, EventTriggerType.PointerUp, OnSteerRightPressedUp);
        }

        if (handbrakeButton != null)
        {
            AddEventTrigger(handbrakeButton, EventTriggerType.PointerDown, OnHandbrakePressedDown);
            AddEventTrigger(handbrakeButton, EventTriggerType.PointerUp, OnHandbrakePressedUp);
        }

        if (flipCarButton != null)
        {
            flipCarButton.onClick.AddListener(OnFlipCarButton);
        }
    }

    /// <summary>
    /// Добавление EventTrigger к кнопке
    /// </summary>
    /// <param name="target">Кнопка</param>
    /// <param name="eventTriggerType">Тип события</param>
    /// <param name="action">Действие при событии</param>
    private void AddEventTrigger(GameObject target, EventTriggerType eventTriggerType, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = target.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventTriggerType
        };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    // ----- Газ -----
    private void OnAcceleratePressedDown()
    {
        if (carController == null) return;
        carController.acceleratePressed = true;
    }

    private void OnAcceleratePressedUp()
    {
        if (carController == null) return;
        carController.acceleratePressed = false;
    }

    // ----- Тормоз -----
    private void OnBrakePressedDown()
    {
        if (carController == null) return;
        carController.brakePressed = true;
    }

    private void OnBrakePressedUp()
    {
        if (carController == null) return;
        carController.brakePressed = false;
    }

    // ----- Поворот влево -----
    private void OnSteerLeftPressedDown()
    {
        if (carController == null) return;
        carController.steerLeftPressed = true;
    }

    private void OnSteerLeftPressedUp()
    {
        if (carController == null) return;
        carController.steerLeftPressed = false;
    }

    // ----- Поворот вправо -----
    private void OnSteerRightPressedDown()
    {
        if (carController == null) return;
        carController.steerRightPressed = true;
    }

    private void OnSteerRightPressedUp()
    {
        if (carController == null) return;
        carController.steerRightPressed = false;
    }

    // ----- Ручник -----
    private void OnHandbrakePressedDown()
    {
        if (carController == null) return;
        carController.handbrakePressed = true;
    }

    private void OnHandbrakePressedUp()
    {
        if (carController == null) return;
        carController.handbrakePressed = false;
    }

    // ----- Перевернуть машину -----
    private void OnFlipCarButton()
    {
        if (carController == null) return;
        carController.FlipCar();
    }
}