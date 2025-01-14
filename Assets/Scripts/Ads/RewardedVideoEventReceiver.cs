using UnityEngine;

public class RewardedVideoEventReceiver : MonoBehaviour
{
    public void OnAdOpened()
    {
        Debug.Log("IronSource: Rewarded Video Ad Opened");
    }
    
    public void OnAdClosed()
    {
        Debug.Log("IronSource: Rewarded Video Ad Closed");
        
        IronSource.Agent.loadRewardedVideo();
    }
    
    public void OnAdRewarded(IronSourcePlacement placement)
    {
        Debug.Log("IronSource: Rewarded Video Ad Rewarded");
        
        GlobalEventManager.TriggerRewardedVideoCompleted();
    }
    
    public void OnAdShowFailed(IronSourceError error)
    {
        Debug.LogError($"IronSource: Rewarded Video Ad Show Failed: {error.getDescription()}");
        
        GlobalEventManager.TriggerRewardedVideoCompleted();
    }
    
    public void OnAdClicked(IronSourcePlacement placement)
    {
        Debug.Log("IronSource: Rewarded Video Ad Clicked");
    }
}