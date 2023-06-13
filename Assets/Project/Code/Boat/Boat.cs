using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Boat: Vehicle
{
	/*
	 * Main boat driver class
	 */

	// Engines
	private List<WaterEngine> engines = new List<WaterEngine>();

	private void Awake()
	{
		InitializeVehicle();
	}
	private void Start()
	{
		if (engines.Count == 0)
			Debug.LogWarning(name + ": Boat missing engine!");
	}
	void Update()
	{
		// Engine throttle input and yaw handling
		for (int i = 0; i < engines.Count; i++)
		{
			engines[i].SetThrottle(_inputThrottle);
			engines[i].SetYaw(_inputRoll);
		}
	}

	#region INPUTS
	public override void SendKeyInput(KeyCode key)
	{
		// Boats have no inputs other than engine, lets keep them simple for now.
		if(key == KeyCode.G)
        {
			for (int i = 0; i < engines.Count; i++)
			{
				engines[i].ToggleReverse();
			}
		}
	}

	#endregion
	protected override void InitializeVehicle()
	{
		base.InitializeVehicle();

		engines = GetComponentsInChildren<WaterEngine>().ToList();
	}
}
