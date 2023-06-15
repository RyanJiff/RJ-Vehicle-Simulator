using System.Collections.Generic;
using UnityEngine;

public class Engine : VehicleSystem
{
    /*
     * All engines have some things in common, here is the super class that has all common engine functionality.
     */
    public enum EngineType { Force, DriveWheels};

    [Header("Engine")]
    [SerializeField] private EngineType engineType = EngineType.Force;
    [SerializeField] private bool ignition = false;
    [SerializeField] private float maxForce = 3000;
    [SerializeField] private float rampSpeed = 0.5f;
    [SerializeField] private bool worksUnderWater = false;
    [SerializeField] private bool worksOverWater = true;
    [Space]

    [Header("Debug")]
    [SerializeField] float currentPower;
    [Space]

    private float throttleInput = 0.0f;
    private bool isOverWater = false;
    private bool applyForceFlag = false;
    
    private Rigidbody rigid;
    private Vehicle myVehicle;

    void Awake()
    {
        myVehicle = GetComponentInParent<Vehicle>();

        if (!myVehicle)
        {
            // If we don't have a vehicle on us or our parents then disable the script
            Debug.LogWarning(name + ": Engine missing Vehicle!");
            enabled = false;
            return;
        }

        rigid = myVehicle.GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (rigid != null)
        {
            isOverWater = false;
            applyForceFlag = false;

            if (EnvironmentSystem.instance)
            {
                if(transform.position.y > EnvironmentSystem.instance.GetSeaLineYPos())
                {
                    isOverWater = true;
                }
            }

            if((worksOverWater && isOverWater) || (worksUnderWater && !isOverWater))
            {
                applyForceFlag = true;
            }

            if (applyForceFlag)
            {
                // Force engines apply a force at thier position.
                if (engineType == EngineType.Force)
                {
                    rigid.AddForceAtPosition(transform.forward * currentPower * maxForce, transform.position, ForceMode.Force);
                }
                // Wheel drive engines drive wheels, needs to be implemented still.
                else if (engineType == EngineType.DriveWheels)
                {

                }
            }
        }
    }
    void Update()
    {
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
    /// <summary>
    /// Current power of engine, between 0.0f and 1.0f
    /// </summary>
    public float GetPower()
    {
        return currentPower;
    }
    public void SetThrottle(float t)
    {
        throttleInput = Mathf.Clamp01(t);
    }
    public void ToggleIgnition()
    {
        ignition = !ignition;
    }
    public bool IsOn()
    {
        return ignition;
    }

    #region VEHICLESYSTEMGUI
    protected override void InitializeGUIElements()
    {
        vehicleGUIElements.Add(new VehicleGUIElement("Throttle", "%", "0.0", showGUIElements));
    }
    protected override void UpdateGUIElements()
    {
        vehicleGUIElements[0].SetValue((throttleInput * 100).ToString("0"));
    }
    #endregion

}
