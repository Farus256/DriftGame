using UnityEngine;

public class SinglePlayerCamera : MonoBehaviour
{
    private Transform target;         // Ссылка на объект машины

    [Header("Настройки камеры")]
    public Vector3 offset = new Vector3(0f, 3f, -6f);   // Смещение камеры относительно машины
    public float smoothSpeed = 5f;                     // Скорость сглаживания движения камеры

    private void Start()
    {
        // Поиск машины игрока по тегу "Player"
        GameObject playerCar = GameObject.FindGameObjectWithTag("Player");
        if (playerCar != null)
        {
            target = playerCar.transform;
        }
        else
        {
            Debug.LogError("[SinglePlayerCamera] Машина игрока с тегом 'Player' не найдена в сцене.");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Вычисляем желаемую позицию камеры
        Vector3 desiredPosition = target.position + offset;

        // Плавно перемещаем камеру к желаемой позиции
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // Камера смотрит на позицию машины с небольшим смещением вверх
        transform.LookAt(target.position + Vector3.up * 1.0f);
    }
}