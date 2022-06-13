using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngineSounds : MonoBehaviour
{
    public float minSpeed, maxSpeed;
    private float _currentSpeed;
    
    private Rigidbody _carRb;
    private AudioSource _carAudio;

    public float minPitch, maxPitch;
    private float _pitchFromCar;

    private void Start()
    {
        _carAudio = GetComponent<AudioSource>();
        _carRb = GetComponent<Rigidbody>();
    }

    private void EngineSound()
    {
        _currentSpeed = _carRb.velocity.magnitude;
        _pitchFromCar = _carRb.velocity.magnitude / 50f;
    }
}
