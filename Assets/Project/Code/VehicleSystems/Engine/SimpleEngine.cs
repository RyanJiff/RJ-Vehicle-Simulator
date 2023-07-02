using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleEngine : Engine
{
    /*
     * Simple engine to apply a force or drive wheels.
     */
    public enum EngineType {Force, DriveWheels};

    [Header("Simple Engine")]
    [SerializeField] private EngineType engineType = EngineType.Force;
    [SerializeField] private float maxForce = 3000;
    [SerializeField] private float rampSpeed = 0.5f;
    [SerializeField] private bool worksUnderWater = false;
    [SerializeField] private bool worksOverWater = true;
    [Space]

    [Header("Debug")]
    [SerializeField] [Range(0f, 1f)] float currentPower;
    [Space]

    private bool isOverWater = false;
    private bool applyForceFlag = false;
    
    private Rigidbody rigid;
    private List<Wheel> wheels = new List<Wheel>();

    private Vector3 originShiftBy = Vector3.zero;

    protected override void VehicleSystemAwake()
    {
        base.VehicleSystemAwake();

        if (!myVehicle)
        {
            // If we don't have a vehicle on us or our parents then disable the script
            Debug.LogWarning(name + ": Engine missing Vehicle!");
            enabled = false;
            return;
        }
        wheels = myVehicle.GetComponentsInChildren<Wheel>().ToList();
        rigid = myVehicle.GetComponent<Rigidbody>();

        FloatingOrigin.OnOriginShiftEnded.AddListener(OriginShifted);
    }
    protected override void VehicleSystemUpdate()
    {
        base.VehicleSystemUpdate();

        if (ignition)
        {
            // Power calculations
            currentPower = Mathf.MoveTowards(currentPower, throttleInput, rampSpeed * Time.deltaTime);
        }
        else
        {
            currentPower = Mathf.MoveTowards(currentPower, 0, rampSpeed * Time.deltaTime);
        }
    }
    protected override void VehicleSystemFixedUpdate()
    {
        base.VehicleSystemFixedUpdate();

        if (rigid != null)
        {
            isOverWater = false;
            applyForceFlag = false;

            if (EnvironmentSystem.instance)
            {
                if (transform.position.y > EnvironmentSystem.instance.GetSeaLineYPos())
                {
                    isOverWater = true;
                }
            }
            else
            {
                if (transform.position.y > 0)
                {
                    isOverWater = true;
                }
            }

            if ((worksOverWater && isOverWater) || (worksUnderWater && !isOverWater))
            {
                applyForceFlag = true;
            }

            if (applyForceFlag)
            {
                Vector3 forceDirection = transform.forward;
                
                // Force engines apply a force at thier position.
                if (engineType == EngineType.Force)
                {
                    rigid.AddForceAtPosition(forceDirection * currentPower * maxForce, (transform.position + originShiftBy), ForceMode.Force);
                }
                // Wheel drive engines drive wheels, needs to be implemented still.
                else if (engineType == EngineType.DriveWheels)
                {
                    SetPowerWheelsTorque(wheels, currentPower * maxForce);
                }
            }
            else
            {
                SetPowerWheelsTorque(wheels, 0);
            }
        }

        // Origin shift hack part 2
        originShiftBy = Vector3.zero;
    }
    public void OriginShifted(Vector3 v)
    {
        originShiftBy = v;
    }
    private void SetPowerWheelsTorque(List<Wheel> w, float torque)
    {
        for(int i = 0; i < w.Count; i++)
        {
            if (w[i].GetPowerEnabled())
            {
                w[i].SetTorque(torque);
            }
            else
            {
                w[i].SetTorque(0);
            }
        }
    }
    protected override void InitializeGUIElements()
    {
        base.InitializeGUIElements();

        vehicleGUIElements.Add(new VehicleGUIElement("Throttle", "%", "0.0", showGUIElements));
        vehicleGUIElements.Add(new VehicleGUIElement("Engine Ignition", "", "", showGUIElements));
    }
    protected override void UpdateGUIElements()
    {
        vehicleGUIElements[0].SetValue((throttleInput * 100).ToString("0"));
        vehicleGUIElements[1].SetValue(ignition.ToString());
    }
}
