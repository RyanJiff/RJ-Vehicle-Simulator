using UnityEngine;
using System.Collections.Generic;

public class GearVisuals : VehicleSystem
{
	/*
	 * Written by Brian Hernandez. 
	 * TODO: Need to write a new visual system for wheels
	 */

	public WheelCollider[] wheels;
	public Transform wheelVisualizerPrefab;

	private Dictionary<Transform, WheelCollider> visualToWheelMap;

	protected override void VehicleSystemAwake()
	{
		base.VehicleSystemAwake();

		visualToWheelMap = new Dictionary<Transform, WheelCollider>();
	}

    // Use this for initialization
    protected override void VehicleSystemStart()
    {
        base.VehicleSystemStart();

		if (wheelVisualizerPrefab != null)
		{
			// Create a cylinder and associate each cylinder with a wheel.
			foreach (WheelCollider wheel in wheels)
			{
				Transform visual = Instantiate(wheelVisualizerPrefab, wheel.transform);
				visualToWheelMap.Add(visual, wheel);
			}
		}
	}
	protected override void VehicleSystemUpdate()
	{
		base.VehicleSystemUpdate();

		if (visualToWheelMap.Count > 0)
		{
			Vector3 pos;
			Quaternion rot;

			foreach (var visualWheel in visualToWheelMap)
			{
				visualWheel.Value.GetWorldPose(out pos, out rot);
				visualWheel.Key.position = pos;
				visualWheel.Key.rotation = rot;
			}
		}
	}
}
