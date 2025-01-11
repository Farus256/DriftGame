using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TuningUIManager : MonoBehaviour
{
    public static TuningUIManager Instance { get; private set; }

    [SerializeField] private MenuCarSpawner menuCarSpawner;
    [SerializeField] private MenuUIManager menuUIManager;
    [SerializeField] private Button spoilerButton;
    [SerializeField] private Button sideSkirtsButton;
    [SerializeField] private Button paintRedButton;
    [SerializeField] private Button paintBlueButton;
    [SerializeField] private Button paintStandardButton;
    [SerializeField] private Button paintDefaultButton;

    [SerializeField] private float extraPartCost = 500f;
    [SerializeField] private float paintCost = 300f;

    private const string STANDARD_COLOR_HEX = "#FFFFFF";
    private const string DEFAULT_COLOR_HEX = "#CCCCCC";
    private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerStats = PlayerDataManager.LoadPlayerStats();
        RefreshUI();
    }

    private void OnEnable()
    {
        GlobalEventManager.onCarUpdated.AddListener(RefreshUI);
    }

    private void OnDisable()
    {
        GlobalEventManager.onCarUpdated.RemoveListener(RefreshUI);
    }

    // Сделали метод публичным, чтобы MenuCarSpawner (или любой другой скрипт)
    // мог вызвать RefreshUI() при переключении машины.
    public void RefreshUI()
    {
        playerStats = PlayerDataManager.LoadPlayerStats();
        if (playerStats == null) return;

        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0) return;

        CarStats currentCar = CarDataManager.GetCarStatsById(selectedCarId);
        if (currentCar == null) return;

        UpdateExtraPartButton(spoilerButton,    "Spoiler",    currentCar);
        UpdateExtraPartButton(sideSkirtsButton, "SideSkirts", currentCar);

        UpdatePaintButton(paintRedButton,      "Red",      "#FF0000",     false, currentCar);
        UpdatePaintButton(paintBlueButton,     "Blue",     "#0000FF",     false, currentCar);
        UpdatePaintButton(paintStandardButton, "Standard", STANDARD_COLOR_HEX, true,  currentCar);
        UpdatePaintButton(paintDefaultButton,  "Default",  DEFAULT_COLOR_HEX,  true,  currentCar);
    }

    private void UpdateExtraPartButton(Button button, string partName, CarStats carStats)
    {
        if (!button) return;

        // Если машина не куплена
        if (!playerStats.IsCarPurchased(carStats.ID))
        {
            button.interactable = false;
            var txt = button.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = $"{partName} (Locked)";
            return;
        }

        bool isActive = carStats.ActiveExtraParts.Contains(partName);
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText)
        {
            buttonText.text = isActive
                ? $"{partName} (Installed)"
                : $"{partName} (${extraPartCost})";
        }
        button.interactable = true;
    }

    private void UpdatePaintButton(Button button, string colorName, string colorHex, bool isFree, CarStats carStats)
    {
        if (!button) return;

        if (!playerStats.IsCarPurchased(carStats.ID))
        {
            button.interactable = false;
            var txt = button.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = $"{colorName} (Locked)";
            return;
        }

        bool isApplied = carStats.PaintColor == colorHex;
        TMP_Text bt = button.GetComponentInChildren<TMP_Text>();
        if (bt)
        {
            if (isFree)
                bt.text = isApplied
                    ? $"{colorName} (Applied)"
                    : $"{colorName} (Free)";
            else
                bt.text = isApplied
                    ? $"{colorName} (Applied)"
                    : $"{colorName} (${paintCost})";
        }
        button.interactable = !isApplied;
    }

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
        int selectedCarId = CarSelection.SelectedCarId;
        if (selectedCarId < 0) return;

        CarStats carStats = CarDataManager.GetCarStatsById(selectedCarId);
        if (carStats == null) return;

        if (!playerStats.IsCarPurchased(carStats.ID)) return;
        if (!carStats.AvailableExtraParts.Contains(partName)) return;

        bool isActive = carStats.ActiveExtraParts.Contains(partName);
        if (isActive)
        {
            carStats.DeactivateExtraPart(partName);
        }
        else
        {
            if (playerStats.Money < extraPartCost) return;
            playerStats.AddMoney(-extraPartCost);
            PlayerDataManager.SavePlayerStats(playerStats);
            menuUIManager.UpdateMoneyText();
            carStats.ActivateExtraPart(partName);
            CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());
        }

        GlobalEventManager.TriggerCarUpdated();
    }

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
        if (selectedCarId < 0) return;

        CarStats carStats = CarDataManager.GetCarStatsById(selectedCarId);
        if (carStats == null) return;
        if (!playerStats.IsCarPurchased(carStats.ID)) return;
        if (carStats.PaintColor == colorHex) return;

        if (!isFree)
        {
            if (playerStats.Money < paintCost) return;
            playerStats.AddMoney(-paintCost);
            PlayerDataManager.SavePlayerStats(playerStats);
            menuUIManager.UpdateMoneyText();
        }

        carStats.SetPaintColor(colorHex);
        CarDataManager.SaveAllCarStats(menuCarSpawner.GetAllCarStats());

        GameObject currentCar = menuCarSpawner.GetCurrentCarInstance();
        if (currentCar)
        {
            ApplyPaintColorToCar(currentCar, colorHex);
        }

        GlobalEventManager.TriggerCarUpdated();
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
        }
    }
}
