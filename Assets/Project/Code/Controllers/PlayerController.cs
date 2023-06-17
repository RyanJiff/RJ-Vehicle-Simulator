using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(VehicleController))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(SimpleVehicleGUI))]
[RequireComponent(typeof(FloatingOrigin))]
public class PlayerController : MonoBehaviour
{
    /*
     * Handles handing player control over a vehicle and any non vehicle inputs
     */

    VehicleController myController = null;
    CameraController myCameraController = null;
    SimpleVehicleGUI myVehicleGUI = null;
    FloatingOrigin floatingOrigin = null;

    // List of all selectable vehicles
    private List<Vehicle> vehiclesInPlay = new List<Vehicle>();
    private int currentSelected = 0;

    // For Dev purposes, set the starting vehicle of the player
    public Vehicle startingVehicle; 

    void Awake()
    {
        myController = GetComponent<VehicleController>();
        myCameraController = GetComponent<CameraController>();
        myVehicleGUI = GetComponent<SimpleVehicleGUI>();
        floatingOrigin = GetComponent<FloatingOrigin>();

        vehiclesInPlay = FindObjectsOfType<Vehicle>().ToList();
    }
    private void Start()
    {
        if (startingVehicle)
        {
            TakeControl(startingVehicle);
        }
    }

    void Update()
    {
        if (ApplicationManager.instance)
        {
            // If we have an application manager then R to reload the scene, for early builds.
            // Still need to make a proper menu for such functionality
            if (Input.GetKeyDown(KeyCode.R))
                ApplicationManager.instance.ReloadScene();
        }
        if (Input.GetKeyDown(Enums.PLAYER_SWITCH_VEHICLE))
        {
            currentSelected = (currentSelected + 1) % vehiclesInPlay.Count;
            TakeControl(vehiclesInPlay[currentSelected]);
        }
    }


    /// <summary>
    /// Take control of an airplane gameobject and set the GUI to reflect it
    /// </summary>
    public void TakeControl(Vehicle vehicle)
    {
        if(vehicle.GetComponent<Vehicle>() != null)
        {
            myController.GiveControl(vehicle);
            myCameraController.SetTargetVehicle(vehicle.transform);
            myVehicleGUI.SetVehicle(vehicle);
            floatingOrigin.referenceObject = vehicle.transform;
        }
        else
        {
            Debug.LogWarning("Tried to take control of non controllable object!");
        }
    }
}
