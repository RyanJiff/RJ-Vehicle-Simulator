using UnityEngine;

public class Engine : VehicleSystem
{
    /*
     * Base engine class, all throttle controlled systems should inherit from here.
     */
    [Header("Base Engine Class")]
    [SerializeField] protected bool ignition = false;

    protected float throttleInput = 0.0f;

    public virtual void SetThrottle(float t)
    {
        throttleInput = Mathf.Clamp01(t);
    }
    public virtual void ToggleIgnition()
    {
        ignition = !ignition;
    }
}
