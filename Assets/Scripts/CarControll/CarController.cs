using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
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
    public float motorPower = 1500f; 
    public float maxSteerAngle = 25f;  
    public float brakeForce = 1500f; 
    public float handbrakeForce = 3000f;  
    public float driftFactor = 0.5f; 
    public float autoSteerSpeed = 2f;  
    public float driftThreshold = 10f;  
    public float autoSteerMinSpeed = 5f; 

    public bool CanDrive { get; set; } = true; 

    private Rigidbody _rb;
    private float _currentSteerAngle;
    private bool _isDrifting;

    private WheelFrictionCurve _rearLeftOriginalFriction;
    private WheelFrictionCurve _rearRightOriginalFriction;

    public bool IsDrifting => _isDrifting;
    public float CurrentSpeedKmh => _rb.velocity.magnitude * 3.6f;

    public delegate void DriftEventHandler(bool started, float driftDuration);
    public event DriftEventHandler OnDriftEvent;

    private bool  _wasDriftingLastFrame;
    private float _driftStartTime;

    private PhotonView _photonView;
    private bool _isOnMultiplayer;

    // --- Поля для мобильного управления ---
    [Header("Mobile Input Flags")]
    public bool acceleratePressed = false;
    public bool brakePressed      = false;
    public bool steerLeftPressed  = false;
    public bool steerRightPressed = false;
    public bool handbrakePressed  = false;

    private void Start()
    {
        // Проверка на PhotonView
        if (GetComponent<PhotonView>() != null)
        {
            _isOnMultiplayer = true;
            _photonView = GetComponent<PhotonView>();
        }
        else
        {
            _isOnMultiplayer = false;
        }
        
        _rb = GetComponent<Rigidbody>();

        // Сохраняем оригинальные настройки трения задних колёс
        _rearLeftOriginalFriction  = rearLeftWheelCollider.sidewaysFriction;
        _rearRightOriginalFriction = rearRightWheelCollider.sidewaysFriction;
    }

    private void Update()
    {
        if (_isOnMultiplayer && PhotonNetwork.IsConnected && !_photonView.IsMine)
        {
            return;
        }

        if (CanDrive)
        {
            // --- 1) Получаем «мобильный» ввод ---
            float verticalInput   = GetVerticalInput();   // газ / тормоз
            float horizontalInput = GetHorizontalInput(); // поворот

            // --- 2) Управляем машиной ---
            HandleMotor(verticalInput);
            HandleSteering(horizontalInput);
            HandleBrakes();
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

        // Обновляем визуал колёс
        UpdateWheelVisuals(frontLeftWheelCollider,  frontLeftWheelTransform,  true);
        UpdateWheelVisuals(frontRightWheelCollider, frontRightWheelTransform, true);
        UpdateWheelVisuals(rearLeftWheelCollider,   rearLeftWheelTransform,   false);
        UpdateWheelVisuals(rearRightWheelCollider,  rearRightWheelTransform,  false);

        _wasDriftingLastFrame = _isDrifting;
    }

    // --- Методы получения «мобильного» ввода ---
    float GetVerticalInput()
    {
        // Газ
        if (acceleratePressed && !brakePressed)
        {
            return 1f;
        }
        // Тормоз
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
                float autoSteer = -localVelocity.x * autoSteerSpeed;

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

    private void HandleBrakes()
    {
        // Ручник => блокирует задние колёса
        if (handbrakePressed)
        {
            rearLeftWheelCollider.brakeTorque  = handbrakeForce;
            rearRightWheelCollider.brakeTorque = handbrakeForce;

            // Чуть повышаем склонность к заносу
            AdjustDriftFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction, 0.8f);
            AdjustDriftFriction(rearRightWheelCollider, _rearRightOriginalFriction, 0.8f);
        }
        else
        {
            rearLeftWheelCollider.brakeTorque  = 0f;
            rearRightWheelCollider.brakeTorque = 0f;

            if (!_isDrifting)
            {
                // Если не дрифтим, возвращаем заводское трение
                ResetFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction);
                ResetFriction(rearRightWheelCollider, _rearRightOriginalFriction);
            }
        }

        // Отдельно тормозим передние колёса при нажатой кнопке «тормоз»
        if (brakePressed && !acceleratePressed)
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
            float driftEndTime = Time.time;
            float driftDuration = driftEndTime - _driftStartTime;
            OnDriftEvent?.Invoke(false, driftDuration);
        }
    }

    private float GetAverageSideSlip()
    {
        WheelHit hit;
        float totalSlip = 0f;
        int count = 0;

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

    private void UpdateWheelVisuals(WheelCollider collider, Transform wheelTransform, bool isFrontWheel)
    {
        if (wheelTransform == null) return;

        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;

        // Крутим колесо по вращению RPM
        float rpm = collider.rpm;
        if (Mathf.Abs(_rb.velocity.magnitude) < 0.05f)
            rpm = 0f;

        float rotationThisFrame = rpm * 6f * Time.deltaTime;
        rot *= Quaternion.Euler(rotationThisFrame, 0f, 0f);

        wheelTransform.rotation = rot;
    }

    // --- КНОПКА ПЕРЕВОРОТА МАШИНЫ ---
    public void FlipCar()
    {
        // Ставим машину обратно на колёса и обнуляем скорость
        transform.position += Vector3.up * 1.5f; // приподнимаем
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        _rb.velocity        = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
}
