using UnityEngine;
using Photon.Pun;

// Добавим PhotonView, PhotonTransformView руками или в Инспекторе
[RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
public class CarController : MonoBehaviourPun
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms (Visuals)")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Car Settings")]
    public float motorPower     = 1500f; 
    public float maxSteerAngle  = 25f;   
    public float brakeForce     = 1500f; 
    public float handbrakeForce = 3000f; 
    public float driftFactor    = 0.5f;  
    public float autoSteerSpeed = 2f;    
    public float driftThreshold = 10f;   
    public float autoSteerMinSpeed = 5f; 
    public bool  CanDrive       = true;

    private Rigidbody _rb;
    private float     _currentSteerAngle;
    private bool      _isDrifting;

    private WheelFrictionCurve _rearLeftOriginalFriction;
    private WheelFrictionCurve _rearRightOriginalFriction;

    public bool  IsDrifting      => _isDrifting;
    public float CurrentSpeedKmh => _rb.velocity.magnitude * 3.6f;

    public delegate void DriftEventHandler(bool started, float driftDuration);
    public event DriftEventHandler OnDriftEvent;

    private bool  _wasDriftingLastFrame;
    private float _driftStartTime;

    // --- Поля для мобильного управления (пример) ---
    public bool acceleratePressed = false;
    public bool brakePressed      = false;
    public bool steerLeftPressed  = false;
    public bool steerRightPressed = false;
    public bool handbrakePressed  = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        // Сохраняем оригинальные настройки трения задних колёс
        _rearLeftOriginalFriction  = rearLeftWheelCollider.sidewaysFriction;
        _rearRightOriginalFriction = rearRightWheelCollider.sidewaysFriction;
    }

    private void Update()
    {
        // Если это не наша машина - выходим (не обрабатываем ввод), 
        // но колёса всё равно визуально обновим.
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            UpdateWheelVisuals(frontLeftWheelCollider,  frontLeftWheelTransform,  true);
            UpdateWheelVisuals(frontRightWheelCollider, frontRightWheelTransform, true);
            UpdateWheelVisuals(rearLeftWheelCollider,   rearLeftWheelTransform,   false);
            UpdateWheelVisuals(rearRightWheelCollider,  rearRightWheelTransform,  false);
            return;
        }

        if (CanDrive)
        {
            float verticalInput   = GetVerticalInput();   // газ/тормоз
            float horizontalInput = GetHorizontalInput(); // поворот

            HandleMotor(verticalInput);
            HandleSteering(horizontalInput);
            HandleBrakes(verticalInput);
        }
        else
        {
            // Полная блокировка колёс
            frontLeftWheelCollider.brakeTorque  = 9999f;
            frontRightWheelCollider.brakeTorque = 9999f;
            rearLeftWheelCollider.brakeTorque   = 9999f;
            rearRightWheelCollider.brakeTorque  = 9999f;
        }

        // Дрифт (учёт времени дрифта)
        HandleDriftTimeCounting();

        // Обновляем визуал колёс (у локального игрока)
        UpdateWheelVisuals(frontLeftWheelCollider,  frontLeftWheelTransform,  true);
        UpdateWheelVisuals(frontRightWheelCollider, frontRightWheelTransform, true);
        UpdateWheelVisuals(rearLeftWheelCollider,   rearLeftWheelTransform,   false);
        UpdateWheelVisuals(rearRightWheelCollider,  rearRightWheelTransform,  false);

        _wasDriftingLastFrame = _isDrifting;
    }

    // --- Методы получения «мобильного» ввода ---
    float GetVerticalInput()
    {
        // Газ вперёд
        if (acceleratePressed && !brakePressed)
        {
            return 1f;
        }
        // Тормоз / Реверс
        else if (brakePressed && !acceleratePressed)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    float GetHorizontalInput()
    {
        // Поворот влево
        if (steerLeftPressed && !steerRightPressed)
        {
            return -1f;
        }
        // Поворот вправо
        else if (steerRightPressed && !steerLeftPressed)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    private void HandleMotor(float verticalInput)
    {
        float motorTorque = verticalInput * motorPower;
        rearLeftWheelCollider.motorTorque  = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
    }

    private void HandleSteering(float horizontalInput)
    {
        _currentSteerAngle = maxSteerAngle * horizontalInput;

        // Проверка на дрифт
        float slip = GetAverageSideSlip();
        _isDrifting = Mathf.Abs(slip) > driftThreshold;

        if (_isDrifting)
        {
            // Сниженное сцепление задних колёс
            AdjustDriftFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction, driftFactor);
            AdjustDriftFriction(rearRightWheelCollider, _rearRightOriginalFriction, driftFactor);

            // Автоподруливание
            if (_rb.velocity.magnitude > autoSteerMinSpeed)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
                float   autoSteer     = -localVelocity.x * autoSteerSpeed;

                _currentSteerAngle = Mathf.Lerp(
                    _currentSteerAngle,
                    _currentSteerAngle + autoSteer,
                    Time.deltaTime
                );
            }
        }
        else
        {
            // Восстанавливаем обычное сцепление
            ResetFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction);
            ResetFriction(rearRightWheelCollider, _rearRightOriginalFriction);
        }

        // Применяем угол руля (передние колёса)
        frontLeftWheelCollider.steerAngle  = _currentSteerAngle;
        frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    private void HandleBrakes(float verticalInput)
    {
        // Ручник => блокирует задние колёса
        if (handbrakePressed)
        {
            rearLeftWheelCollider.brakeTorque  = handbrakeForce;
            rearRightWheelCollider.brakeTorque = handbrakeForce;

            // Чуть снижаем сцепление
            AdjustDriftFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction, 0.8f);
            AdjustDriftFriction(rearRightWheelCollider, _rearRightOriginalFriction, 0.8f);
        }
        else
        {
            rearLeftWheelCollider.brakeTorque  = 0f;
            rearRightWheelCollider.brakeTorque = 0f;

            if (!_isDrifting)
            {
                ResetFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction);
                ResetFriction(rearRightWheelCollider, _rearRightOriginalFriction);
            }
        }

        // Торможение передними колёсами:
        float forwardSpeed = Vector3.Dot(_rb.velocity, transform.forward);

        if (verticalInput < 0f && forwardSpeed > 1f)
        {
            frontLeftWheelCollider.brakeTorque  = brakeForce;
            frontRightWheelCollider.brakeTorque = brakeForce;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque  = 0f;
            frontRightWheelCollider.brakeTorque = 0f;
        }
    }

    private void HandleDriftTimeCounting()
    {
        if (_isDrifting && !_wasDriftingLastFrame)
        {
            // Начало дрифта
            _driftStartTime = Time.time;
            OnDriftEvent?.Invoke(true, 0f);
        }
        else if (!_isDrifting && _wasDriftingLastFrame)
        {
            // Конец дрифта
            float driftEndTime  = Time.time;
            float driftDuration = driftEndTime - _driftStartTime;
            OnDriftEvent?.Invoke(false, driftDuration);
        }
    }

    private float GetAverageSideSlip()
    {
        WheelHit hit;
        float totalSlip = 0f;
        int   count     = 0;

        if (rearLeftWheelCollider.GetGroundHit(out hit))
        {
            totalSlip += hit.sidewaysSlip;
            count++;
        }
        if (rearRightWheelCollider.GetGroundHit(out hit))
        {
            totalSlip += hit.sidewaysSlip;
            count++;
        }
        return (count > 0) ? (totalSlip / count) : 0f;
    }

    private void AdjustDriftFriction(WheelCollider wheelCollider, WheelFrictionCurve originalFriction, float factor)
    {
        WheelFrictionCurve friction = wheelCollider.sidewaysFriction;
        friction.extremumValue  = originalFriction.extremumValue  * factor;
        friction.asymptoteValue = originalFriction.asymptoteValue * factor;
        wheelCollider.sidewaysFriction = friction;
    }

    private void ResetFriction(WheelCollider wheelCollider, WheelFrictionCurve originalFriction)
    {
        wheelCollider.sidewaysFriction = originalFriction;
    }

    // -- Главное изменение: убираем двойное вращение, полагаемся на GetWorldPose
    private void UpdateWheelVisuals(WheelCollider collider, Transform wheelTransform, bool isFrontWheel)
    {
        if (wheelTransform == null) return;

        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    // --- КНОПКА ПЕРЕВОРОТА МАШИНЫ ---
    public void FlipCar()
    {
        transform.position += Vector3.up * 1.5f;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        _rb.velocity        = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
}
