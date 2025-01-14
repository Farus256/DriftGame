using UnityEngine;

public class IronSourceLevelPlayManager : MonoBehaviour
{
    public static IronSourceLevelPlayManager Instance { get; private set; }
    
    [SerializeField] private string appKey = "zaglushka";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeIronSource();
    }

   
    void InitializeIronSource()
    {
        IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO);
        
        IronSource.Agent.setAdaptersDebug(true);
        
        IronSource.Agent.loadRewardedVideo();
        
        Invoke("AttachRewardedVideoEventReceiver", 1f);
    }
    
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
    
    public void ShowRewardedVideo()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("IronSource: Rewarded Video Ad is not available");
            
            GlobalEventManager.TriggerRewardedVideoCompleted();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
