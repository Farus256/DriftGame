using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Cost Settings")]
    private const float CostMultiplier = 0.2f; // Для апгрейдов

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

    [Header("Purchase UI")]
    [SerializeField] private Button buyCarButton;  // Кнопка «Купить машину»
    [SerializeField] private Button playButton;    // Кнопка «Play»

    [Header("Car Cost Text")]
    [SerializeField] private TMP_Text carCostText;
    
    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    private PlayerStats _playerStats;

    private void Awake()
    {
        // Проверка назначений
        if (menuCarSpawner == null)
        {
            Debug.LogError("[MenuUIManager] MenuCarSpawner is not assigned in the inspector!");
        }

        // Подписка на кнопки
        if (upgradeMotorPowerButton != null)
            upgradeMotorPowerButton.onClick.AddListener(OnUpgradeMotorPower);
        if (upgradeBrakeForceButton != null)
            upgradeBrakeForceButton.onClick.AddListener(OnUpgradeBrakeForce);

        if (buyCarButton != null)
            buyCarButton.onClick.AddListener(OnBuyCarButton);
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButton);
    }

    private void Start()
    {
        LoadPlayerStats();
        UpdateMoneyText();
        UpdateCarStatsUI();
    }

    private void OnDestroy()
    {
        if (upgradeMotorPowerButton != null)
            upgradeMotorPowerButton.onClick.RemoveListener(OnUpgradeMotorPower);
        if (upgradeBrakeForceButton != null)
            upgradeBrakeForceButton.onClick.RemoveListener(OnUpgradeBrakeForce);

        if (buyCarButton != null)
            buyCarButton.onClick.RemoveListener(OnBuyCarButton);
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButton);
    }
    
    private void OnEnable()
    {
        GlobalEventManager.onCarUpdated.AddListener(UpdateCarStatsUI);
    }

    private void OnDisable()
    {
        GlobalEventManager.onCarUpdated.RemoveListener(UpdateCarStatsUI);
    }
    
    private void LoadPlayerStats()
    {
        _playerStats = PlayerDataManager.LoadPlayerStats();
    }

    public void UpdateMoneyText()
    {
        if (_playerStats != null)
        {
            moneyText.text = $"Money: ${_playerStats.Money:F2}";
        }
    }

    public void UpdateCarStatsUI()
    {
        if (_playerStats == null)
        {
            Debug.LogWarning("[MenuUIManager] PlayerStats not loaded!");
            return;
        }

        // Берём ID выбранного автомобиля из CarSelection
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0)
        {
            Debug.LogWarning("[MenuUIManager] No car selected!");
            ClearCarStatsUI();
            return;
        }

        // Получаем CarStats по ID
        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCar == null)
        {
            Debug.LogWarning("[MenuUIManager] currentCar is null!");
            ClearCarStatsUI();
            return;
        }

        carNameLabel.text   = $"Car: {currentCar.CarName}";
        massText.text       = $"Mass: {currentCar.Mass} kg";
        motorPowerText.text = $"Motor Power: {currentCar.MotorPower} HP";
        brakeForceText.text = $"Brake Force: {currentCar.BrakeForce} N";

        float motorUpgradeCost = CalculateUpgradeCost(currentCar.MotorPower);
        float brakeUpgradeCost = CalculateUpgradeCost(currentCar.BrakeForce);

        motorPowerUpgradeCostText.text = $"Upgrade Motor: ${motorUpgradeCost:F2}";
        brakeForceUpgradeCostText.text = $"Upgrade Brakes: ${brakeUpgradeCost:F2}";

        // Отображаем стоимость машины
        if (carCostText != null)
        {
            carCostText.text = $"Cost: ${currentCar.Cost:F2}";
        }

        // Проверяем, куплена ли машина
        bool isCarOwned = _playerStats.IsCarPurchased(currentCar.ID);

        if (buyCarButton != null)
        {
            buyCarButton.gameObject.SetActive(!isCarOwned);
        }

        if (upgradeMotorPowerButton != null)
            upgradeMotorPowerButton.interactable = isCarOwned;
        if (upgradeBrakeForceButton != null)
            upgradeBrakeForceButton.interactable = isCarOwned;
        if (playButton != null)
            playButton.interactable = isCarOwned;
    }

    private void ClearCarStatsUI()
    {
        carNameLabel.text               = "Car: -";
        massText.text                   = "Mass: - kg";
        motorPowerText.text             = "Motor Power: - HP";
        brakeForceText.text             = "Brake Force: - N";
        motorPowerUpgradeCostText.text  = "Upgrade Motor: -";
        brakeForceUpgradeCostText.text  = "Upgrade Brakes: -";

        if (carCostText != null)
            carCostText.text = "Cost: -";

        if (buyCarButton != null)
            buyCarButton.gameObject.SetActive(false);

        if (upgradeMotorPowerButton != null)
            upgradeMotorPowerButton.interactable = false;
        if (upgradeBrakeForceButton != null)
            upgradeBrakeForceButton.interactable = false;
        if (playButton != null)
            playButton.interactable = false;
    }

    private float CalculateUpgradeCost(float currentValue)
    {
        return CostMultiplier * currentValue;
    }

    private void OnBuyCarButton()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0) return;

        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCar == null) return;

        if (_playerStats.IsCarPurchased(currentCar.ID))
        {
            Debug.LogWarning("[MenuUIManager] Car already purchased!");
            return;
        }

        if (_playerStats.Money < currentCar.Cost)
        {
            Debug.LogWarning("[MenuUIManager] Not enough money to buy this car!");
            return;
        }

        _playerStats.AddMoney(-currentCar.Cost);
        _playerStats.PurchaseCar(currentCar.ID);

        PlayerDataManager.SavePlayerStats(_playerStats);

        UpdateMoneyText();
        UpdateCarStatsUI();

        Debug.Log($"[MenuUIManager] Car '{currentCar.CarName}' purchased for ${currentCar.Cost}. Remaining money: {_playerStats.Money}");
    }

    private void OnUpgradeMotorPower()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0) return;

        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCar == null) return;

        float upgradeAmount = 100f;
        float upgradeCost   = CalculateUpgradeCost(currentCar.MotorPower);

        if (_playerStats.Money < upgradeCost)
        {
            Debug.LogWarning("[MenuUIManager] Not enough money for Motor Power upgrade!");
            return;
        }

        _playerStats.AddMoney(-upgradeCost);
        PlayerDataManager.SavePlayerStats(_playerStats);
        UpdateMoneyText();

        currentCar.UpgradeMotorPower(upgradeAmount);
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());
        UpdateCarStatsUI();

        Debug.Log($"[MenuUIManager] Upgraded Motor Power by {upgradeAmount}. Cost: {upgradeCost}. Remaining: {_playerStats.Money}");
    }

    private void OnUpgradeBrakeForce()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0) return;

        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCar == null) return;

        float upgradeAmount = 100f;
        float upgradeCost   = CalculateUpgradeCost(currentCar.BrakeForce);

        if (_playerStats.Money < upgradeCost)
        {
            Debug.LogWarning("[MenuUIManager] Not enough money for Brake Force upgrade!");
            return;
        }

        _playerStats.AddMoney(-upgradeCost);
        PlayerDataManager.SavePlayerStats(_playerStats);
        UpdateMoneyText();

        currentCar.UpgradeBrakeForce(upgradeAmount);
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());
        UpdateCarStatsUI();

        Debug.Log($"[MenuUIManager] Upgraded Brake Force by {upgradeAmount}. Cost: {upgradeCost}. Remaining: {_playerStats.Money}");
    }

    public void OnNextCarButton()
    {
        // Переключаемся на следующую машину
        menuCarSpawner.ShowNextCar();

        // После переключения получаем новый CarStats и сохраняем в CarSelection
        CarStats newCar = menuCarSpawner.GetCurrentCarStats();
        if (newCar != null)
        {
            CarSelection.SelectCar(newCar.ID);
        }

        UpdateCarStatsUI();
    }

    public void OnPreviousCarButton()
    {
        menuCarSpawner.ShowPreviousCar();

        // После переключения получаем новый CarStats и сохраняем в CarSelection
        CarStats newCar = menuCarSpawner.GetCurrentCarStats();
        if (newCar != null)
        {
            CarSelection.SelectCar(newCar.ID);
        }

        UpdateCarStatsUI();
    }

    public void OnSettingsButton()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        UpdateCarStatsUI();
    }

    private void OnPlayButton()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0)
        {
            Debug.LogWarning("[MenuUIManager] No car selected!");
            return;
        }

        CarStats chosenCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (chosenCar == null)
        {
            Debug.LogWarning("[MenuUIManager] Invalid car ID!");
            return;
        }

        if (!_playerStats.IsCarPurchased(chosenCar.ID))
        {
            Debug.LogWarning("[MenuUIManager] You cannot play with a car you haven't purchased!");
            return;
        }

        // Запоминаем выбранный автомобиль (если нужно в других сценах)
        CarSelection.SelectCar(chosenCar.ID);

        // Можно вызвать начало уровня
        GlobalEventManager.TriggerLevelStart();
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
