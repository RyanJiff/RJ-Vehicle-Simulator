using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Airplane : Vehicle
{
	/*
	 * Main airplane driver class
	 */

	[Header("Brakes")]
	[SerializeField] private WheelCollider[] brakeWheels;
	[SerializeField] private int brakeTorque;
	[Space]

	[Header("Steering")]
	[SerializeField] private WheelCollider[] steeringWheels;
	[SerializeField] private float maxSteerAngle = 20;
	[SerializeField] private bool invertedSteering = false;
	

	private void Awake()
	{
		InitializeVehicle();
	}
	void Update()
	{
		VehicleUpdate();
	}

    protected override void VehicleUpdate()
    {
        base.VehicleUpdate();
        
        // Steering inputs are handled first
        foreach (WheelCollider c in steeringWheels)
        {
            if (!invertedSteering)
            {
                c.steerAngle = _inputYaw * maxSteerAngle;
            }
            else
            {
                c.steerAngle = _inputYaw * -maxSteerAngle;
            }
        }
        // Brakes inputs are on and off, need to make a float
        if (_inputBrake)
        {
            foreach (WheelCollider w in brakeWheels)
            {
                w.brakeTorque = brakeTorque;
            }
        }
        else
        {
            foreach (WheelCollider w in brakeWheels)
            {
                w.brakeTorque = 0;
                // We have to give the motor some torque when not braking so it does not lock up and go into sleep state.
                // This is not good
                w.motorTorque = 0.00000001f;
            }
        }
    }

    #region INPUTS
    public override void SendKeyInput(KeyCode key)
    {
		base.SendKeyInput(key);
    }
	#endregion
	protected override void InitializeVehicle()
	{
		base.InitializeVehicle();
	}
}
