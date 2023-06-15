using UnityEngine;

public class Enums
{
    // CONTROL ENUMS
    public enum Axis { VERTICAL, HORIZONTAL};
    public enum AxisInput { PITCH, ROLL, YAW, THROTTLE, BRAKE};

    public const KeyCode ENGINE_TOGGLE = KeyCode.I;
    public const KeyCode GEAR_TOGGLE = KeyCode.G;
    public const KeyCode TRIM_VERTICAL_INCREASE = KeyCode.Equals;
    public const KeyCode TRIM_VERTICAL_DECREASE = KeyCode.Minus;
}
