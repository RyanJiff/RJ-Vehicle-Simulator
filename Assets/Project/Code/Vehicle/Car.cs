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
		// After here we add any custom inputs
	}
	protected override void InitializeVehicle()
	{
		base.InitializeVehicle();
	}
}
