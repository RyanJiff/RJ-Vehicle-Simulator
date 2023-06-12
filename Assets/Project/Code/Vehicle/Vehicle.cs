using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    /*
     * Any controllable vehicle inherits from this class
     */

    // Vehicle Type is used for controls and GUI
    public enum VehicleType { LAND, AIR, WATER}
    [SerializeField] VehicleType vehicleType = VehicleType.AIR;

    [Header("Vehicle Parameters")]
    [SerializeField] protected float baseWeightKG = 1000.0f;
    [Space]

    [Header("Telemetry and information")]
    public float pitch;
    public float roll;
    public float altitude;
    [Space]

    // Inputs that are changed through external MonoBehaviours (VehicleController)
    protected float _inputRoll = 0;
    protected float _inputPitch = 0;
    protected float _inputYaw = 0;
    protected bool _inputBrake = false;
    protected float _inputThrottle = 0;
    protected float verticalTrim = 0.0f;

    protected Rigidbody rigid;

    public virtual void SendAxisInputs(float y, float x, float z, bool brake, float throttle)
    {
        _inputPitch = Mathf.Clamp(y, -1.0f, 1.0f);
        _inputRoll = Mathf.Clamp(x, -1.0f, 1.0f);
        _inputYaw = Mathf.Clamp(z, -1.0f, 1.0f);
        // Brake input has to be made an axis, not a boolean
        _inputBrake = brake;
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
                // Still need to make brake input a float
                return 0f;
        }

        // If we get here, there is an axis we did not set a case for.
        Debug.Log("Tried to get an axis input that is not implemented");
        return -2f;
    }
    public virtual void SendKeyInput(KeyCode key)
    {
        Debug.Log("No Custom inputs!");
    }
	public void ChangeTrim(float amount, Enums.Axis axis)
    {
        if (axis == Enums.Axis.VERTICAL)
        {
            verticalTrim += amount;
            verticalTrim = Mathf.Clamp01(verticalTrim);
        }
    }
    public float GetTrim(Enums.Axis axis)
    {
        if (axis == Enums.Axis.VERTICAL) 
        { 
            return verticalTrim;
        }
        return 0f;
    }
    public VehicleType GetVehicleType()
    {
        return vehicleType;
    }
    public List<VehicleSystem> GetAllVehicleSystems()
    {
        return gameObject.GetComponentsInChildren<VehicleSystem>().ToList();
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
    protected Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
        return point + planeNormal * distance;
    }
    protected float SignedAngle(Vector3 v1, Vector3 v2, Vector3 normal)
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
