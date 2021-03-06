using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAcceleration = 30f;
    public float brakeAcceleration = 50f;

    public float turnSensitivity = 1f;
    public float maxSteerAngle = 30f;

    public Vector3 centerOfMass;

    public List<Wheel> wheels;

    private float _moveInput;
    private float _steerInput;

    private Rigidbody _carRb;

    private void Start()
    {
        _carRb = GetComponent<Rigidbody>();
        _carRb.centerOfMass = centerOfMass;
    }

    private void Update()
    {
        GetInputs();
        WheelAnimations();
    }

    private void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    private void GetInputs()
    {
        _moveInput = Input.GetAxis("Vertical");
        _steerInput = Input.GetAxis("Horizontal");
    }

    private void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = _moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    private void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel != Axel.Front) continue;
            var steerAngle = _steerInput * turnSensitivity * maxSteerAngle;
            wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
        }
    }

    private void Brake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0f;
            }
        }

    }

    private void WheelAnimations()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }
    

    /*private const string Horizontal = "Horizontal", Vertical = "Vertical"; 

    private float _horizontalInput, _verticalInput, _currentBreakForce, _currentSteerAngle;

    private bool _isBreaking;

    [SerializeField] private float motorForce, breakForce, maxSteeringAngle;
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider;
    [SerializeField] private Transform frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    
    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis(Horizontal);
        _verticalInput = Input.GetAxis(Vertical);
        _isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = _verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = _verticalInput * motorForce;
        _currentBreakForce = _isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void HandleSteering()
    {
        _currentSteerAngle = maxSteeringAngle * _horizontalInput;
        frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheel);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheel);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheel);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheel);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pose;
        Quaternion rot;
        
        wheelCollider.GetWorldPose(out pose, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pose;
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = _currentBreakForce;
        frontLeftWheelCollider.brakeTorque = _currentBreakForce;
        rearRightWheelCollider.brakeTorque = _currentBreakForce;
        rearLeftWheelCollider.brakeTorque = _currentBreakForce;
    }*/
}
