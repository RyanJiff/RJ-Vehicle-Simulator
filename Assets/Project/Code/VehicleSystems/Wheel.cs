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
    [SerializeField] private bool powerEnabled = false;
    [SerializeField] private bool steerEnabled = false;
    [SerializeField] private bool invertSteering = false;
    [SerializeField] private float maxSteerAngle = 40f;
    [SerializeField] private bool brakeWheel = false;
    [SerializeField] private float maxBrakeForce = 500;

    private WheelCollider wheelCollider;
    [SerializeField] private float steerInput = 0;
    [SerializeField] private float brakeInput = 0;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelCollider.motorTorque = 0.000001f;
    }
    private void Update()
    {
        wheelCollider.steerAngle = maxSteerAngle * steerInput;
        wheelCollider.brakeTorque = maxBrakeForce * brakeInput;
    }
    public void SetSteerInput(float s)
    {
        if (steerEnabled)
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
        if (brakeWheel && b >= 0.2f)
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
        if (powerEnabled)
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
        return steerEnabled;
    }
    public bool GetPowerEnabled()
    {
        return powerEnabled;
    }
}
