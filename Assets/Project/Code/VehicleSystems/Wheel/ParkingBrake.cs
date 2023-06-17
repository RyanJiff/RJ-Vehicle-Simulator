using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParkingBrake : VehicleSystem
{
    [SerializeField] bool parkingBrakeSet = false;

    private List<Wheel> myVehicleWheels = new List<Wheel>();

    protected override void VehicleSystemAwake()
    {
        base.VehicleSystemAwake();

        if (myVehicle)
        {
            myVehicleWheels = myVehicle.GetComponentsInChildren<Wheel>().ToList();
        }
    }

    public void SetParkingBrakeState(bool s)
    {
        parkingBrakeSet = s;
        for(int i = 0;i < myVehicleWheels.Count; i++)
        {
            myVehicleWheels[i].SetParkingBrake(parkingBrakeSet);
        }
    }
    public void ToggleParkingBrake()
    {
        parkingBrakeSet = !parkingBrakeSet;
        for (int i = 0; i < myVehicleWheels.Count; i++)
        {
            myVehicleWheels[i].SetParkingBrake(parkingBrakeSet);
        }
    }
    protected override void InitializeGUIElements()
    {
        base.InitializeGUIElements();

        vehicleGUIElements.Add(new VehicleGUIElement("Parking Brake", "", "",showGUIElements));
    }
    protected override void UpdateGUIElements()
    {
        base.UpdateGUIElements();

        vehicleGUIElements[0].SetValue(parkingBrakeSet.ToString());
    }

}
