using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    /*
     * Any controllable vehicle inherits from this class
     */

    [Header("Vehicle Parameters")]
    [SerializeField] protected float baseWeightKG = 1000.0f;
    [SerializeField] protected Transform centerOfMassTransform = null;
    [SerializeField] protected float defualtVerticalTrim = 0.0f;
    [SerializeField] protected float defualtHorizontalTrim = 0.0f;
    [Space]

    [Header("Telemetry and information")]
    public float pitch;
    public float roll;
    [Space]

    // Inputs that are changed through external MonoBehaviours (VehicleController)
    protected float _inputRoll = 0;
    protected float _inputPitch = 0;
    protected float _inputYaw = 0;
    protected float _inputBrake = 0;
    protected float _inputThrottle = 0;
    protected float verticalTrim = 0.0f;
    protected float horizontalTrim = 0.0f;

    protected Rigidbody rigid;

    // General Vehicle systems
    protected List<Engine> engines = new List<Engine>();
    protected List<Wheel> wheels = new List<Wheel>();
    protected ParkingBrake parkingBrake = null;
    protected RetractableWheels retractableWheels = null;
    
    protected List<ControlSurface> controlSurfaces = new List<ControlSurface>();
    protected List<ControlSurface> pitchControlSurfaces = new List<ControlSurface>();
    protected List<ControlSurface> leftRollControlSurfaces = new List<ControlSurface>();
    protected List<ControlSurface> rightRollControlSurfaces = new List<ControlSurface>();
    protected List<ControlSurface> yawControlSurfaces = new List<ControlSurface>();
    // End of General Vehicle Systems

    protected virtual void InitializeVehicle()
    {
        // Setup vehicle, here we get all vehicle systems and tie them to the vehicle based on what they do.
        // First setup vehicle systems
        engines = GetComponentsInChildren<Engine>().ToList();
        wheels = GetComponentsInChildren<Wheel>().ToList();
        parkingBrake = GetComponentInChildren<ParkingBrake>();
        retractableWheels = GetComponentInChildren<RetractableWheels>();
        SetupControlSurfaces();

        // Initial vehicle mass should not take anything into consideration
        rigid = GetComponent<Rigidbody>();
        rigid.mass = baseWeightKG;

        // If we have a center of mass transform then set the vehicles rigidbody centerOfMass vector to the its local position.
        // A vehicle should always have a center of mass transform!
        if (centerOfMassTransform)
        {
            rigid.centerOfMass = centerOfMassTransform.transform.localPosition;
        }
        else
        {
            Debug.LogWarning(name + ": Vehicle missing center of mass transform!");
        }
    }
    protected virtual void VehicleUpdate()
    {
        // Clamp trim, there has to be a better way to do this.
        verticalTrim = Mathf.Clamp(verticalTrim, -1f, 1f);
        horizontalTrim = Mathf.Clamp(horizontalTrim, -1f, 1f);

        // Update telemetry information
        GetPitch();
        GetRoll();

        // Control Surface deflection inputs are given to the vehicle through inputs from SendAxisInputs()
        SetControlSurfacesDeflection(pitchControlSurfaces, _inputPitch);
        SetControlSurfacesDeflection(leftRollControlSurfaces, -_inputRoll);
        SetControlSurfacesDeflection(rightRollControlSurfaces, _inputRoll);
        SetControlSurfacesDeflection(yawControlSurfaces, _inputYaw);

        // Wheel Steering and Brake inputs
        for (int i = 0; i < wheels.Count; i++)
        {
            wheels[i].SetSteerInput(_inputYaw);
            wheels[i].SetBrakeInput(_inputBrake);
        }

        // Engine throttle input handling
        for (int i = 0; i < engines.Count; i++)
        {
            engines[i].SetThrottle(_inputThrottle);
        }
    }
    public virtual void SendAxisInputs(float y, float x, float z, float brake, float throttle)
    {
        _inputPitch = Mathf.Clamp(y + verticalTrim, -1.0f, 1.0f);
        _inputRoll = Mathf.Clamp(x + horizontalTrim, -1.0f, 1.0f);
        _inputYaw = Mathf.Clamp(z, -1.0f, 1.0f);
        _inputBrake = Mathf.Clamp01(brake);
        _inputThrottle = Mathf.Clamp01(throttle);
    }
    public virtual float GetAxisInput(Enums.AxisInput aI)
    {
        switch (aI)
        {
            case Enums.AxisInput.PITCH:
                return _inputPitch;
            case Enums.AxisInput.ROLL:
                return _inputRoll;
            case Enums.AxisInput.YAW:
                return _inputYaw;
            case Enums.AxisInput.THROTTLE:
                return _inputThrottle;
            case Enums.AxisInput.BRAKE:
                return _inputBrake;
        }

        // If we get here, there is an axis we did not set a case for.
        Debug.Log("Tried to get an axis input that is not implemented");
        return -2f;
    }
    public virtual void SendKeyInput(KeyCode key)
    {
        switch (key)
        {
            case Enums.VEHICLE_GEAR_TOGGLE:
                ToggleExtendableWheels();
                break;
            case Enums.VEHICLE_ENGINE_TOGGLE:
                ToggleEngines();
                break;
            case Enums.VEHICLE_PARKING_BRAKE_TOGGLE:
                ToggleParkingBrake();
                break;
            case Enums.VEHICLE_TRIM_VERTICAL_INCREASE:
                ChangeTrim(0.02f, Enums.Axis.VERTICAL);
                break;
            case Enums.VEHICLE_TRIM_VERTICAL_DECREASE:
                ChangeTrim(-0.02f, Enums.Axis.VERTICAL);
                break;
        }
    }
    protected void SetControlSurfacesDeflection(List<ControlSurface> surfaces, float value)
    {
        if (surfaces.Count > 0)
        {
            for (int i = 0; i < surfaces.Count; i++)
            {
                surfaces[i].targetDeflection = value;
            }
        }
    }
    private void ToggleEngines()
    {
        for (int i = 0; i < engines.Count; i++)
        {
            engines[i].ToggleIgnition();
        }
    }
    private void ToggleParkingBrake()
    {
        if (parkingBrake)
        {
            parkingBrake.ToggleParkingBrake();
        }
    }
    private void ToggleExtendableWheels()
    {
        if (retractableWheels)
        {
            retractableWheels.ToggleExtendableWheels();
        }
        else
        {
            Debug.Log("No retractable gear!");
        }
    }
    private void ChangeTrim(float amount, Enums.Axis axis)
    {
        if (axis == Enums.Axis.VERTICAL)
        {
            verticalTrim += amount;
            verticalTrim = Mathf.Clamp01(verticalTrim);
        }
        if (axis == Enums.Axis.HORIZONTAL)
        {
            horizontalTrim += amount;
            horizontalTrim = Mathf.Clamp01(horizontalTrim);
        }
    }
    public void SetTrim(Enums.Axis axis, float trim)
    {
        if (axis == Enums.Axis.VERTICAL)
        {
            verticalTrim = trim;
        }
        if (axis == Enums.Axis.HORIZONTAL)
        {
            horizontalTrim = trim;
        }
    }
    public float GetTrim(Enums.Axis axis)
    {
        if (axis == Enums.Axis.VERTICAL) 
        { 
            return verticalTrim;
        }
        if (axis == Enums.Axis.HORIZONTAL)
        {
            return horizontalTrim;
        }
        return 0f;
    }
    public List<VehicleSystem> GetAllVehicleSystems()
    {
        return gameObject.GetComponentsInChildren<VehicleSystem>().ToList();
    }
    private void SetupControlSurfaces()
    {
        // Make sure all control surface lists are clear before we add new ones
        controlSurfaces.Clear();
        pitchControlSurfaces.Clear();
        leftRollControlSurfaces.Clear();
        rightRollControlSurfaces.Clear();
        yawControlSurfaces.Clear();

        controlSurfaces = GetComponentsInChildren<ControlSurface>().ToList();
        // Setup control surfaces based on their axis, this is needed for any flyable vehicle to have controllable surfaces
        for (int i = 0; i < controlSurfaces.Count; i++)
        {
            if (controlSurfaces[i])
            {
                if (controlSurfaces[i].controlType == ControlSurface.ControlType.PITCH)
                {
                    // Check if we are infront or behind the wings center and set the invert accordingly 
                    // Frontal elevators will be inverted
                    controlSurfaces[i].inverted = controlSurfaces[i].transform.localPosition.z > 0;
                    pitchControlSurfaces.Add(controlSurfaces[i]);
                }
                else if (controlSurfaces[i].controlType == ControlSurface.ControlType.ROLL)
                {
                    // Left Ailerons will have their local X position negative
                    if (controlSurfaces[i].transform.localPosition.x < 0)
                    {
                        leftRollControlSurfaces.Add(controlSurfaces[i]);
                    }
                    else
                    {
                        rightRollControlSurfaces.Add(controlSurfaces[i]);
                    }
                }
                else if (controlSurfaces[i].controlType == ControlSurface.ControlType.YAW)
                {
                    yawControlSurfaces.Add(controlSurfaces[i]);
                }
            }
        }
    }

    #region CALCULATIONS
    public float CalculatePitchG()
    {
        if (!rigid)
        {
            rigid = GetComponent<Rigidbody>();
        }
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
    public Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
        return point + planeNormal * distance;
    }
    public float SignedAngle(Vector3 v1, Vector3 v2, Vector3 normal)
    {
        Vector3 perp = Vector3.Cross(normal, v1);
        float angle = Vector3.Angle(v1, v2);
        angle *= Mathf.Sign(Vector3.Dot(perp, v2));
        return angle;
    }
    public float GroundSpeedKnots()
    {
        if (!rigid)
        {
            rigid = GetComponent<Rigidbody>();
        }
        const float msToKnots = 1.94384f;
        return rigid.velocity.magnitude * msToKnots;
    }
    public float VerticalSpeedFeetPerMinute()
    {
        if (!rigid)
        {
            rigid = GetComponent<Rigidbody>();
        }
        return rigid.velocity.y * 3.28084f * 60;
    }
    public float GetPitch()
    {
        Vector3 pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.forward);
        pitch = SignedAngle(transform.forward, pos, transform.right);
        return pitch;
    }
    public float GetRoll()
    {
		Vector3 pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.forward);	
		pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.right);
		roll = SignedAngle(transform.right, pos, transform.forward);
        return roll;
    }
    #endregion
}