// using UnityEngine;
//
// public class IronSourceInitializer : MonoBehaviour
// {
//     private string appKey = "ВАШ_APP_KEY";  // Получаете из IronSource дашборда
//
//     void Start()
//     {
//         // Подписываемся на событие, указывающее, что SDK проинициализировался.
//         IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompleted;
//         
//         // Инициализируем SDK, передавая наш appKey.
//         IronSource.Agent.init(appKey);
//         
//         // (необязательно, но рекомендуется)
//         IronSource.Agent.validateIntegration();
//     }
//
//     private void SdkInitializationCompleted()
//     {
//         Debug.Log("IronSource (LevelPlay) SDK initialization completed!");
//         // Можно загружать/показывать рекламу
//         LoadRewardedAd();
//     }
//
//     private void LoadRewardedAd()
//     {
//         // Для ironSource (LevelPlay) rewarded video обычно грузится автоматически,
//         // но вы можете вручную вызвать загрузку — зависит от версии SDK.
//         // Для баннеров, интерстициалов — есть свои методы loadBanner, loadInterstitial и т.д.
//         Debug.Log("Load Rewarded Ad");
//         // IronSource.Agent.loadRewardedVideo(); // если нужно явно вызвать загрузку
//     }
//
//     public void ShowRewardedAd()
//     {
//         if (IronSource.Agent.isRewardedVideoAvailable())
//         {
//             Debug.Log("Showing Rewarded Video...");
//             IronSource.Agent.showRewardedVideo();
//         }
//         else
//         {
//             Debug.LogWarning("Rewarded Video not available");
//         }
//     }
//
//     private void OnEnable()
//     {
//         // Подписка на события rewarded video
//         // IronSourceEvents.onRewardedVideoAdOpenedEvent += OnRewardedVideoAdOpenedEvent;
//         // IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedVideoAdClosedEvent;
//         // IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChangedEvent;
//         // IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewardedEvent;
//         // IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailedEvent;
//     }
//
//     private void OnDisable()
//     {
//         // Отписка
//         // IronSourceEvents.onRewardedVideoAdOpenedEvent -= OnRewardedVideoAdOpenedEvent;
//         // IronSourceEvents.onRewardedVideoAdClosedEvent -= OnRewardedVideoAdClosedEvent;
//         // IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= OnRewardedVideoAvailabilityChangedEvent;
//         // IronSourceEvents.onRewardedVideoAdRewardedEvent -= OnRewardedVideoAdRewardedEvent;
//         // IronSourceEvents.onRewardedVideoAdShowFailedEvent -= OnRewardedVideoAdShowFailedEvent;
//     }
//
//     private void OnRewardedVideoAdOpenedEvent()
//     {
//         Debug.Log("Rewarded video ad opened");
//     }
//
//     private void OnRewardedVideoAdClosedEvent()
//     {
//         Debug.Log("Rewarded video ad closed");
//     }
//
//     private void OnRewardedVideoAvailabilityChangedEvent(bool available)
//     {
//         Debug.Log("Rewarded video availability changed: " + available);
//         // Вы можете менять interactable кнопки, если хотите
//     }
//
//     private void OnRewardedVideoAdRewardedEvent(IronSourcePlacement placement)
//     {
//         // Здесь вы даёте пользователю награду
//         Debug.Log("User rewarded! placement: " + placement.getRewardName() + " amount: " + placement.getRewardAmount());
//     }
//
//     private void OnRewardedVideoAdShowFailedEvent(IronSourceError error)
//     {
//         Debug.LogError("Failed to show rewarded video: " + error.getDescription());
//     }
// }
//
