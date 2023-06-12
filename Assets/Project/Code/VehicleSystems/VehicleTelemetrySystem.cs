using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleTelemetrySystem : VehicleSystem
{
    /*
     * Gets vehicle info (whatever vehicle information we desire) and creates GUI elements for it
     */

    Vehicle vehicle;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    protected override void InitializeGUIElements()
    {
        vehicleGUIElements.Add(new VehicleGUIElement("Ground Speed", "kn", "0", true));
        vehicleGUIElements.Add(new VehicleGUIElement("Vertical Speed", "ft/min", "0", true));
    }
    protected override void UpdateGUIElements()
    {
        vehicleGUIElements[0].SetValue(vehicle.GroundSpeedKnots().ToString("0"));
        vehicleGUIElements[1].SetValue(vehicle.VerticalSpeedFeetPerMinute().ToString("0"));
    }
}
