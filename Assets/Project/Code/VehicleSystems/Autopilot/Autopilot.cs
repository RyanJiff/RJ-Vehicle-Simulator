using UnityEngine;
using PID_Controller;

[RequireComponent(typeof(PIDController))]
public class Autopilot : VehicleSystem
{
    /*
     * Simple Autopilot for fixed wing aircraft.
     * Somehow this works ??
     */

    [Header("Control")]
    // Autopilot state variables
    [SerializeField] private bool masterEngage = false;
    [SerializeField] private bool holdRoll = true;
    [SerializeField] private bool holdPitch = true;
    [SerializeField] private bool holdVerticalSpeed = false;
    [Space]

    [Header("Settings")]
    // Autopilot settings
    [SerializeField][Range(-90, 90)] private float targetRoll = 0f;
    [SerializeField] [Range(-90, 90)] private float targetPitch = 0f;
    [SerializeField] private float targetVerticalSpeed = 0f;
    [Space]

    [Header("PID controllers")]
    // Autopilot PID systems
    [SerializeField] private PIDController rollPIDController;
    [SerializeField] private PIDController pitchPIDController;
    [Space]

    [Header("DEBUG INFO")]
    // Autopilot memory
    [SerializeField] private float currentRoll = 0f;
    [SerializeField] private float requestedRollTrim = 0f;
    [SerializeField] private float currentPitch = 0f;
    [SerializeField] private float requestedPitchTrim = 0f;
    [SerializeField] private float currentVerticalSpeed = 0f;

    protected override void VehicleSystemAwake()
    {
        base.VehicleSystemAwake();

        rollPIDController.PID_Init();
    }

    protected override void VehicleSystemUpdate()
    {
        base.VehicleSystemUpdate();


        if(holdVerticalSpeed && holdPitch)
        {
            // Holding VS has more authority over holding pitch, if both are on we want holding VS to take over.
            holdPitch = false;
        }

        if (masterEngage && myVehicle != null)
        {
            if (rollPIDController != null && holdRoll)
            {
                currentRoll = myVehicle.GetRoll();
                requestedRollTrim = rollPIDController.PID_Update(targetRoll, currentRoll, Time.deltaTime);
                myVehicle.SetTrim(Enums.Axis.ROLL, requestedRollTrim);
            }
            if (pitchPIDController != null && holdPitch)
            {
                currentPitch = myVehicle.GetPitch();
                requestedPitchTrim = pitchPIDController.PID_Update(targetPitch, currentPitch, Time.deltaTime);
                myVehicle.SetTrim(Enums.Axis.PITCH, requestedPitchTrim);
            }
            if (pitchPIDController != null && holdVerticalSpeed)
            {
                // Somehow this works, because the PID controller deals in values between -1 and 1 dividing the currentVS by 1000 to artificially reduce the PIDs inputs works.
                currentVerticalSpeed = myVehicle.GetVerticalSpeedFeetPerMinute();
                requestedPitchTrim = pitchPIDController.PID_Update(targetVerticalSpeed/1000, currentVerticalSpeed/1000, Time.deltaTime);
                myVehicle.SetTrim(Enums.Axis.PITCH, requestedPitchTrim);
            }

        }
    }

    public void ToggleMasterEngage()
    {
        masterEngage = !masterEngage;
        
        // When we disable autpilot, reset the roll trim.
        if (!masterEngage)
        {
            myVehicle.SetTrim(Enums.Axis.ROLL, 0);
        }
    }
    public void ToggleVerticalSpeedHold()
    {
        holdVerticalSpeed = !holdVerticalSpeed;
    }
    public void ToggleHoldPitch()
    {
        holdPitch = !holdPitch;
    }
    public void ToggleHoldRoll()
    {
        holdRoll = !holdRoll;
    }
    public bool GetMasterEngage()
    {
        return masterEngage;
    }
    public bool GetVerticalSpeedHold()
    {
        return holdVerticalSpeed;
    }
    public bool GetHoldPitch()
    {
        return holdPitch;
    }
    public bool GetHoldRoll()
    {
        return holdRoll;
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
