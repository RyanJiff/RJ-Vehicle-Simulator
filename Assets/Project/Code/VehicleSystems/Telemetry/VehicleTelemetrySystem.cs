using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleTelemetrySystem : VehicleSystem
{
    /*
     * Gets vehicle info (whatever vehicle information we desire) and creates GUI elements for it
     */

    protected override void InitializeGUIElements()
    {
        base.InitializeGUIElements();

        vehicleGUIElements.Add(new VehicleGUIElement("Ground Speed", "kn", "0", true));
        vehicleGUIElements.Add(new VehicleGUIElement("Altitude", "ft", "0", true));
        vehicleGUIElements.Add(new VehicleGUIElement("Vertical Speed", "ft/min", "0", true));
        vehicleGUIElements.Add(new VehicleGUIElement("Pitch", "Degrees", "0", true));
        vehicleGUIElements.Add(new VehicleGUIElement("G-Force", "G", "0", true));
    }
    protected override void UpdateGUIElements()
    {
        base.UpdateGUIElements();

        vehicleGUIElements[0].SetValue(myVehicle.GetGroundSpeedKnots().ToString("0"));
        vehicleGUIElements[1].SetValue((myVehicle.GetAltitude()* 3.28084f).ToString("0"));
        vehicleGUIElements[2].SetValue(myVehicle.GetVerticalSpeedFeetPerMinute().ToString("0"));
        vehicleGUIElements[3].SetValue(myVehicle.GetPitch().ToString("0.0"));
        vehicleGUIElements[4].SetValue(myVehicle.CalculatePitchG().ToString("0.0"));
    }
}