using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance { get; private set; }
    
    private const float CostMultiplier = 0.2f;

    [SerializeField] private MenuCarSpawner menuCarSpawner;
    
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text carNameLabel;
    [SerializeField] private TMP_Text massText;
    [SerializeField] private TMP_Text motorPowerText;
    [SerializeField] private TMP_Text brakeForceText;
    [SerializeField] private TMP_Text carCostText;
    [SerializeField] private TMP_Text motorPowerUpgradeCostText;
    [SerializeField] private TMP_Text brakeForceUpgradeCostText;
    
    [SerializeField] private Button upgradeMotorPowerButton;
    [SerializeField] private Button upgradeBrakeForceButton;
    [SerializeField] private Button buyCarButton;
    [SerializeField] private Button playButton;
   
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject levelPanel;

    private PlayerStats playerStats;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (upgradeMotorPowerButton)
            upgradeMotorPowerButton.onClick.AddListener(OnUpgradeMotorPower);
        if (upgradeBrakeForceButton)
            upgradeBrakeForceButton.onClick.AddListener(OnUpgradeBrakeForce);
        if (buyCarButton)
            buyCarButton.onClick.AddListener(OnBuyCarButton);
    }

    private void Start()
    {
        LoadPlayerStats();
        UpdateMoneyText();
        UpdateGoldText();
        UpdateCarStatsUI();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnMoneyChanged -= UpdateMoneyText;
            playerStats.OnGoldChanged  -= UpdateGoldText;
        }
        
        if (upgradeMotorPowerButton)
            upgradeMotorPowerButton.onClick.RemoveListener(OnUpgradeMotorPower);
        if (upgradeBrakeForceButton)
            upgradeBrakeForceButton.onClick.RemoveListener(OnUpgradeBrakeForce);
        if (buyCarButton)
            buyCarButton.onClick.RemoveListener(OnBuyCarButton);
        if (playButton)
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
        playerStats = PlayerDataManager.LoadPlayerStats();
        if (playerStats != null)
        {
            playerStats.OnMoneyChanged += UpdateMoneyText;
            playerStats.OnGoldChanged  += UpdateGoldText;
        }
    }
    
    public void UpdateMoneyText(float currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: ${currentMoney:F2}";
        }
    }
    
    public void UpdateMoneyText()
    {
        if (playerStats != null) moneyText.text = $"Money: ${playerStats.Money:F2}";
    }
    
    public void UpdateGoldText(float currentGold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {currentGold:F2}";
        }
    }

    public void UpdateGoldText()
    {
        if (playerStats != null && goldText != null)
            goldText.text = $"Gold: {playerStats.Gold:F2}";
    }
    
    public void UpdateCarStatsUI()
    {
        if (playerStats == null) return;
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0)
        {
            ClearCarStatsUI();
            return;
        }
        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCar == null)
        {
            ClearCarStatsUI();
            return;
        }
        carNameLabel.text = $"Car: {currentCar.CarName}";
        massText.text = $"Mass: {currentCar.Mass} kg";
        motorPowerText.text = $"Motor Power: {currentCar.MotorPower}";
        brakeForceText.text = $"Brake Force: {currentCar.BrakeForce}";
        
        float motorUpgradeCost = CalculateUpgradeCost(currentCar.MotorPower);
        float brakeUpgradeCost = CalculateUpgradeCost(currentCar.BrakeForce);
        
        motorPowerUpgradeCostText.text = $"Upgrade Motor: ${motorUpgradeCost:F2}";
        
        brakeForceUpgradeCostText.text = $"Upgrade Brakes: ${brakeUpgradeCost:F2}";
        
        if (carCostText) carCostText.text = $"Cost: ${currentCar.Cost:F2}";
        
        bool isCarOwned = playerStats.IsCarPurchased(currentCar.ID);
        
        if (buyCarButton) buyCarButton.gameObject.SetActive(!isCarOwned);
        if (upgradeMotorPowerButton) upgradeMotorPowerButton.interactable = isCarOwned;
        if (upgradeBrakeForceButton) upgradeBrakeForceButton.interactable = isCarOwned;
        if (playButton) playButton.interactable = isCarOwned;
    }

    private void ClearCarStatsUI()
    {
        carNameLabel.text = "Car: -";
        massText.text = "Mass: -";
        motorPowerText.text = "Motor Power: -";
        brakeForceText.text = "Brake Force: -";
        motorPowerUpgradeCostText.text = "Upgrade Motor: -";
        brakeForceUpgradeCostText.text = "Upgrade Brakes: -";
        
        if (carCostText) carCostText.text = "Cost: -";
        if (buyCarButton) buyCarButton.gameObject.SetActive(false);
        if (upgradeMotorPowerButton) upgradeMotorPowerButton.interactable = false;
        if (upgradeBrakeForceButton) upgradeBrakeForceButton.interactable = false;
        if (playButton) playButton.interactable = false;
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
        if (playerStats.IsCarPurchased(currentCar.ID)) return;
        if (playerStats.Money < currentCar.Cost) return;
        
        playerStats.AddMoney(-currentCar.Cost);
        playerStats.PurchaseCar(currentCar.ID);
        
        PlayerDataManager.SavePlayerStats(playerStats);
        UpdateMoneyText();
        UpdateGoldText();
        UpdateCarStatsUI();
        TuningUIManager.Instance.RefreshUI();
    }

    private void OnUpgradeMotorPower()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        
        if (selectedCarId < 0) return;
        
        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        
        if (currentCar == null) return;
        float upgradeAmount = 100f;
        float upgradeCost = CalculateUpgradeCost(currentCar.MotorPower);
        
        if (playerStats.Money < upgradeCost) return;
        playerStats.AddMoney(-upgradeCost);
        
        PlayerDataManager.SavePlayerStats(playerStats);
        
        UpdateMoneyText();
        UpdateGoldText();
        currentCar.UpgradeMotorPower(upgradeAmount);
        
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());
        UpdateCarStatsUI();
    }

    private void OnUpgradeBrakeForce()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        
        if (selectedCarId < 0) return;
        
        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        
        if (currentCar == null) return;
        float upgradeAmount = 100f;
        float upgradeCost = CalculateUpgradeCost(currentCar.BrakeForce);
        
        if (playerStats.Money < upgradeCost) return;
        playerStats.AddMoney(-upgradeCost);
        
        PlayerDataManager.SavePlayerStats(playerStats);
        
        UpdateMoneyText();
        currentCar.UpgradeBrakeForce(upgradeAmount);
        
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());
        UpdateCarStatsUI();
    }

    public void OnNextCarButton()
    {
        menuCarSpawner.ShowNextCar();
        CarStats newCar = menuCarSpawner.GetCurrentCarStats();
        if (newCar != null) CarSelection.SelectCar(newCar.ID);
        UpdateCarStatsUI();
    }

    public void OnPreviousCarButton()
    {
        menuCarSpawner.ShowPreviousCar();
        CarStats newCar = menuCarSpawner.GetCurrentCarStats();
        if (newCar != null) CarSelection.SelectCar(newCar.ID);
        UpdateCarStatsUI();
    }

    public void OnSettingsButton()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        UpdateCarStatsUI();
    }

    public void OnPlayButton()
    {
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0) return;
        
        CarStats chosenCar = CarDataManager.GetCarStatsById(selectedCarId);
        
        if (chosenCar == null) return;
        if (!playerStats.IsCarPurchased(chosenCar.ID)) return;
        
        CarSelection.SelectCar(chosenCar.ID);

        levelPanel.SetActive(!levelPanel.activeSelf);

        // GlobalEventManager.TriggerLevelStart();
    }

    public void OnShopButton()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }

    public void OnLevelCloseButton()
    {
        levelPanel.SetActive(!levelPanel.activeSelf);
    }
    public void OnExitButton()
    {
        Application.Quit();
    }
}
