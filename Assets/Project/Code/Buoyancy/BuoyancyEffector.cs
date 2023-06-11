using UnityEngine;

public class BuoyancyEffector : MonoBehaviour
{
    /*
     * Applies an upward force or drag based on if we are under the sea line and by how much.
     * Simple buoyancy force equation: F = waterDensity*g*displacementVolume*buoyancyMultiplier*underwaterPercent
        F: Force in netwons ( this is what we want to calculate )
        waterDensity: Density of fluid ( for water this is ~997 kg/m^3 )
        g: gravity ( for earth at sea level this is ~9.8m/s^2 )
        displacementVolume: Volume displaced by object
        buoyancyMultiplier: A constant multiplier to the total force
        underWaterPercent: How much of the object is under the water line 
     */

    [Tooltip("How big the effector is, used for the volume calculation.")]
    public float effectorSize = 1f;
    [Tooltip("When true, buoyant forces will be applied only at the center of mass.")]
    public bool applyForcesToCenter = false;
    [Tooltip("The higher the value, the more buoyancy the effector")]
    public float buoyancyMultiplier = 1f;
    [Tooltip("Drag multiplier, the higher the value the more the drag force")]
    public Vector3 dragMultiplier = Vector3.one;

    private Rigidbody rigid;

    // This would be the gravity vector * -1f in a space/planet type simulation.
    private Vector3 buoyancyForceDirection = Vector3.up;

    // This is used to gauge how much volume is under water, the calculation assumes the shape to be a cube and does not take rotation into consideration.
    private float underWaterPercent = 0f;

    private float buoyancyForce = 0f;
    private float dragForce = 0f;
    private Vector3 dragDirection = Vector3.zero;
    private Vector3 dragDirectionAndForce = Vector3.zero;

    // Gravity constant (m/s^2)
    private const float g = 9.81f;
    // Water density constant (KG/M^3)
    private const float waterDensity = 997f;

    public float EffectorVolume
    {
        get { return effectorSize*effectorSize*effectorSize; }
    }

    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
    }
    private void Update()
    {
        // DEBUG
        if (rigid != null)
        {
            Debug.DrawRay(transform.position, buoyancyForce * buoyancyForceDirection * 0.001f, Color.magenta);
            Debug.DrawRay(transform.position, dragDirectionAndForce * underWaterPercent * 0.01f, Color.red);
        }
    }

    private void FixedUpdate()
    {
        
        if(rigid != null)
        {
            Vector3 forceApplyPos = (applyForcesToCenter) ? rigid.transform.TransformPoint(rigid.centerOfMass) : transform.position;

            float seaLevelLine = 0f;

            // Buoyancy calculations require the EnvironmentSystem instance to know the sea level, otherwise we just set the sealevel to 0.
            if (EnvironmentSystem.instance)
            {
                seaLevelLine = EnvironmentSystem.instance.GetSeaLineYPos();
            }

            Vector3 worldVelocity = rigid.GetPointVelocity(transform.position);

            // Lowest point by Y value of the effector, this value will be used in the percent underwater calculation.
            Vector3 lowestPointOnEffector = transform.position + (Vector3.down * effectorSize / 2);
            //Debug.DrawLine(lowestPointOnEffector, transform.position);

            // How much of the volume is underwater, this is really primitive and could be improved.
            underWaterPercent = Mathf.Clamp01((seaLevelLine - lowestPointOnEffector.y)/effectorSize);

            // Water density and g (gravity) are pretty arbitrary, should probably only use the buoyancyMultiplier.
            buoyancyForce = waterDensity * g * underWaterPercent * buoyancyMultiplier * EffectorVolume;
            
            // Again, the water density variable should be removed as it has no real bearing on the final result.
            dragForce = worldVelocity.sqrMagnitude * waterDensity;            

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
            rigid.AddForceAtPosition(buoyancyForceDirection * buoyancyForce, forceApplyPos, ForceMode.Force);
            rigid.AddForceAtPosition(dragDirectionAndForce * underWaterPercent, forceApplyPos, ForceMode.Force);
        }
    }
    // Prevent this code from throwing errors in a built game.
    #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(effectorSize, effectorSize, effectorSize));
            //Gizmos.DrawWireSphere(transform.InverseTransformDirection(Vector3.down) * effectorSize/2 , effectorSize/4f);
            Gizmos.matrix = oldMatrix;
        }
    #endif
}
