using UnityEngine;

public class Enums
{
    // CONTROL ENUMS
    public enum Axis { PITCH, ROLL};
    public enum AxisInput { PITCH, ROLL, YAW, THROTTLE, BRAKE};

    // Player Controller Inputs
    public const KeyCode PLAYER_SWITCH_VEHICLE = KeyCode.Tab;
    public const KeyCode PLAYER_PAUSE_GAME = KeyCode.Escape;

    // Camera Controller Inputs
    public const KeyCode CAMERA_MODE_NEXT = KeyCode.Alpha0;

    // Vehicle Inputs
    public const KeyCode VEHICLE_BRAKES = KeyCode.B;
    public const KeyCode VEHICLE_ENGINE_TOGGLE = KeyCode.I;
    public const KeyCode VEHICLE_PARKING_BRAKE_TOGGLE = KeyCode.Space;
    public const KeyCode VEHICLE_GEAR_TOGGLE = KeyCode.G;
    public const KeyCode VEHICLE_TRIM_VERTICAL_INCREASE = KeyCode.Equals;
    public const KeyCode VEHICLE_TRIM_VERTICAL_DECREASE = KeyCode.Minus;
    public const KeyCode VEHICLE_AUTOPILOT_MASTER_TOGGLE = KeyCode.Z;

    // Airplane Inputs
    public const KeyCode AIRPLANE_FLAPS_UP = KeyCode.Alpha3;
    public const KeyCode AIRPLANE_FLAPS_DOWN = KeyCode.Alpha4;

}
