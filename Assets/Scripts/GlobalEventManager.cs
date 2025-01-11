using UnityEngine;
using UnityEngine.Events;

public static class GlobalEventManager
{
    // Существующие события
    public static UnityEvent onLevelEnd = new UnityEvent();
    public static UnityEvent onLevelStart = new UnityEvent();
    public static UnityEvent<string> onSceneChanged = new UnityEvent<string>();

    // Новое событие, заменяющее CarEvents.OnCarUpdated
    public static UnityEvent onCarUpdated = new UnityEvent();

    public static void TriggerLevelEnd() => onLevelEnd?.Invoke();
    public static void TriggerLevelStart() => onLevelStart?.Invoke();
    public static void TriggerSceneChanged(string sceneName) => onSceneChanged?.Invoke(sceneName);

    // Вызываем "обновление машины"
    public static void TriggerCarUpdated() => onCarUpdated?.Invoke();
}