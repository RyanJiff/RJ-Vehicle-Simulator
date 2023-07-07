using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
[RequireComponent(typeof(WheelVisual))]
public class Wheel : VehicleSystem
{
    /*
     * All wheel colliders on vehicles should have a Wheel script as it is used by other vehicle systems to control the wheel collider.
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

    protected override void VehicleSystemAwake()
    {
        base.VehicleSystemAwake();

        wheelCollider = GetComponent<WheelCollider>();
        wheelCollider.motorTorque = 0.000001f;
    }
    protected override void VehicleSystemUpdate()
    {
        base.VehicleSystemUpdate();

        // Set steer angle first
        wheelCollider.steerAngle = maxSteerAngle * steerInput;

        // TODO: I wrote this for clarity more than effeciency, with some very basic boolean algebra this can be made more effecient.
        // The braking of the wheel is different for parking brake enabled and non parking brake wheels.
        if (hasParkingBrake)
        {
            if (parkingBrakeOn)
            {
                // We have a parking brake and it is enabled. Brake the wheel.
                wheelCollider.brakeTorque = maxBrakeForce;
            }
            else if (!parkingBrakeOn && isBrakeWheel)
            {
                // We have a parking brake and it is off and we are a braking wheel. Use normal brake input.
                wheelCollider.brakeTorque = brakeInput * maxBrakeForce;
            }
            else if (!parkingBrakeOn && !isBrakeWheel)
            {
                // We have a parking brake but we are not a braking wheel. Do not brake unless the parking brake is on.
                wheelCollider.brakeTorque = 0;
            }
        }
        // Here if we do not have a parking brake we just brake if we are a brake wheel
        else
        {
            if (isBrakeWheel)
            {
                // We have brakes. Brake normally.
                wheelCollider.brakeTorque = brakeInput * maxBrakeForce;
            }
            else
            {
                // We have no parking brake or brakes. Never brake.
                wheelCollider.brakeTorque = 0;
            }
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
