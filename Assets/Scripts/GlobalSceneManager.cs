using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance { get; private set; }
    private bool _isLoadingScene = false;
    
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
        Debug.Log("[GlobalSceneManager] Level started");
        TriggerLoadScene(gameSceneName);
    }

    private void HandleLevelEnd()
    {
        Debug.Log("[GlobalSceneManager] Level ended");
        TriggerLoadScene(mainMenuSceneName);
    }

    private void HandleSceneChanged(string sceneName)
    {
        Debug.Log($"[GlobalSceneManager] Scene changed to: {sceneName}");
        TriggerLoadScene(sceneName);
    }

    private void TriggerLoadScene(string sceneName)
    {
        if (_isLoadingScene)
        {
            Debug.LogWarning("[GlobalSceneManager] Scene loading is already in progress.");
            return;
        }

        _isLoadingScene = true;
        LoadScene(sceneName);
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log($"[GlobalSceneManager] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
        _isLoadingScene = false;
    }
}