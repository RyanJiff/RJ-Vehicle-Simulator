using UnityEngine;

public interface IControllable
{
    void SendAxisInputs(float y, float x, float z, bool brake, float throttle);
    float GetAxisInput(Enums.AxisInput aI);
    void SendKeyInput(KeyCode key);
    void ChangeTrim(float amount, Enums.Axis axis);
    float GetTrim(Enums.Axis axis);
}
