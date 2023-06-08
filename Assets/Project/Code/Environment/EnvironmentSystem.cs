using UnityEngine;

public class EnvironmentSystem : MonoBehaviour
{
    /*
     * Environment singleton, manages wind, air density and any global environment variables.
     */
    public static EnvironmentSystem instance { get; private set; }

    // Sometimes the origin is shifted and the actual positions used in calculations are offset (floating origin). 
    // We want to keep a reference transform (instantiated in Awake()) to keep any origin shifts in mind when we calculate position specific values.
    private Transform worldOriginTransform;

    // Wind vector, magnitude of each component of the vector is taken into account.
    // Where the vector points is where the wind is going.
    public Vector3 globalWindVector = Vector3.zero;

    // Used in the calculation of the wind vector
    [Range(0,360)] [SerializeField] float windDirectionDegrees = 0f;
    [Range(0,100)] [SerializeField] float windSpeedMetersPerSecond = 0f;

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

        if (!worldOriginTransform)
        {
            worldOriginTransform = new GameObject("World-Origin").transform;
            worldOriginTransform.position = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        globalWindVector = new Vector3(Mathf.Sin(windDirectionDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(windDirectionDegrees * Mathf.Deg2Rad)) * windSpeedMetersPerSecond;
    }
    
    /// <summary>
    /// The Y position of the origin transform is the sea level line.
    /// </summary>
    /// <returns></returns>
    public float GetSeaLineYPos()
    {
        return worldOriginTransform.position.y;
    }
}