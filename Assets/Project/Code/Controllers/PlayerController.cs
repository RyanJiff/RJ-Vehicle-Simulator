using UnityEngine;

[RequireComponent(typeof(VehicleController))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(SimpleVehicleGUI))]
public class PlayerController : MonoBehaviour
{
    /*
     * Handles handing player control over a vehicle and any non vehicle inputs
     */
    VehicleController myController = null;
    CameraController myCameraController = null;
    SimpleVehicleGUI myVehicleGUI = null;

    // For Dev purposes, set the starting vehicle of the player
    public Vehicle startingVehicle; 

    void Awake()
    {
        myController = GetComponent<VehicleController>();
        myCameraController = GetComponent<CameraController>();
        myVehicleGUI = GetComponent<SimpleVehicleGUI>();
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
        }
        else
        {
            Debug.LogWarning("Tried to take control of non controllable object!");
        }
    }
}
