using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("CostSettings")]
    [SerializeField] private const float CostMultiplier = 0.2f;
    
    [Header("References")]
    [SerializeField] private MenuCarSpawner menuCarSpawner;
    [SerializeField] private TMP_Text moneyText;

    [Header("Car Stats UI")]
    [SerializeField] private TMP_Text carNameLabel;
    [SerializeField] private TMP_Text massText;
    [SerializeField] private TMP_Text motorPowerText;
    [SerializeField] private TMP_Text brakeForceText;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button upgradeMotorPowerButton;
    [SerializeField] private Button upgradeBrakeForceButton;

    [Header("Upgrade Cost Texts")]
    [SerializeField] private TMP_Text motorPowerUpgradeCostText;
    [SerializeField] private TMP_Text brakeForceUpgradeCostText;

    private PlayerStats _playerStats;

    private void Awake()
    {
        // Убедимся, что MenuCarSpawner назначен
        if (menuCarSpawner == null)
        {
            Debug.LogError("[MenuUIManager] MenuCarSpawner is not assigned in the inspector!");
        }

        // Настройка слушателей кнопок
        if (upgradeMotorPowerButton != null)
            upgradeMotorPowerButton.onClick.AddListener(OnUpgradeMotorPower);

        if (upgradeBrakeForceButton != null)
            upgradeBrakeForceButton.onClick.AddListener(OnUpgradeBrakeForce);
    }

    private void Start()
    {
        LoadPlayerStats();
        UpdateMoneyText();
        UpdateCarStatsUI();
    }

    private void OnDestroy()
    {
        // Удаление слушателей, чтобы избежать утечек
        if (upgradeMotorPowerButton != null)
            upgradeMotorPowerButton.onClick.RemoveListener(OnUpgradeMotorPower);

        if (upgradeBrakeForceButton != null)
            upgradeBrakeForceButton.onClick.RemoveListener(OnUpgradeBrakeForce);
    }

    /// <summary>
    /// Загружает данные игрока
    /// </summary>
    private void LoadPlayerStats()
    {
        _playerStats = PlayerDataManager.LoadPlayerStats();
    }

    /// <summary>
    /// Обновляет отображение денег в UI
    /// </summary>
    public void UpdateMoneyText()
    {
        if (_playerStats != null)
        {
            moneyText.text = $"Money: ${_playerStats.Money:F2}";
        }
    }

    /// <summary>
    /// Обновляет отображение характеристик текущей машины
    /// </summary>
    public void UpdateCarStatsUI()
    {
        CarStats currentCar = menuCarSpawner.GetCurrentCarStats();
        if (currentCar == null)
        {
            Debug.LogWarning("[MenuUIManager] No car selected!");
            ClearCarStatsUI();
            return;
        }

        // Обновление текстовых полей
        carNameLabel.text = $"Car: {currentCar.CarName}";
        massText.text = $"Mass: {currentCar.Mass} kg";
        motorPowerText.text = $"Motor Power: {currentCar.MotorPower} HP";
        brakeForceText.text = $"Brake Force: {currentCar.BrakeForce} N";

        // Обновление стоимости апгрейдов
        motorPowerUpgradeCostText.text = $"Upgrade Motor Power: ${CalculateUpgradeCost(currentCar.MotorPower)}";
        brakeForceUpgradeCostText.text = $"Upgrade Brake Force: ${CalculateUpgradeCost(currentCar.BrakeForce)}";
    }

    /// <summary>
    /// Очищает UI отображения характеристик машины
    /// </summary>
    private void ClearCarStatsUI()
    {
        carNameLabel.text = "Car: -";
        massText.text = "Mass: - kg";
        motorPowerText.text = "Motor Power: - HP";
        brakeForceText.text = "Brake Force: - N";

        motorPowerUpgradeCostText.text = "Upgrade Motor Power: -";
        brakeForceUpgradeCostText.text = "Upgrade Brake Force: -";
    }

    /// <summary>
    /// Рассчитывает стоимость апгрейда на основе текущего значения характеристики
    /// </summary>
    private float CalculateUpgradeCost(float currentValue)
    {
        
        return CostMultiplier * currentValue;
    }

    /// <summary>
    /// Обработчик нажатия кнопки апгрейда мощности двигателя
    /// </summary>
    private void OnUpgradeMotorPower()
    {
        CarStats currentCar = menuCarSpawner.GetCurrentCarStats();
        if (currentCar == null)
        {
            Debug.LogError("[MenuUIManager] No car stats available!");
            return;
        }

        float upgradeAmount = 100f; // Примерное увеличение
        float upgradeCost = CalculateUpgradeCost(currentCar.MotorPower);

        if (_playerStats.Money < upgradeCost)
        {
            Debug.LogWarning("[MenuUIManager] Not enough money for Motor Power upgrade!");
            // Добавьте UI уведомление о нехватке денег
            return;
        }

        // Уменьшаем деньги
        _playerStats.AddMoney(-upgradeCost);
        PlayerDataManager.SavePlayerStats(_playerStats);
        UpdateMoneyText();

        // Увеличиваем мощность
        currentCar.UpgradeMotorPower(upgradeAmount);

        // Сохраняем обновленные car stats
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

        // Обновляем UI
        UpdateCarStatsUI();

        Debug.Log($"[MenuUIManager] Upgraded Motor Power by {upgradeAmount}. Cost: {upgradeCost}. Remaining Money: {_playerStats.Money}");
    }

    /// <summary>
    /// Обработчик нажатия кнопки апгрейда тормозной силы
    /// </summary>
    private void OnUpgradeBrakeForce()
    {
        CarStats currentCar = menuCarSpawner.GetCurrentCarStats();
        if (currentCar == null)
        {
            Debug.LogError("[MenuUIManager] No car stats available!");
            return;
        }

        float upgradeAmount = 100f; // Примерное увеличение
        float upgradeCost = CalculateUpgradeCost(currentCar.BrakeForce);

        if (_playerStats.Money < upgradeCost)
        {
            Debug.LogWarning("[MenuUIManager] Not enough money for Brake Force upgrade!");
            // Добавьте UI уведомление о нехватке денег
            return;
        }

        // Уменьшаем деньги
        _playerStats.AddMoney(-upgradeCost);
        PlayerDataManager.SavePlayerStats(_playerStats);
        UpdateMoneyText();

        // Увеличиваем тормозную силу
        currentCar.UpgradeBrakeForce(upgradeAmount);

        // Сохраняем обновленные car stats
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

        // Обновляем UI
        UpdateCarStatsUI();

        Debug.Log($"[MenuUIManager] Upgraded Brake Force by {upgradeAmount}. Cost: {upgradeCost}. Remaining Money: {_playerStats.Money}");
    }

    /// <summary>
    /// Обработчик переключения машины (следующая)
    /// </summary>
    public void OnNextCarButton()
    {
        menuCarSpawner.ShowNextCar();
        UpdateCarStatsUI();
    }

    /// <summary>
    /// Обработчик переключения машины (предыдущая)
    /// </summary>
    public void OnPreviousCarButton()
    {
        menuCarSpawner.ShowPreviousCar();
        UpdateCarStatsUI();
    }

    /// <summary>
    /// Обработчик кнопки Play
    /// </summary>
    public void OnPlayButton()
    {
        CarStats chosenCar = menuCarSpawner.GetCurrentCarStats();

        if (chosenCar == null)
        {
            Debug.LogWarning("[MenuUIManager] No car selected!");
            return;
        }

        CarSelection.SelectedCarId = chosenCar.ID;

        GlobalEventManager.TriggerLevelStart();
    }

    /// <summary>
    /// Обработчик кнопки Exit
    /// </summary>
    public void OnExitButton()
    {
        Application.Quit();
    }
}
