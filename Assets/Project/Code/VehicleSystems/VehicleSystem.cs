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

    /// <summary>
    /// Initialize all the GUI elements of the vehicle system and set its parameters and base values.
    /// </summary>
    protected virtual void InitializeGUIElements()
    {
        // Implementation differs from each vehicle system
    }

    /// <summary>
    /// Update all GUI element values and whether they show or not
    /// </summary>
    protected virtual void UpdateGUIElements()
    {
        // Implementation differs from each vehicle system
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
