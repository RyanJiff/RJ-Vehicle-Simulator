using UnityEngine;
using System.Collections.Generic;

public class VehicleSystem : MonoBehaviour
{
    /*
     * Vehicles have a collection of vehicle systems that act as independent components, eg engines, fuel tanks, control surfaces, electrical systems.
     */

    [Header("GUI Elements")]
    [SerializeField] protected List<VehicleGUIElement> vehicleGUIElements = new List<VehicleGUIElement>();
    [SerializeField] protected bool showGUIElements = false;
    [Space]
    private bool guiElementsInitialized = false;

    protected Vehicle myVehicle = null;

    void Awake()
    {
        VehicleSystemAwake();
    }
    void Start()
    {
        VehicleSystemStart();
    }
    void Update()
    {
        VehicleSystemUpdate();
    }
    void FixedUpdate()
    {
        VehicleSystemFixedUpdate();
    }
    /// <summary>
    /// This function runs on Awake()
    /// </summary>
    protected virtual void VehicleSystemAwake()
    {
        myVehicle = GetComponentInParent<Vehicle>();
    }
    /// <summary>
    /// This function runs on Start()
    /// </summary>
    protected virtual void VehicleSystemStart()
    {
        
    }
    /// <summary>
    /// This function runs every in Update()
    /// </summary>
    protected virtual void VehicleSystemUpdate()
    {

    }
    /// <summary>
    /// This function runs every in FixedUpdate()
    /// </summary>
    protected virtual void VehicleSystemFixedUpdate()
    {

    }
    /// <summary>
    /// Initialize all the GUI elements of the vehicle system and set its parameters and base values.
    /// </summary>
    protected virtual void InitializeGUIElements()
    {
        // Implementation differs from each vehicle system
        // Example usage: vehicleGUIElements.Add(new VehicleGUIElement("This is shown on the GUI", "%", "0.0", showGUIElements));
        vehicleGUIElements.Clear();
    }

    /// <summary>
    /// Update all GUI element values and whether they show or not
    /// </summary>
    protected virtual void UpdateGUIElements()
    {
        // Implementation differs from each vehicle system
        // Example usage: vehicleGUIElements[0].SetValue((value * 100).ToString("0"));
    }
    /// <summary>
    /// Update elements and return the results
    /// </summary>
    public List<VehicleGUIElement> GetGUIElements()
    {
        if (!guiElementsInitialized)
        {
            InitializeGUIElements();
            guiElementsInitialized = true;
        }
        UpdateGUIElements();
        return vehicleGUIElements;
    }
}
