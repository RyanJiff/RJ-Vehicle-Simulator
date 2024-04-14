using UnityEngine;
using System.Collections.Generic;

public class VolumetricBuoyancy : MonoBehaviour
{
    /*
     * Applies an upward force and drag based on if we are under the sea line and by how much.
     * Uses cube volume 
     */

    [Tooltip("How big the effector is, used for the volume calculation.")]
    public Vector3 effectorDimensions = Vector3.one;
    [Tooltip("Automatic dimension setting will use the transforms local scale")]
    public bool automaticallySetDimensions = false;
    [Tooltip("The higher the value, the more buoyancy the effector has")]
    public float buoyancyMultiplier = 1f;
    [Tooltip("Drag multiplier, the higher the value the more the drag force")]
    public Vector3 dragMultiplier = Vector3.one * 0.025f;
    [Tooltip("How many subdivisions do we have for the volume effect points")]
    [Range(1,10)] public int volumeSubdivisions = 1;

    private Rigidbody rigid;

    // This would be the gravity vector * -1f in a space/planet type simulation.
    private Vector3 buoyancyForceDirection = Vector3.up;

    // This is used to gauge how much volume is under water for a given point.
    private float underWaterPercent = 0f;

    private float buoyancyForce = 0f;
    private float dragForce = 0f;
    private Vector3 dragDirection = Vector3.zero;
    private Vector3 dragDirectionAndForce = Vector3.zero;

    // Gravity constant (m/s^2)
    private const float g = 9.81f;
    // Water density constant (KG/M^3)
    private const float waterDensity = 997f;

    [SerializeField] List<Vector3> pointsOfEffect = new List<Vector3>();
    [SerializeField] float effectorPointSize = 0f;


    public float EffectorVolume
    {
        get { return effectorDimensions.x * effectorDimensions.y * effectorDimensions.z; }
    }

    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
    }
    private void Start()
    {
        if (automaticallySetDimensions)
        {
            effectorDimensions = transform.localScale;
        }
        CalculateForcePoints();
    }
    private void Update()
    {
        // DEBUG
        if (rigid != null)
        {
            
        }
    }

    private void FixedUpdate()
    {

        if (rigid != null)
        {
            float seaLevelLine = 0f;

            // Buoyancy calculations require the EnvironmentSystem instance to know the sea level, otherwise we just set the sealevel to 0.
            if (EnvironmentSystem.instance)
            {
                seaLevelLine = EnvironmentSystem.instance.GetSeaLineYPos();
            }

            //CalculateForcePoints();

            for (int i = 0; i < pointsOfEffect.Count; i++)
            {
                Vector3 calculationPoint = transform.position + transform.TransformDirection(pointsOfEffect[i]);

                Vector3 worldVelocity = rigid.GetPointVelocity(calculationPoint);

                // Lowest point by Y value of the effector, this value will be used in the percent underwater calculation.
                Vector3 lowestPointOnCalculationPoint = calculationPoint + (Vector3.down * (effectorPointSize / 2));
                Debug.DrawLine(lowestPointOnCalculationPoint, lowestPointOnCalculationPoint + Vector3.down);

                // How much of the volume is underwater, this is really primitive and could be improved.
                underWaterPercent = Mathf.Clamp01((seaLevelLine - lowestPointOnCalculationPoint.y) / effectorPointSize);

                // Water density and g (gravity) are pretty arbitrary, should probably only use the buoyancyMultiplier.
                buoyancyForce = waterDensity * g * underWaterPercent * buoyancyMultiplier * EffectorVolume / pointsOfEffect.Count;

                // Again, the water density variable should be removed as it has no real bearing on the final result.
                dragForce = worldVelocity.sqrMagnitude * waterDensity / pointsOfEffect.Count;
                dragForce = Mathf.Clamp(dragForce, rigid.mass * rigid.velocity.magnitude * -50f, rigid.mass * rigid.velocity.magnitude * 50f);

                // Drag direciton should always be opposite of effector velocity.
                dragDirection = (-worldVelocity).normalized;

                // We need to convert the drag direction to a local velocity and then apply the drag coeffecients, this is not very effecient and could be better.
                // NEEDS TESTING
                dragDirection = transform.InverseTransformDirection(dragDirection);
                dragDirection = new Vector3(dragDirection.x * dragMultiplier.x, dragDirection.y * dragMultiplier.y, dragDirection.z * dragMultiplier.z);
                dragDirection = transform.TransformDirection(dragDirection);

                // Drag force should never be more than the mass of the object times a constant, this is because as velocity approaches really big numbers the resulting drag force can cause instability.
                // Clamping the force to only be able to apply a force relative to the mass is a cheesy way of solving this issue.
                dragDirectionAndForce = dragDirection * dragForce;

                // Apply forces
                rigid.AddForceAtPosition(buoyancyForceDirection * buoyancyForce, calculationPoint, ForceMode.Force);
                rigid.AddForceAtPosition(dragDirectionAndForce * underWaterPercent, calculationPoint, ForceMode.Force);

                if (rigid != null)
                {
                    Debug.DrawRay(calculationPoint, buoyancyForce * buoyancyForceDirection * 0.01f, Color.magenta);
                    Debug.DrawRay(calculationPoint, dragDirectionAndForce * underWaterPercent * 0.01f, Color.red);
                }
            }
        }
    }

    void CalculateForcePoints()
    {
        float smallestDimensionAxis = effectorDimensions.x;

        if (effectorDimensions.y < smallestDimensionAxis)
        {
            smallestDimensionAxis = effectorDimensions.y;
        }
        if (effectorDimensions.z < smallestDimensionAxis)
        {
            smallestDimensionAxis = effectorDimensions.z;
        }

        effectorPointSize = smallestDimensionAxis / (volumeSubdivisions + 1);
        int totalPoints = 0;
        pointsOfEffect.Clear();
        
        // Volume points
        for (int x = 0; x < volumeSubdivisions + 1; x++)
        {
            for (int y = 0; y < volumeSubdivisions + 1; y++)
            {
                for (int z = 0; z < volumeSubdivisions + 1; z++)
                {
                    Vector3 preShiftPoint = new Vector3(-effectorDimensions.x / 2 + (effectorDimensions.x / volumeSubdivisions + 1 / 2) * x,
                                                    -effectorDimensions.y / 2 + (effectorDimensions.y / volumeSubdivisions + 1 / 2) * y,
                                                    -effectorDimensions.z / 2 + (effectorDimensions.z / volumeSubdivisions + 1 / 2) * z);

                    Vector3 postShiftPoint = preShiftPoint + ((transform.InverseTransformVector(transform.position - transform.TransformPoint(preShiftPoint))) / (volumeSubdivisions + 1));

                    //Gizmos.DrawWireCube(postShiftPoint, Vector3.one * effectorPointSize);
                    pointsOfEffect.Add(postShiftPoint);
                    totalPoints++;
                }
            }
        }

        Debug.Log("Calculated " + totalPoints + "buoyancy points");
    }
    // Prevent this code from throwing errors in a built game.
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        //CalculateForcePoints();

        // Volume bounds
        //Gizmos.DrawWireCube(Vector3.zero, effectorDimensions);
        for(int i = 0;i < pointsOfEffect.Count;i++)
        {
            Gizmos.DrawWireCube(pointsOfEffect[i], Vector3.one * effectorPointSize);
            //Gizmos.DrawWireSphere(postShiftPoint, effectorPointSize/2);
            Gizmos.DrawRay(pointsOfEffect[i], transform.InverseTransformVector(-Vector3.up * effectorPointSize / 2));
        }
        Gizmos.matrix = oldMatrix;
    }
    #endif
}
