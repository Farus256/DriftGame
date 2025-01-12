using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        // При необходимости, вы можете оставить активацию UI 
        // или вовсе убрать этот метод, если UI не надо скрывать.
        if (mobileUIRoot != null)
            mobileUIRoot.SetActive(true);
    }

    private void Start()
    {
        // Находим объект игрока и его CarController
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            carController = player.GetComponent<CarController>();
            if (carController == null)
            {
                Debug.LogError("CarController not found on Player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }

        // Подключаем кнопки через код
        SetupButtonListeners();
    }

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
