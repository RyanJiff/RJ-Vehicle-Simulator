using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Airplane : Vehicle
{
	/*
	 * Main airplane driver class
	 */
	[Header("Flaps")]
	[SerializeField] private List<float> flapLevels = new List<float>();
	[Space]

	[Header("Trim")]
	[SerializeField] private float defualtElevatorTrim = 0.0f;
	[Space]

	[Header("Brakes")]
	[SerializeField] private WheelCollider[] brakeWheels;
	[SerializeField] private int brakeTorque;
	[Space]

	[Header("Steering")]
	[SerializeField] private WheelCollider[] steeringWheels;
	[SerializeField] private float maxSteerAngle = 20;
	[SerializeField] private bool invertedSteering = false;
	[Space]

	[Header("Center Of Mass")]
	[SerializeField] private Transform centerOfMassTransform;
	[Space]

	// Control surfaces
	private List<ControlSurface> controlSurfaces = new List<ControlSurface>();
	private List<ControlSurface> elevators = new List<ControlSurface>();
	private List<ControlSurface> leftAilerons = new List<ControlSurface>();
	private List<ControlSurface> rightAilerons = new List<ControlSurface>();
	private List<ControlSurface> rudders = new List<ControlSurface>();
	
	// Flaps
	private List<ControlSurface> flaps = new List<ControlSurface>();
	private int currentFlapLevel;
	
	// Engines
	private List<AirplaneEngine> engines = new List<AirplaneEngine>();
	
	// Fuel Tanks
	private List<FuelTank> fuelTanks = new List<FuelTank>();

	// Landing gear (retract/extend system)
	private LandingGear landingGear;

	private bool yawDefined = false;

	private void Awake()
	{
		InitializeAirplane();
	}
	private void Start()
	{
		if (elevators.Count == 0)
			Debug.LogWarning(name + ": Airplane missing elevator!");
		if (leftAilerons.Count == 0)
			Debug.LogWarning(name + ": Airplane missing left aileron!");
		if (rightAilerons.Count == 0)
			Debug.LogWarning(name + ": Airplane missing right aileron!");
		if (rudders.Count == 0)
			Debug.LogWarning(name + ": Airplane missing rudder!");
		if (engines.Count == 0)
			Debug.LogWarning(name + ": Airplane missing engine!");

		try
		{
			Input.GetAxis("Yaw");
			yawDefined = true;
		}
		catch (ArgumentException e)
		{
			Debug.LogWarning(e);
			Debug.LogWarning(name + ": \"Yaw\" axis not defined in Input Manager. Rudder will not work correctly!");
		}
	}
	void Update()
	{
		// Control surfaces inputs are handled first
		SetControlSurfacesListDeflection(elevators, _inputPitch + verticalTrim);
		SetControlSurfacesListDeflection(leftAilerons, -_inputRoll);
		SetControlSurfacesListDeflection(rightAilerons, _inputRoll);
		
		// If yaw is not defined there is an issue with the input manager settings!
		if (yawDefined)
		{
			SetControlSurfacesListDeflection(rudders, _inputYaw);

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
		}

		// Clamp elevator trim, there has to be a better way to do this.
		verticalTrim = Mathf.Clamp(verticalTrim, -0.8f, 0.8f);

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

		// Engine throttle input handling
		for (int i = 0; i < engines.Count; i++)
		{
			engines[0].SetThrottle(_inputThrottle);
		}
	}

    #region INPUTS
    public override void SendKeyInput(KeyCode key)
    {
        switch (key)
        {
			case KeyCode.G:
				ToggleGear();
				break;
			case KeyCode.I:
				ToggleEngine();
				break;
			case KeyCode.Equals:
				ChangeTrim(0.02f, Enums.Axis.VERTICAL);
				break;
			case KeyCode.Minus:
				ChangeTrim(-0.02f, Enums.Axis.VERTICAL);
				break;
			case KeyCode.Alpha3:
				currentFlapLevel = Mathf.Clamp(currentFlapLevel - 1, 0, Mathf.Max(0, flapLevels.Count - 1));
				foreach (ControlSurface c in flaps)
                {
					c.targetDeflection = flapLevels[currentFlapLevel];
                }
				break;
			case KeyCode.Alpha4:
				currentFlapLevel = Mathf.Clamp(currentFlapLevel + 1, 0, Mathf.Max(0, flapLevels.Count - 1));
				foreach (ControlSurface c in flaps)
				{
					c.targetDeflection = flapLevels[currentFlapLevel];
				}
				break;
		}
    }
    private void SetControlSurfacesListDeflection(List<ControlSurface> surfaces, float value)
    {
		if(surfaces.Count > 0)
        {
			for(int i = 0;i < surfaces.Count; i++)
            {
				surfaces[i].targetDeflection = value;
            }
        }
    }
	public void ToggleEngine()
	{
		for (int i = 0; i < engines.Count; i++)
		{
			engines[i].ToggleIgnition();
		}
	}
	public void ToggleGear()
    {
		if (landingGear)
        {
			landingGear.ToggleGear();
        }
        else
        {
			Debug.Log("No retractable gear!");
        }
    }
	#endregion
	void InitializeAirplane()
	{
		verticalTrim = defualtElevatorTrim;

		controlSurfaces = GetComponentsInChildren<ControlSurface>().ToList();
		engines = GetComponentsInChildren<AirplaneEngine>().ToList();
		rigid = GetComponent<Rigidbody>();
		landingGear = GetComponent<LandingGear>();
		fuelTanks = GetFuelTanks();

		if (centerOfMassTransform)
		{
			rigid.centerOfMass = centerOfMassTransform.transform.localPosition;
		}
		else
		{
			Debug.LogWarning(name + ": Airplane missing center of mass transform!");
		}

		// Setup control surfaces based on their axis, this is needed for the airplane to have controllable surfaces
		for (int i = 0; i < controlSurfaces.Count; i++)
		{
			if (controlSurfaces[i])
			{
				if (controlSurfaces[i].controlType == ControlSurface.ControlType.PITCH)
				{
					// Check if we are infront or behind the wings center and set the invert accordingly 
					// Frontal elevators will be inverted
					controlSurfaces[i].inverted = controlSurfaces[i].transform.localPosition.z > 0;
					elevators.Add(controlSurfaces[i]);
				}
				else if (controlSurfaces[i].controlType == ControlSurface.ControlType.ROLL)
				{
					// Left Ailerons will have their local X position negative
					if (controlSurfaces[i].transform.localPosition.x < 0)
					{
						leftAilerons.Add(controlSurfaces[i]);
					}
					else
					{
						rightAilerons.Add(controlSurfaces[i]);
					}
				}
				else if (controlSurfaces[i].controlType == ControlSurface.ControlType.YAW)
				{
					rudders.Add(controlSurfaces[i]);
				}
				else if (controlSurfaces[i].controlType == ControlSurface.ControlType.FLAPS)
				{
					flaps.Add(controlSurfaces[i]);
				}
			}
		}

	}
	public List<FuelTank> GetFuelTanks()
	{
		fuelTanks = GetComponentsInChildren<FuelTank>().ToList();
		return fuelTanks;
	}
}
