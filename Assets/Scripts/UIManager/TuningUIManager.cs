using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TuningUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuCarSpawner menuCarSpawner;
    [SerializeField] private MenuUIManager menuUIManager; // Для обновления UI (например, отображения денег)

    [Header("Buttons")]
    [SerializeField] private Button spoilerButton;
    [SerializeField] private Button sideSkirtsButton;
    [SerializeField] private Button paintRedButton;
    [SerializeField] private Button paintBlueButton;
    [SerializeField] private Button paintStandardButton; 
    [SerializeField] private Button paintDefaultButton;   // Новая кнопка для дефолтного цвета

    // Стоимость дополнительных деталей и покраски
    [SerializeField] private float extraPartCost = 500f;
    [SerializeField] private float paintCost = 300f;

    private const string STANDARD_COLOR_HEX = "#FFFFFF";
    private const string DEFAULT_COLOR_HEX  = "#CCCCCC";

    private PlayerStats playerStats;

    private void Start()
    {
        Debug.Log("[TuningUIManager] Start called.");

        // Загружаем данные игрока
        playerStats = PlayerDataManager.LoadPlayerStats();
        if (playerStats == null)
        {
            Debug.LogError("[TuningUIManager] Failed to load PlayerStats.");
            return;
        }

        RefreshUI();
    }

    private void OnEnable()
    {
        // Подписываемся на глобальное событие
        GlobalEventManager.onCarUpdated.AddListener(RefreshUI);
    }

    private void OnDisable()
    {
        // Отписываемся
        GlobalEventManager.onCarUpdated.RemoveListener(RefreshUI);
    }

    /// <summary>
    /// Обновляет UI при изменении данных автомобиля или переключении.
    /// </summary>
    private void RefreshUI()
    {
        Debug.Log("[TuningUIManager] RefreshUI called.");

        // Пересчитываем PlayerStats
        playerStats = PlayerDataManager.LoadPlayerStats();
        if (playerStats == null)
        {
            Debug.LogError("[TuningUIManager] Failed to reload PlayerStats in RefreshUI.");
            return;
        }

        // Получаем текущий ID выбранного авто
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0)
        {
            Debug.LogWarning("[TuningUIManager] No car selected in CarSelection.");
            // Можно тут же отключить все кнопки, если надо
            return;
        }

        // Берём актуальные данные об автомобиле
        CarStats currentCarStats = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCarStats == null)
        {
            Debug.LogWarning("[TuningUIManager] currentCarStats is null (invalid ID?).");
            return;
        }

        // Кнопки для доп. деталей
        UpdateExtraPartButton(spoilerButton,    "Spoiler",    currentCarStats);
        UpdateExtraPartButton(sideSkirtsButton, "SideSkirts", currentCarStats);

        // Кнопки для покрасок
        UpdatePaintButton(paintRedButton,      "Red",      "#FF0000",    isFree: false, currentCarStats);
        UpdatePaintButton(paintBlueButton,     "Blue",     "#0000FF",    isFree: false, currentCarStats);
        UpdatePaintButton(paintStandardButton, "Standard", STANDARD_COLOR_HEX, isFree: true,  currentCarStats);
        UpdatePaintButton(paintDefaultButton,  "Default",  DEFAULT_COLOR_HEX,  isFree: true,  currentCarStats);
    }

    private void UpdateExtraPartButton(Button button, string partName, CarStats carStats)
    {
        if (button == null)
        {
            Debug.LogError($"[TuningUIManager] Button for '{partName}' is not assigned.");
            return;
        }

        // Если машина не куплена
        if (!playerStats.IsCarPurchased(carStats.ID))
        {
            button.interactable = false;
            var lockedText = button.GetComponentInChildren<TMP_Text>();
            if (lockedText != null)
                lockedText.text = $"{partName} (Locked)";
            return;
        }

        bool isActive = carStats.ActiveExtraParts.Contains(partName);
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (!buttonText)
        {
            Debug.LogError($"[TuningUIManager] TMP_Text component not found on button for '{partName}'.");
            return;
        }

        buttonText.text     = isActive ? $"{partName} (Installed)" : $"{partName} (${extraPartCost})";
        button.interactable = true; 
    }

    private void UpdatePaintButton(Button button, string colorName, string colorHex, bool isFree, CarStats carStats)
    {
        if (button == null)
        {
            Debug.LogError($"[TuningUIManager] Paint Button for '{colorName}' is not assigned.");
            return;
        }

        // Если машина не куплена
        if (!playerStats.IsCarPurchased(carStats.ID))
        {
            button.interactable = false;
            var lockedText = button.GetComponentInChildren<TMP_Text>();
            if (lockedText != null)
                lockedText.text = $"{colorName} (Locked)";
            return;
        }

        bool isApplied = carStats.PaintColor == colorHex;
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (!buttonText)
        {
            Debug.LogError($"[TuningUIManager] TMP_Text component not found on paint button for '{colorName}'.");
            return;
        }

        if (isFree)
        {
            buttonText.text = isApplied ? $"{colorName} (Applied)" : $"{colorName} (Free)";
        }
        else
        {
            buttonText.text = isApplied ? $"{colorName} (Applied)" : $"{colorName} (${paintCost})";
        }

        // Если цвет уже применён, кнопку отключаем
        button.interactable = !isApplied;
    }

    // ================================
    // Методы для переключения деталей
    // ================================
    public void OnButtonSpoilerToggle()
    {
        ToggleExtraPart("Spoiler");
    }

    public void OnButtonSideSkirtsToggle()
    {
        ToggleExtraPart("SideSkirts");
    }

    private void ToggleExtraPart(string partName)
    {
        Debug.Log($"[TuningUIManager] ToggleExtraPart called for: {partName}");

        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0)
        {
            Debug.LogWarning("[TuningUIManager] No car selected to toggle part.");
            return;
        }

        CarStats carStats = CarDataManager.GetCarStatsById(selectedCarId);
        if (carStats == null)
        {
            Debug.LogError("[TuningUIManager] CarStats is null. Cannot toggle part.");
            return;
        }

        if (!playerStats.IsCarPurchased(carStats.ID))
        {
            Debug.LogWarning("[TuningUIManager] Cannot toggle part on an unpurchased car.");
            return;
        }

        if (!carStats.AvailableExtraParts.Contains(partName))
        {
            Debug.LogWarning($"[TuningUIManager] Part '{partName}' is not available for this car.");
            return;
        }

        bool isActive = carStats.ActiveExtraParts.Contains(partName);
        if (isActive)
        {
            // Деактивируем деталь
            carStats.DeactivateExtraPart(partName);
            Debug.Log($"[TuningUIManager] Deactivated {partName}");
        }
        else
        {
            // Покупаем деталь
            if (playerStats.Money >= extraPartCost)
            {
                playerStats.AddMoney(-extraPartCost);
                PlayerDataManager.SavePlayerStats(playerStats);

                // Обновляем текст денег
                menuUIManager.UpdateMoneyText();

                // Активируем деталь
                carStats.ActivateExtraPart(partName);
                CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

                Debug.Log($"[TuningUIManager] Purchased and activated {partName}");
            }
            else
            {
                Debug.LogWarning("[TuningUIManager] Not enough money to purchase extra part.");
                // Здесь можно вызвать всплывающее сообщение о нехватке денег
                return;
            }
        }

        // Сообщаем всем, что машина обновилась
        GlobalEventManager.TriggerCarUpdated();
    }

    // ===========================
    // Методы для покраски
    // ===========================
    public void OnButtonPaintRed()
    {
        ApplyPaint("#FF0000", false);
    }

    public void OnButtonPaintBlue()
    {
        ApplyPaint("#0000FF", false);
    }

    public void OnButtonPaintStandard()
    {
        ApplyPaint(STANDARD_COLOR_HEX, true);
    }

    public void OnButtonPaintDefault()
    {
        ApplyPaint(DEFAULT_COLOR_HEX, true);
    }

    private void ApplyPaint(string colorHex, bool isFree)
    {
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0)
        {
            Debug.LogWarning("[TuningUIManager] No car selected to paint.");
            return;
        }

        CarStats carStats = CarDataManager.GetCarStatsById(selectedCarId);
        if (carStats == null)
        {
            Debug.LogError("[TuningUIManager] CarStats is null. Cannot apply paint.");
            return;
        }

        if (!playerStats.IsCarPurchased(carStats.ID))
        {
            Debug.LogWarning("[TuningUIManager] Cannot paint an unpurchased car!");
            return;
        }

        if (carStats.PaintColor == colorHex)
        {
            Debug.LogWarning($"[TuningUIManager] Color '{colorHex}' is already applied.");
            return;
        }

        if (!isFree)
        {
            if (playerStats.Money < paintCost)
            {
                Debug.LogWarning("[TuningUIManager] Not enough money to purchase paint.");
                return;
            }

            playerStats.AddMoney(-paintCost);
            PlayerDataManager.SavePlayerStats(playerStats);
            menuUIManager.UpdateMoneyText();
        }

        // Устанавливаем цвет покраски
        carStats.SetPaintColor(colorHex);
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

        // Применяем цвет к текущему экземпляру (если он в сцене)
        GameObject currentCar = menuCarSpawner.GetCurrentCarInstance();
        if (currentCar != null)
        {
            ApplyPaintColorToCar(currentCar, colorHex);
        }

        // Сообщаем всем, что машина обновилась
        GlobalEventManager.TriggerCarUpdated();

        Debug.Log(isFree
            ? $"[TuningUIManager] Applied paint color {colorHex} for free"
            : $"[TuningUIManager] Purchased and applied paint color {colorHex}");
    }

    private void ApplyPaintColorToCar(GameObject carInstance, string colorHex)
    {
        if (ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
        {
            Renderer[] renderers = carInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].color = newColor;
                }
            }
            Debug.Log($"[TuningUIManager] Applied color {colorHex} to car instance.");
        }
        else
        {
            Debug.LogError($"[TuningUIManager] Invalid color code: {colorHex}");
        }
    }
}
