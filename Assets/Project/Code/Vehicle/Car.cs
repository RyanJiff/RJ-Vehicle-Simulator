using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Vehicle
{
	/*
	 * Main car driver class
	 */

	private void Awake()
	{
		InitializeVehicle();
	}
	void Update()
	{
		VehicleUpdate();
	}
	public override void SendKeyInput(KeyCode key)
	{
		base.SendKeyInput(key);
	}
	protected override void InitializeVehicle()
	{
		base.InitializeVehicle();
	}
}
