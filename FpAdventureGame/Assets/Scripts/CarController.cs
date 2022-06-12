using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string Horizontal = "Horizontal", Vertical = "Vertical"; 

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
    }
}
