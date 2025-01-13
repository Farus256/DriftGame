using UnityEngine;

// Убедитесь, что этот скрипт прикреплен к объекту IronSourceRewardedVideoEvents в вашей сцене
public class RewardedVideoEventReceiver : MonoBehaviour
{
    /// <summary>
    /// Метод вызывается, когда реклама открыта
    /// </summary>
    public void OnAdOpened()
    {
        Debug.Log("IronSource: Rewarded Video Ad Opened");
    }

    /// <summary>
    /// Метод вызывается, когда реклама закрыта
    /// </summary>
    public void OnAdClosed()
    {
        Debug.Log("IronSource: Rewarded Video Ad Closed");
        // Опционально: Загрузка следующей рекламы после закрытия
        IronSource.Agent.loadRewardedVideo();
    }

    /// <summary>
    /// Метод вызывается, когда пользователь должен быть награжден за просмотр рекламы
    /// </summary>
    /// <param name="placement">Информация о размещении рекламы</param>
    public void OnAdRewarded(IronSourcePlacement placement)
    {
        Debug.Log("IronSource: Rewarded Video Ad Rewarded");
        // Вызов глобального события награждения
        GlobalEventManager.TriggerRewardedVideoCompleted();
    }

    /// <summary>
    /// Метод вызывается, когда произошла ошибка при показе рекламы
    /// </summary>
    /// <param name="error">Информация об ошибке</param>
    public void OnAdShowFailed(IronSourceError error)
    {
        Debug.LogError($"IronSource: Rewarded Video Ad Show Failed: {error.getDescription()}");
        // Обработка ошибки показа рекламы, например, можно предоставить стандартную награду
        GlobalEventManager.TriggerRewardedVideoCompleted();
    }

    /// <summary>
    /// Метод вызывается, когда пользователь кликнул на рекламу
    /// </summary>
    /// <param name="placement">Информация о размещении рекламы</param>
    public void OnAdClicked(IronSourcePlacement placement)
    {
        Debug.Log("IronSource: Rewarded Video Ad Clicked");
    }
}