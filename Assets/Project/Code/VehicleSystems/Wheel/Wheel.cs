using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : VehicleSystem
{
    /*
     * All wheel colliders on vehicles should have a Wheel script 
     */

    [Header("Wheel Settings")]
    [SerializeField] private bool isPowerWheel = false;
    [SerializeField] private bool isSteerWheel = false;
    [SerializeField] private bool invertSteering = false;
    [SerializeField] private float maxSteerAngle = 40f;
    [SerializeField] private bool isBrakeWheel = false;
    [SerializeField] private bool hasParkingBrake = false;
    [SerializeField] private float maxBrakeForce = 500;

    private WheelCollider wheelCollider;
    private float steerInput = 0;
    private float brakeInput = 0;
    private bool parkingBrakeOn = false;

    // DEBUG
    public float brakeForce = 0;
    public float torqueForce = 0;
    public float steerAngle = 0;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelCollider.motorTorque = 0.000001f;
    }
    private void Update()
    {
        wheelCollider.steerAngle = maxSteerAngle * steerInput;
        if (!hasParkingBrake)
        {
            wheelCollider.brakeTorque = maxBrakeForce * brakeInput;
        }
        else if(hasParkingBrake && parkingBrakeOn)
        {
            wheelCollider.brakeTorque = maxBrakeForce;
        }

        // DEBUG
        brakeForce = wheelCollider.brakeTorque;
        torqueForce = wheelCollider.motorTorque;
        steerAngle = wheelCollider.steerAngle;
    }
    public void SetSteerInput(float s)
    {
        if (isSteerWheel)
        {
            s = Mathf.Clamp(s, -1.0f, 1.0f);
            if (invertSteering)
            {
                s *= -1;
            }
            steerInput = s;
        }
        else
        {
            steerInput = 0;
        }
    }
    public void SetBrakeInput(float b)
    {
        // We have a minimum threshold for braking, otherwise funny things happen
        if (isBrakeWheel && b >= 0.2f)
        {
            b = Mathf.Clamp01(b);
            brakeInput = b;
        }
        else
        {
            brakeInput = 0;
        }
    }
    public void SetTorque(float t)
    {
        if (isPowerWheel)
        {
            wheelCollider.motorTorque = t + 0.000001f;
        }
        else
        {
            wheelCollider.motorTorque = 0.000001f;
        }
    }
    public bool GetSteerEnabled()
    {
        return isSteerWheel;
    }
    public bool GetPowerEnabled()
    {
        return isPowerWheel;
    }
    public void SetParkingBrake(bool b)
    {
        parkingBrakeOn = b;
    }
}
