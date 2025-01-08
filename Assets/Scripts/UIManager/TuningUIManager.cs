using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TuningUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuCarSpawner menuCarSpawner;
    [SerializeField] private MenuUIManager menuUIManager; // Для обновления денег

    [Header("Extra Parts UI")]
    [SerializeField] private Transform extraPartsContainer; // Родитель для кнопок дополнительных деталей
    [SerializeField] private Button extraPartButtonPrefab; // Префаб кнопки для каждой детали

    [Header("Paint UI")]
    [SerializeField] private Transform paintOptionsContainer; // Родитель для кнопок покраски
    [SerializeField] private Button paintOptionButtonPrefab; // Префаб кнопки для каждого цвета

    // Стоимость дополнительных деталей и покраски (можно сделать более динамичной)
    [SerializeField] private float extraPartCost = 500f;
    [SerializeField] private float paintCost = 300f;

    // Список доступных цветов
    [SerializeField] private List<string> availablePaintColors; // Формат HEX, например, "#FF5733"

    private CarStats currentCarStats;
    private PlayerStats playerStats;

    private void Start()
    {
        // Загрузка данных игрока и текущей машины
        playerStats = PlayerDataManager.LoadPlayerStats();
        currentCarStats = menuCarSpawner.GetCurrentCarStats();

        if (currentCarStats == null)
        {
            Debug.LogError("[TuningUIManager] currentCarStats is null. Ensure that MenuCarSpawner is correctly assigned and returns a valid CarStats.");
            return;
        }

        // Генерация UI для дополнительных деталей
        PopulateExtraPartsUI();

        // Генерация UI для покраски
        PopulatePaintOptionsUI();
    }


    private void OnEnable()
    {
        // Подписка на событие смены машины, если есть
        // Например, если MenuCarSpawner имеет событие OnCarChanged, можно подписаться
    }

    private void OnDisable()
    {
        // Отписка от событий
    }

    /// <summary>
    /// Генерация кнопок для дополнительных деталей
    /// </summary>
    private void PopulateExtraPartsUI()
    {
        foreach (string part in currentCarStats.AvailableExtraParts)
        {
            // Проверяем, активирована ли деталь
            bool isActive = currentCarStats.ActiveExtraParts.Contains(part);

            // Создаём кнопку
            Button partButton = Instantiate(extraPartButtonPrefab, extraPartsContainer);
            TMP_Text buttonText = partButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = isActive ? $"{part} (Активно)" : $"{part} (${extraPartCost})";

            // Деактивируем кнопку, если деталь уже активирована
            if (isActive)
            {
                partButton.interactable = false;
            }
            else
            {
                // Добавляем обработчик нажатия
                partButton.onClick.AddListener(() => OnExtraPartPurchase(part, partButton));
            }
        }
    }

    /// <summary>
    /// Обработчик покупки дополнительной детали
    /// </summary>
    private void OnExtraPartPurchase(string partName, Button button)
    {
        if (playerStats.Money >= extraPartCost)
        {
            // Уменьшаем деньги
            playerStats.AddMoney(-extraPartCost);
            PlayerDataManager.SavePlayerStats(playerStats);
            menuUIManager.UpdateMoneyText();

            // Активируем деталь
            currentCarStats.ActivateExtraPart(partName);
            CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

            // Обновляем кнопку
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = $"{partName} (Активно)";
            button.interactable = false;

            // Активируем деталь на спауне
            // Можно добавить событие или другой метод для обновления спауна машины

            Debug.Log($"[TuningUIManager] Purchased and activated {partName}");
        }
        else
        {
            Debug.LogWarning("[TuningUIManager] Not enough money to purchase extra part.");
            // Добавьте UI уведомление о нехватке денег
        }
    }

    /// <summary>
    /// Генерация кнопок для покраски
    /// </summary>
    private void PopulatePaintOptionsUI()
    {
        foreach (string colorHex in availablePaintColors)
        {
            // Создаём кнопку
            Button paintButton = Instantiate(paintOptionButtonPrefab, paintOptionsContainer);
            TMP_Text buttonText = paintButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = $"{colorHex} (${paintCost})";

            // Добавляем обработчик нажатия
            paintButton.onClick.AddListener(() => OnPaintPurchase(colorHex, paintButton));
        }
    }

    /// <summary>
    /// Обработчик покупки покраски
    /// </summary>
    private void OnPaintPurchase(string colorHex, Button button)
    {
        if (playerStats.Money >= paintCost)
        {
            // Уменьшаем деньги
            playerStats.AddMoney(-paintCost);
            PlayerDataManager.SavePlayerStats(playerStats);
            menuUIManager.UpdateMoneyText();

            // Устанавливаем цвет покраски
            currentCarStats.SetPaintColor(colorHex);
            CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

            // Применяем цвет к машине, если она уже спаунена
            GameObject currentCar = menuCarSpawner.GetCurrentCarInstance();
            if (currentCar != null)
            {
                ApplyPaintColor(currentCar, colorHex);
            }

            // Обновляем UI кнопки покраски (опционально)
            // Например, отметить выбранный цвет

            Debug.Log($"[TuningUIManager] Purchased and applied paint color {colorHex}");
        }
        else
        {
            Debug.LogWarning("[TuningUIManager] Not enough money to purchase paint.");
            // Добавьте UI уведомление о нехватке денег
        }
    }

    /// <summary>
    /// Применяет цвет покраски к машине
    /// </summary>
    private void ApplyPaintColor(GameObject carInstance, string colorHex)
    {
        Renderer[] renderers = carInstance.GetComponentsInChildren<Renderer>();
        Color newColor;
        if (ColorUtility.TryParseHtmlString(colorHex, out newColor))
        {
            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.color = newColor;
                }
            }
            Debug.Log($"[TuningUIManager] Applied color {colorHex} to car.");
        }
        else
        {
            Debug.LogError($"[TuningUIManager] Invalid color code: {colorHex}");
        }
    }
}
