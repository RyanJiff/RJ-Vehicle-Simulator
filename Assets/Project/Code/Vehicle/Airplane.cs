using System.Collections.Generic;
using UnityEngine;

public class Airplane : Vehicle
{
	/*
	 * Main airplane driver class
	 */

	protected List<ControlSurface> flapsControlSurfaces = new List<ControlSurface>();
	protected float _inputFlaps = 0f;

	private void Awake()
	{
		InitializeVehicle();

		// Init flaps
		flapsControlSurfaces.Clear();
		for (int i = 0; i < controlSurfaces.Count; i++)
		{
			if (controlSurfaces[i])
			{
				if (controlSurfaces[i].controlType == ControlSurface.ControlType.FLAPS)
				{
					flapsControlSurfaces.Add(controlSurfaces[i]);
				}
			}
		}
	}
	void Update()
	{
		VehicleUpdate();
		
		// Flap inputs
		if (Input.GetKey(Enums.AIRPLANE_FLAPS_UP))
		{
			_inputFlaps -= 0.5f * Time.deltaTime;
		}
		else if (Input.GetKey(Enums.AIRPLANE_FLAPS_DOWN))
		{
			_inputFlaps += 0.5f * Time.deltaTime;
		}

		_inputFlaps = Mathf.Clamp01(_inputFlaps);
		SetControlSurfacesDeflection(flapsControlSurfaces, _inputFlaps);

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
