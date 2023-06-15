using System.Collections.Generic;
using UnityEngine;

public class Engine : VehicleSystem
{
    /*
     * All engines have some things in common, here is the super class that has all common engine functionality.
     */

    [Header("Engine")]
    [SerializeField] private bool ignition = false;
    [SerializeField] private float maxPower = 3000;
    [SerializeField] private float rampSpeed = 0.5f;
    [Space]

    [Header("Debug")]
    [SerializeField] float currentPower;
    [Space]

    private float throttleInput = 0.0f;
    
    private Rigidbody rigid;
    private Vehicle myVehicle;

    const float msToKnots = 1.94384f;

    void Awake()
    {
        myVehicle = GetComponentInParent<Vehicle>();
        if (!myVehicle)
        {
            // If we don't have a vehicle on us or our parents then disable the script
            Debug.LogWarning(name + ": SimpleAirplaneEngine missing Vehicle!");
            enabled = false;
            return;
        }

        rigid = myVehicle.GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (rigid != null)
        {
            // Apply forces or affect vehicle systems here
            rigid.AddForceAtPosition(transform.forward * currentPower , transform.position, ForceMode.Force);
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
            currentPower = Mathf.MoveTowards(currentPower, 0, rampSpeed / 2 * Time.deltaTime);
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
