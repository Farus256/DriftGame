using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;         // Ссылка на объект машины
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float smoothSpeed = 5f;   // Скорость сглаживания движения
    
    void LateUpdate()
    {
        if (!target) return;
        
        // Желаемая позиция
        Vector3 desiredPosition = target.position + offset;
        // Сглаживаем движение камеры
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        
        // Смотрим на цель (обычно на центр автомобиля)
        transform.LookAt(target.position + Vector3.up * 1.0f);
    }
}