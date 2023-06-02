using UnityEngine;

public class EnvironmentSystem : MonoBehaviour
{
    /*
     * Environment singleton, manages wind, air density and any global environment variables.
     */
    public static EnvironmentSystem instance { get; private set; }

    // Wind vector, magnitude of each component of the vector is taken into account.
    // Where the vector points is where the wind is going.
    public Vector3 globalWindVector = Vector3.zero;

    // Used in the calculation of the wind vector
    [Range(0,360)] public float windDirectionDegrees = 0f;
    [Range(0, 100)] public float windSpeedMetersPerSecond = 0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            // We already have an instance
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        globalWindVector = new Vector3(Mathf.Sin(windDirectionDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(windDirectionDegrees * Mathf.Deg2Rad)) * windSpeedMetersPerSecond;
    }
}
