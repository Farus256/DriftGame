using UnityEngine;

public class SinglePlayerCamera : MonoBehaviour
{
    private Transform target;

    [Header("Настройки камеры")]
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float smoothSpeed = 5f;

    [Header("Настройки вращения камеры")]
    public float rotationSpeed = 100f;
    public float minYAngle = -20f;
    public float maxYAngle = 80f;

    [Header("Настройки авто-выпрямления")]
    public float straightenSpeed = 2f;
    public float accelerationThreshold = 5f;

    private float currentYAngle = 0f;
    private Vector3 defaultOffset;
    private Quaternion defaultRotation;

    private Vector2 previousTouchPosition;
    private bool isTouching = false;

    private void Start()
    {
        GameObject playerCar = GameObject.FindGameObjectWithTag("Player");
        if (playerCar != null)
        {
            target = playerCar.transform;
        }
        else
        {
            Debug.LogError("[SinglePlayerCamera] Машина игрока с тегом 'Player' не найдена в сцене.");
        }

        defaultOffset = offset;
        defaultRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleRotation();
        HandleAutoStraighten();

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(target.position + Vector3.up * 1.0f);
    }

    private void HandleRotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isTouching = true;
                    previousTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (isTouching)
                    {
                        Vector2 delta = touch.position - previousTouchPosition;
                        previousTouchPosition = touch.position;

                        float deltaX = delta.x * rotationSpeed * Time.deltaTime;
                        float deltaY = delta.y * rotationSpeed * Time.deltaTime;

                        transform.RotateAround(target.position, Vector3.up, deltaX);

                        currentYAngle -= deltaY;
                        currentYAngle = Mathf.Clamp(currentYAngle, minYAngle, maxYAngle);

                        Quaternion verticalRotation = Quaternion.Euler(currentYAngle, 0, 0);
                        offset = verticalRotation * defaultOffset;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    break;
            }
        }
    }

    private void HandleAutoStraighten()
    {
        float currentSpeed = GetCarSpeed();

        if (currentSpeed > accelerationThreshold)
        {
            offset = Vector3.Lerp(offset, defaultOffset, Time.deltaTime * straightenSpeed);
            currentYAngle = Mathf.Lerp(currentYAngle, 0f, Time.deltaTime * straightenSpeed);
        }
    }

    private float GetCarSpeed()
    {
        Rigidbody carRigidbody = target.GetComponent<Rigidbody>();
        if (carRigidbody != null)
        {
            return carRigidbody.velocity.magnitude * 3.6f;
        }

        Debug.LogWarning("[SinglePlayerCamera] Не удалось получить скорость машины. Убедитесь, что у машины есть компонент Rigidbody.");
        return 0f;
    }
}
