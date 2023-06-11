using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Airplane : Vehicle, IControllable
{
	/*
	 * Main airplane driver class
	 */

	[Header("All control surfaces")]
	[SerializeField] private List<ControlSurface> controlSurfaces = new List<ControlSurface>();
	[Space]

	[Header("Control Surfaces")]
	private List<ControlSurface> elevators = new List<ControlSurface>();
	private List<ControlSurface> leftAilerons = new List<ControlSurface>();
	private List<ControlSurface> rightAilerons = new List<ControlSurface>();
	private List<ControlSurface> rudders = new List<ControlSurface>();
	[SerializeField] private float defualtElevatorTrim = 0.0f;
	[Space]

	[Header("Flaps")]
	private List<ControlSurface> flaps = new List<ControlSurface>();
	[SerializeField] private List<float> flapLevels = new List<float>();
	private int currentFlapLevel;
	[Space]

	[Header("Engines")]
	private List<AirplaneEngine> engines = new List<AirplaneEngine>();
	[Space]

	[Header("Fuel Tanks")]
	private List<FuelTank> fuelTanks = new List<FuelTank>();
	[Space]

	[Header("Landing Gear")]
	private LandingGear landingGear;
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

	[Header("Center of mass ")]
	[SerializeField] private Transform centerOfMassTransform;
	[Space]

	private Rigidbody rigid;
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

		// Pitch
		Vector3 pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.forward);
		pitch = SignedAngle(transform.forward, pos, transform.right);

		// Roll
		pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.right);
		roll = SignedAngle(transform.right, pos, transform.forward);
	}

    #region CALCULATIONS
    private float CalculatePitchG()
	{
		// Angular velocity is in radians per second.
		Vector3 localVelocity = transform.InverseTransformDirection(rigid.velocity);
		Vector3 localAngularVel = transform.InverseTransformDirection(rigid.angularVelocity);

		// Local pitch velocity (X) is positive when pitching down.

		// Radius of turn = velocity / angular velocity
		float radius = (Mathf.Approximately(localAngularVel.x, 0.0f)) ? float.MaxValue : localVelocity.z / localAngularVel.x;

		// The radius of the turn will be negative when in a pitching down turn.

		// Force is mass * radius * angular velocity^2
		float verticalForce = (Mathf.Approximately(radius, 0.0f)) ? 0.0f : (localVelocity.z * localVelocity.z) / radius;

		// Express in G (Always relative to Earth G)
		float verticalG = verticalForce / -9.81f;

		// Add the planet's gravity in. When the up is facing directly up, then the full
		// force of gravity will be felt in the vertical.
		verticalG += transform.up.y * (Physics.gravity.y / -9.81f);

		return verticalG;
	}

	Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
		return point + planeNormal * distance;
	}

	float SignedAngle(Vector3 v1, Vector3 v2, Vector3 normal)
	{
		Vector3 perp = Vector3.Cross(normal, v1);
		float angle = Vector3.Angle(v1, v2);
		angle *= Mathf.Sign(Vector3.Dot(perp, v2));
		return angle;
	}
    public float GroundSpeed()
	{
		const float msToKnots = 1.94384f;
		return rigid.velocity.magnitude * msToKnots;
	}
	public float VerticalSpeed()
	{
		return rigid.velocity.y * 3.28084f * 60;
	}
    #endregion

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
			engines[0].ToggleIgnition();
		}
	}
	/// <summary>
	/// Toggle gear if we have one
	/// </summary>
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
	/// <summary>
	/// Set all control surfaces and references to components and initialize them
	/// </summary>
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

	/// <summary>
	/// Return amount of fuel in all tanks
	/// </summary>
	public float GetFuelAmount()
	{
		float fuelAmount = 0f;
		foreach (FuelTank f in fuelTanks)
		{
			fuelAmount += f.GetCurrentFuelAmount();
		}
		return fuelAmount;
	}

	/// <summary>
	/// Setup all the fuel tanks for the airplane
	/// </summary>
	public List<FuelTank> GetFuelTanks()
	{
		fuelTanks = GetComponentsInChildren<FuelTank>().ToList();
		return fuelTanks;
	}

	/// <summary>
	/// Get total fuel amount in all tanks 
	/// </summary>
	/// <param name="onlyUsableTanks">do we add the total fuel of usable tanks only?</param>
	public float GetTotalFuelAmount(bool onlyUsableTanks)
	{
		float fuelAmount = 0f;
		foreach (FuelTank f in fuelTanks)
		{
			if (onlyUsableTanks)
			{
				if (f.IsFuelValveOpen())
				{
					fuelAmount += f.GetCurrentFuelAmount();
				}
			}
			else
			{
				fuelAmount += f.GetCurrentFuelAmount();
			}
		}
		return fuelAmount;
	}

	/// <summary>
	/// The current flap level, 0 for fully retracted, 1 for fully extended. -1 if airplane has no flaps setup
	/// </summary>
	public float GetCurrentFlapLevel()
    {
		if (flapLevels.Count > 0)
		{
			return flapLevels[currentFlapLevel];
		}
		return -1f;
    }

	/// <summary>
	/// For airplanes this updates the mass of the airplane based on fuel tanks and location of the fuel
	/// </summary>
    public override void VehicleUpdate()
    {
        //TODO: Implement
    }
}
