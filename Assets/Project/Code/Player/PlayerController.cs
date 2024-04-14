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

    // Controllers
    private VehicleController myController = null;
    private CameraController myCameraController = null;
    private SimpleVehicleGUI myVehicleGUI = null;

    // Floating origin reference
    private FloatingOrigin floatingOrigin = null;

    // Pause menu
    [SerializeField] private GameObject pauseMenuPrefab = null;
    GameObject pauseMenuObject = null;

    // List of all selectable vehicles
    private List<Vehicle> vehiclesInPlay = new List<Vehicle>();
    private int currentSelected = 0;

    [SerializeField] private bool paused = false;

    // For Dev purposes, set the starting vehicle of the player
    public Vehicle startingVehicle; 

    void Awake()
    {
        myController = GetComponent<VehicleController>();
        myCameraController = GetComponent<CameraController>();
        myVehicleGUI = GetComponent<SimpleVehicleGUI>();
        floatingOrigin = GetComponent<FloatingOrigin>();

        if (pauseMenuPrefab)
        {
            pauseMenuObject = Instantiate(pauseMenuPrefab);
        }

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
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
        if (pauseMenuObject != null)
        {
            pauseMenuObject.SetActive(paused);
        }
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
        if (Input.GetKeyUp(Enums.PLAYER_PAUSE_GAME))
        {
            TogglePause();
        }

        Debug.Log("Physics running at: "+ (1 / Time.fixedDeltaTime) + "Hz");
    }


    /// <summary>
    /// Take control of a vehicle gameobject.
    /// </summary>
    public void TakeControl(Vehicle vehicle)
    {
        if(vehicle.GetComponent<Vehicle>() != null)
        {
            myController.GiveControl(vehicle);
            myCameraController.SetTargetVehicle(vehicle);
            myVehicleGUI.SetVehicle(vehicle);
            floatingOrigin.referenceObject = vehicle.transform;
        }
        else
        {
            Debug.LogWarning("Tried to take control of non controllable object!");
        }
    }
    public void TogglePause()
    {
        paused = !paused;
    }
}
