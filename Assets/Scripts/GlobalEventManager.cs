using UnityEngine;
using UnityEngine.Events;

public static class GlobalEventManager
{
    public static UnityEvent onLevelEnd = new UnityEvent();
    public static UnityEvent onLevelStart = new UnityEvent();
    public static UnityEvent<string> onSceneChanged = new UnityEvent<string>();

    public static void TriggerLevelEnd() => onLevelEnd?.Invoke();
    public static void TriggerLevelStart() => onLevelStart?.Invoke();
    public static void TriggerSceneChanged(string sceneName) => onSceneChanged?.Invoke(sceneName);
}