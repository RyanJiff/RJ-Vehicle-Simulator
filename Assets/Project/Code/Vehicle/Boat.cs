using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Boat: Vehicle
{
	/*
	 * Main boat driver class
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
