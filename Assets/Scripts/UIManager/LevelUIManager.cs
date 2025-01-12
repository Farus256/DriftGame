using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUIManager : MonoBehaviour
{
    // Singleton Instance
    public static LevelUIManager Instance { get; private set; }
    
    [Header("Buttons")]
    [SerializeField] private Button loadFreeLevelButton;
    [SerializeField] private Button loadPaidLevelButton1;
    [SerializeField] private Button loadPaidLevelButton2;

    [Header("Scene Names")] 
    [SerializeField] private string freeLevelSceneName = "FreeLevel";
    [SerializeField] private string paidLevelSceneName1 = "PaidLevel1";
    [SerializeField] private string paidLevelSceneName2 = "PaidLevel2";
    
    [Header("Level Costs")]
    [SerializeField] private float paidLevelCost1 = 500f;
    [SerializeField] private float paidLevelCost2 = 1000f;
    
    
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
        if (playerStats != null)
        {
            playerStats.OnMoneyChanged += OnMoneyChanged;
        }
        
        if (loadFreeLevelButton)
            loadFreeLevelButton.onClick.AddListener(OnLoadFreeLevelButtonClicked);
        if (loadPaidLevelButton1)
            loadPaidLevelButton1.onClick.AddListener(OnLoadPaidLevelButton1Clicked);
        if (loadPaidLevelButton2)
            loadPaidLevelButton2.onClick.AddListener(OnLoadPaidLevelButton2Clicked);
        
        UpdateButtonInteractability();
    }
    
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnMoneyChanged -= OnMoneyChanged;
        }
        
        if (loadFreeLevelButton)
            loadFreeLevelButton.onClick.RemoveListener(OnLoadFreeLevelButtonClicked);
        if (loadPaidLevelButton1)
            loadPaidLevelButton1.onClick.RemoveListener(OnLoadPaidLevelButton1Clicked);
        if (loadPaidLevelButton2)
            loadPaidLevelButton2.onClick.RemoveListener(OnLoadPaidLevelButton2Clicked);
    }
    
    private void OnMoneyChanged(float currentMoney)
    {
        UpdateButtonInteractability();
    }
    
    private void UpdateButtonInteractability()
    {
        if (playerStats == null)
            return;
        
        if (loadPaidLevelButton1)
            loadPaidLevelButton1.interactable = playerStats.Money >= paidLevelCost1;
        if (loadPaidLevelButton2)
            loadPaidLevelButton2.interactable = playerStats.Money >= paidLevelCost2;
    }
    
    private void OnLoadFreeLevelButtonClicked()
    {
        GlobalEventManager.TriggerSceneChanged(freeLevelSceneName);
    }
    
    private void OnLoadPaidLevelButton1Clicked()
    {
        AttemptLoadPaidLevel(paidLevelSceneName1, paidLevelCost1);
    }
    
    private void OnLoadPaidLevelButton2Clicked()
    {
        AttemptLoadPaidLevel(paidLevelSceneName2, paidLevelCost2);
    }
    
    private void AttemptLoadPaidLevel(string sceneName, float cost)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null. Cannot process level loading.");
            return;
        }
        
        if (playerStats.Money >= cost)
        {
            playerStats.AddMoney(-cost);
            PlayerDataManager.SavePlayerStats(playerStats);
            
            GlobalEventManager.TriggerSceneChanged(sceneName);
        }
        else
        {
            Debug.Log("Not enough money to load this level.");
        }
    }
}
