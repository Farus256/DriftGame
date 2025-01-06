using UnityEngine;
using static IronSource;

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

        Debug.Log("[IronSourceInitializer] Initializing IronSource with AppKey: " + appKey);

        // Инициализация IronSource SDK
        IronSource.Agent.init(appKey);
        IronSource.Agent.validateIntegration();

        // Проверка доступности Rewarded Video
        CheckInitializationStatus();
    }

    private void CheckInitializationStatus()
    {
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
    }

    private void OnApplicationPause(bool isPaused)
    {
        // Уведомляем IronSource о состоянии приложения
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void Update()
    {
        // Если нужно, можно периодически проверять статус и вывести в лог
        if (!isInitialized)
        {
            CheckInitializationStatus();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("[IronSourceInitializer] Destroying IronSource Manager...");
    }
}
