using UnityEngine;

public class MultiPlayerCamera : MonoBehaviour
{
    private Transform target;         // Ссылка на объект машины

    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float smoothSpeed = 5f;   // Скорость сглаживания движения

    private void OnEnable()
    {
        // Подписываемся на событие спавна локального автомобиля
        MultiPlayerCarSpawner.OnLocalCarSpawned += HandleLocalCarSpawned;
    }

    private void OnDisable()
    {
        // Отписываемся от события при отключении
        MultiPlayerCarSpawner.OnLocalCarSpawned -= HandleLocalCarSpawned;
    }

    // Метод-колбэк для события спавна локального автомобиля
    private void HandleLocalCarSpawned(GameObject localCar)
    {
        target = localCar.transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Желаемая позиция
        Vector3 desiredPosition = target.position + offset;

        // Сглаживаем движение камеры
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * smoothSpeed
        );

        // Смотрим на цель (обычно на центр автомобиля)
        transform.LookAt(target.position + Vector3.up * 1.0f);
    }
}