using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "GameSession/StartData", order = 1)]
public class GameSessionStartData : ScriptableObject
{
    /*
     * This class contains references that are used by the application manager to load a level.
     */

    public GameObject worldPrefab = null;
    public GameObject environmentSystemPrefab = null;
    public GameObject playerPrefab = null;

    public GameObject playerStartingVehicle = null;
    public Vector3 playerVehicleStartingPosition = Vector3.zero;
}
