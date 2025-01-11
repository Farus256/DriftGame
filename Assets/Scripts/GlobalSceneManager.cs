using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance { get; private set; }
    private bool isLoadingScene = false;

    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName = "TestScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SubscribeToGlobalEvents();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            UnsubscribeFromGlobalEvents();
            Instance = null;
        }
    }

    private void SubscribeToGlobalEvents()
    {
        GlobalEventManager.onLevelStart.AddListener(HandleLevelStart);
        GlobalEventManager.onLevelEnd.AddListener(HandleLevelEnd);
        GlobalEventManager.onSceneChanged.AddListener(HandleSceneChanged);
    }

    private void UnsubscribeFromGlobalEvents()
    {
        GlobalEventManager.onLevelStart.RemoveListener(HandleLevelStart);
        GlobalEventManager.onLevelEnd.RemoveListener(HandleLevelEnd);
        GlobalEventManager.onSceneChanged.RemoveListener(HandleSceneChanged);
    }

    private void HandleLevelStart()
    {
        if (!isLoadingScene) TriggerLoadScene(gameSceneName);
    }

    private void HandleLevelEnd()
    {
        if (!isLoadingScene) TriggerLoadScene(mainMenuSceneName);
    }

    private void HandleSceneChanged(string sceneName)
    {
        if (!isLoadingScene) TriggerLoadScene(sceneName);
    }

    private void TriggerLoadScene(string sceneName)
    {
        if (isLoadingScene) return;
        isLoadingScene = true;
        SceneManager.LoadScene(sceneName);
        isLoadingScene = false;
    }
}