using UnityEngine;

public class IronSourceInitializer : MonoBehaviour
{
    [Header("IronSource App Key")]
    [SerializeField] private string appKey = "20a6c9acd"; // Замените на ваш ключ из IronSource Dashboard 

    private static IronSourceInitializer _instance;
    private bool isInitialized = false; // Флаг успешной инициализации

    private void Awake()
    {
        // Убедимся, что существует только один экземпляр этого объекта
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Сделаем объект постоянным между сценами
        DontDestroyOnLoad(gameObject);

        // Инициализация IronSource SDK
        InitializeIronSource();
    }

    private void InitializeIronSource()
    {
        if (string.IsNullOrEmpty(appKey))
        {
            Debug.LogError("[IronSourceInitializer] AppKey is not set! Please configure it in the Inspector.");
            return;
        }

#if UNITY_EDITOR
        // В редакторе симулируем доступность рекламы
        Debug.Log("[IronSourceInitializer] Simulating IronSource initialization in Unity Editor.");
        isInitialized = true;
#else
        // Для реального устройства
        Debug.Log("[IronSourceInitializer] Initializing IronSource with AppKey: " + appKey);

        // Инициализация IronSource SDK
        IronSource.Agent.init(appKey);
        IronSource.Agent.validateIntegration();

        // Проверка доступности Rewarded Video
        CheckInitializationStatus();
#endif
    }

    private void CheckInitializationStatus()
    {
#if UNITY_EDITOR
        Debug.Log("[IronSourceInitializer] Simulating rewarded video availability in Unity Editor.");
        isInitialized = true;
#else
        // Проверим, доступно ли Rewarded Video как индикатор успешной инициализации
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            isInitialized = true;
            Debug.Log("[IronSourceInitializer] IronSource initialized successfully. Rewarded Video is available.");
        }
        else
        {
            Debug.LogWarning("[IronSourceInitializer] IronSource initialization in progress or failed. Rewarded Video not available.");
        }
#endif
    }

    private void OnApplicationPause(bool isPaused)
    {
#if !UNITY_EDITOR
        // Уведомляем IronSource о состоянии приложения
        IronSource.Agent.onApplicationPause(isPaused);
#endif
    }

    private void Update()
    {
        // Если нужно, можно периодически проверять статус и вывести в лог
        if (!isInitialized)
        {
            CheckInitializationStatus();
        }
    }

    public void ShowRewardedVideo()
    {
#if UNITY_EDITOR
        Debug.Log("[IronSourceInitializer] Simulating Rewarded Ad in Unity Editor.");
        SimulateRewardedAd();
#else
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            Debug.Log("[IronSourceInitializer] Showing Rewarded Video...");
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.LogWarning("[IronSourceInitializer] Rewarded Video is not available.");
        }
#endif
    }

    private void SimulateRewardedAd()
    {
        // Симулируем события Rewarded Ad в редакторе
        Debug.Log("Simulating Rewarded Ad Completion...");
        
    }

    private void OnDestroy()
    {
        Debug.Log("[IronSourceInitializer] Destroying IronSource Manager...");
    }
}
