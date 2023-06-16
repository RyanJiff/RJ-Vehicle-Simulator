using UnityEngine;

public class VehicleController : MonoBehaviour
{
	/*
	 * Vehicle Controller manages inputs of player and sends them to the vehicle directly
     */
    [SerializeField] Vehicle vehicle = null;

    private float _inputRoll = 0;
    private float _inputPitch = 0;
    private float _inputYaw = 0;
    private float _inputBrake = 0;
	private float _inputThrottle = 0;

    public bool mouseYoke = true;
	public float deadZone = 0.02f;

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Y))
			mouseYoke = !mouseYoke;

		if (vehicle)
		{
			// Engine ignition toggle
			if (Input.GetKeyDown(Enums.VEHICLE_ENGINE_TOGGLE))
				vehicle.SendKeyInput(Enums.VEHICLE_ENGINE_TOGGLE );

			// Gear toggle
			if (Input.GetKeyDown(Enums.VEHICLE_GEAR_TOGGLE))
				vehicle.SendKeyInput(Enums.VEHICLE_GEAR_TOGGLE);

			// Trim inputs
			if (Input.GetKeyDown(Enums.VEHICLE_TRIM_VERTICAL_INCREASE))
				vehicle.SendKeyInput(Enums.VEHICLE_TRIM_VERTICAL_INCREASE);

			if (Input.GetKeyDown(Enums.VEHICLE_TRIM_VERTICAL_DECREASE))
				vehicle.SendKeyInput(Enums.VEHICLE_TRIM_VERTICAL_DECREASE);

			// Flaps for planes
			// Unused right now
			/*
			if (Input.GetKeyDown(KeyCode.Alpha3))
				vehicle.SendKeyInput(KeyCode.Alpha3);

			if (Input.GetKeyDown(KeyCode.Alpha4))
				vehicle.SendKeyInput(KeyCode.Alpha4);
			*/

			// Control Axis and brakes
			_inputRoll = Mathf.Clamp(Input.GetAxis("Horizontal") + MouseControlX(), -1, 1);
			_inputPitch = Mathf.Clamp(-Input.GetAxis("Vertical") - MouseControlY(), -1, 1);
			_inputYaw = Mathf.Clamp(Input.GetAxis("Yaw"), -1, 1);
			_inputBrake = Mathf.Clamp01(Input.GetAxis("Brake"));

			// Throttle inputs
			if (Input.GetKey(KeyCode.Alpha2))
			{
				_inputThrottle += 0.5f * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.Alpha1))
			{
				_inputThrottle -= 0.8f * Time.deltaTime;
			}
			_inputThrottle = Mathf.Clamp01(_inputThrottle);

			// Send inputs
			vehicle.SendAxisInputs(_inputPitch, _inputRoll, _inputYaw, _inputBrake,_inputThrottle);
		}
	}

	/// <summary>
	/// get X axis for mouse yoke controls
	/// </summary>
	float MouseControlX()
	{
		float xPoint = ((Input.mousePosition.x * 2 / Screen.width) - 1);
		if (Mathf.Abs(xPoint) > deadZone && mouseYoke)
		{
			return xPoint;
		}
		else return 0;

	}
	/// <summary>
	/// get Y axis for mouse yoke controls
	/// </summary>
	float MouseControlY()
	{
		float yPoint = ((Input.mousePosition.y * 2 / Screen.height) - 1);
		if (Mathf.Abs(yPoint) > deadZone && mouseYoke)
		{
			return yPoint;
		}
		else return 0;
	}

	public void GiveControl(Vehicle v)
    {
		vehicle = v;
    }
}
