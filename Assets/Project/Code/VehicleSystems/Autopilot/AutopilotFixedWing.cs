using UnityEngine;
using PID_Controller;

[RequireComponent(typeof(PIDController))]
public class AutopilotFixedWing : VehicleSystem
{
    /*
     * Simple Autopilot for fixed wing aircraft.
     * Somehow this works ??
     */

    // Autopilot state variables
    [SerializeField] private bool masterEngage = false;

    [SerializeField] private bool holdRoll = true;
    [SerializeField] private bool holdPitch = true;

    // Autopilot PID systems
    [SerializeField] private PIDController rollPIDController;
    [SerializeField] private PIDController pitchPIDController;

    // Autopilot settings
    [SerializeField][Range(-90, 90)] private float targetRoll = 0f;
    [SerializeField] [Range(-90, 90)] private float targetPitch = 0f;

    // Autopilot memory
    [SerializeField] private float currentRoll = 0f;
    [SerializeField] private float requestedRollTrim = 0f;
    [SerializeField] private float currentPitch = 0f;
    [SerializeField] private float requestedPitchTrim = 0f;

    protected override void VehicleSystemAwake()
    {
        base.VehicleSystemAwake();

        rollPIDController.PID_Init();
    }

    protected override void VehicleSystemUpdate()
    {
        base.VehicleSystemUpdate();

        if (masterEngage && myVehicle != null)
        {
            if (rollPIDController != null && holdRoll)
            {
                currentRoll = myVehicle.GetRoll();
                requestedRollTrim = rollPIDController.PID_Update(targetRoll, currentRoll, Time.deltaTime);
                myVehicle.SetTrim(Enums.Axis.HORIZONTAL, requestedRollTrim);
            }
            if (pitchPIDController != null && holdPitch)
            {
                currentPitch = myVehicle.GetPitch();
                requestedPitchTrim = pitchPIDController.PID_Update(targetPitch, currentPitch, Time.deltaTime);
                myVehicle.SetTrim(Enums.Axis.VERTICAL, requestedPitchTrim);
            }
        }
    }

    protected override void InitializeGUIElements()
    {
        base.InitializeGUIElements();

        vehicleGUIElements.Add(new VehicleGUIElement("Autopilot", "", "", showGUIElements));
    }
    protected override void UpdateGUIElements()
    {
        base.UpdateGUIElements();

        vehicleGUIElements[0].SetValue(masterEngage.ToString());
    }
}
