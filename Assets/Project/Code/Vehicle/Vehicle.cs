using UnityEngine;

public class Vehicle : MonoBehaviour, IControllable
{
    /*
     * Any controllable vehicle inherits from this class
     */

    // Inputs that are changed through external MonoBehaviours (VehicleController)
    protected float _inputRoll = 0;
    protected float _inputPitch = 0;
    protected float _inputYaw = 0;
    protected bool _inputBrake = false;
    protected float _inputThrottle = 0;
    protected float verticalTrim = 0.0f;

    [Header("General Vehicle Parameters")]
    [Tooltip("Dry Weight of the vehicle without fuel or cargo")]
    [SerializeField] protected float baseWeightKG = 1000.0f;
    [Space]

    [Header("Telemetry and information")]
    public float pitch;
    public float roll;


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
    /// <summary>
	/// Change trim by amount on the selected axis
	/// </summary>
	public void ChangeTrim(float amount, Enums.Axis axis)
    {
        if (axis == Enums.Axis.VERTICAL)
        {
            verticalTrim += amount;
            verticalTrim = Mathf.Clamp01(verticalTrim);
        }
    }
    /// <summary>
	/// Get trim by amount on the selected axis
	/// </summary>
    public float GetTrim(Enums.Axis axis)
    {
        if (axis == Enums.Axis.VERTICAL) 
        { 
            return verticalTrim;
        }
        return 0f;
    }
    /// <summary>
    /// Vehicle update updates any vehicle related functions, use to update mass or any other vehicle specific function
    /// </summary>
    public virtual void VehicleUpdate()
    {
        Debug.Log("Vehicle update not implemented for this vehicle or base function called.");
    }
}
