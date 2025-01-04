using UnityEngine;

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
    public float motorPower       = 1500f; 
    public float maxSteerAngle    = 25f;  
    public float brakeForce       = 1500f; 
    public float handbrakeForce   = 3000f;  
    public float driftFactor      = 0.5f; 
    public float autoSteerSpeed   = 2f;  
    public float driftThreshold   = 10f;  // Порог бокового скольжения
    public float autoSteerMinSpeed= 5f;   // Скорость для автоподруливания

    public bool CanDrive { get; set; } = true; 
    // Свойство, определяющее, можем ли мы сейчас управлять машиной.
    // Например, при окончании уровня GameManager установит CanDrive = false.

    private Rigidbody _rb;
    private float _currentSteerAngle;
    private bool  _isDrifting;

    // Исходная боковая фрикция задних колёс
    private WheelFrictionCurve _rearLeftOriginalFriction;
    private WheelFrictionCurve _rearRightOriginalFriction;

    // Чтобы при желании считывать из другого класса
    public bool IsDrifting => _isDrifting;
    // Можно сделать свойство для скорости:
    public float CurrentSpeedKmh => _rb.velocity.magnitude * 3.6f;

    // События (например, о начале/конце дрифта):
    public delegate void DriftEventHandler(bool started, float driftDuration);
    public event DriftEventHandler OnDriftEvent;

    private bool  _wasDriftingLastFrame;
    private float _driftStartTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rearLeftOriginalFriction  = rearLeftWheelCollider.sidewaysFriction;
        _rearRightOriginalFriction = rearRightWheelCollider.sidewaysFriction;
    }

    private void Update()
    {
        if (CanDrive)
        {
            // Игровой ввод
            float verticalInput   = Input.GetAxis("Vertical");   
            float horizontalInput = Input.GetAxis("Horizontal");

            HandleMotor(verticalInput);
            HandleSteering(horizontalInput);
            HandleBrakes();
        }
        else
        {
            // Блокируем колеса
            frontLeftWheelCollider.brakeTorque    = 9999f;
            frontRightWheelCollider.brakeTorque   = 9999f;
            rearLeftWheelCollider.brakeTorque     = 9999f;
            rearRightWheelCollider.brakeTorque    = 9999f;
        }

        // Отслеживаем дрифт (начало/конец)
        HandleDriftTimeCounting();

        UpdateWheelVisuals(frontLeftWheelCollider,  frontLeftWheelTransform,  true);
        UpdateWheelVisuals(frontRightWheelCollider, frontRightWheelTransform, true);
        UpdateWheelVisuals(rearLeftWheelCollider,   rearLeftWheelTransform,   false);
        UpdateWheelVisuals(rearRightWheelCollider,  rearRightWheelTransform,  false);

        _wasDriftingLastFrame = _isDrifting;
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

        float slip = GetAverageSideSlip();
        _isDrifting = Mathf.Abs(slip) > driftThreshold;

        if (_isDrifting)
        {
            // Уменьшаем фрикцию (дрифт)
            AdjustDriftFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction, driftFactor);
            AdjustDriftFriction(rearRightWheelCollider, _rearRightOriginalFriction,driftFactor);

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
            ResetFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction);
            ResetFriction(rearRightWheelCollider, _rearRightOriginalFriction);
        }

        // Применяем угол руля (передние колёса)
        frontLeftWheelCollider.steerAngle  = _currentSteerAngle;
        frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    private void HandleBrakes()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            frontLeftWheelCollider.brakeTorque  = brakeForce;
            frontRightWheelCollider.brakeTorque = brakeForce;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque  = 0f;
            frontRightWheelCollider.brakeTorque = 0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rearLeftWheelCollider.brakeTorque  = handbrakeForce;
            rearRightWheelCollider.brakeTorque = handbrakeForce;

            AdjustDriftFriction(rearLeftWheelCollider,  _rearLeftOriginalFriction,  0.8f);
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
    }

    private void HandleDriftTimeCounting()
    {
        if (_isDrifting && !_wasDriftingLastFrame)
        {
            // Начало дрифта
            _driftStartTime = Time.time;
            OnDriftEvent?.Invoke(true, 0f);  // Событие: дрифт начался
        }
        else if (!_isDrifting && _wasDriftingLastFrame)
        {
            // Конец дрифта
            float driftEndTime = Time.time;
            float driftDuration = driftEndTime - _driftStartTime;

            OnDriftEvent?.Invoke(false, driftDuration);  // Событие: дрифт закончился
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

        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);

        wheelTransform.position = pos;

        // RPM
        float rpm = collider.rpm;
        if (Mathf.Abs(_rb.velocity.magnitude) < 0.05f)
        {
            rpm = 0f;
        }

        float rotationThisFrame = rpm * 6f * Time.deltaTime;
        rot *= Quaternion.Euler(rotationThisFrame, 0f, 0f);

        wheelTransform.rotation = rot;
    }
}
