using UnityEngine;

public class IronSourceLevelPlayManager : MonoBehaviour
{
    public static IronSourceLevelPlayManager Instance { get; private set; }

    // Замените на ваш собственный App Key из IronSource Dashboard
    [SerializeField] private string appKey = "zaglushka";

    void Awake()
    {
        // Реализация Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeIronSource();
    }

    /// <summary>
    /// Инициализация IronSource LevelPlay SDK
    /// </summary>
    void InitializeIronSource()
    {
        // Инициализация IronSource с указанием App Key и рекламных форматов
        IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO);

        // Опционально: Включение отладочных логов
        IronSource.Agent.setAdaptersDebug(true);

        // Загрузка Rewarded Video рекламы
        IronSource.Agent.loadRewardedVideo();

        // Прикрепление RewardedVideoEventReceiver к IronSourceRewardedVideoEvents GameObject
        Invoke("AttachRewardedVideoEventReceiver", 1f); // Задержка 1 секунда, чтобы гарантировать создание объекта
    }

    /// <summary>
    /// Метод для прикрепления RewardedVideoEventReceiver к IronSourceRewardedVideoEvents
    /// </summary>
    void AttachRewardedVideoEventReceiver()
    {
        GameObject rewardedVideoEvents = GameObject.Find("IronSourceRewardedVideoEvents");
        if (rewardedVideoEvents != null)
        {
            RewardedVideoEventReceiver receiver = rewardedVideoEvents.GetComponent<RewardedVideoEventReceiver>();
            if (receiver == null)
            {
                rewardedVideoEvents.AddComponent<RewardedVideoEventReceiver>();
                Debug.Log("RewardedVideoEventReceiver добавлен к IronSourceRewardedVideoEvents.");
            }
            else
            {
                Debug.Log("RewardedVideoEventReceiver уже прикреплен к IronSourceRewardedVideoEvents.");
            }
        }
        else
        {
            Debug.LogError("IronSourceRewardedVideoEvents не найден в сцене.");
        }
    }

    /// <summary>
    /// Метод для отображения Rewarded Video рекламы
    /// </summary>
    public void ShowRewardedVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("IronSource: Rewarded Video Ad is not available");
            // Обработка случая, когда реклама недоступна, например, сразу наградить игрока
            GlobalEventManager.TriggerRewardedVideoCompleted();
        }
    }

    void OnDestroy()
    {
        // Дополнительная очистка при необходимости
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
