using UnityEngine;

public class Enums
{
    // CONTROL ENUMS
    public enum Axis { VERTICAL, HORIZONTAL};
    public enum AxisInput { PITCH, ROLL, YAW, THROTTLE, BRAKE};

    public const KeyCode VEHICLE_BRAKES = KeyCode.B;
    public const KeyCode VEHICLE_ENGINE_TOGGLE = KeyCode.I;
    public const KeyCode VEHICLE_GEAR_TOGGLE = KeyCode.G;
    public const KeyCode VEHICLE_TRIM_VERTICAL_INCREASE = KeyCode.Equals;
    public const KeyCode VEHICLE_TRIM_VERTICAL_DECREASE = KeyCode.Minus;
}
