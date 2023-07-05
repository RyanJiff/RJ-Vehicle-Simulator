using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
     * Camera controller to control camera rig transform around target vehicle
     */
    public enum CameraMode { CHASE, FIXED, ORBIT }
    [SerializeField] private CameraMode cameraMode = CameraMode.CHASE;

    // This is the camera rig prefab, it must contain a camera as a child gameobjects
    [SerializeField] private GameObject cameraRigPrefab;

    // Transforms to act on
    private Transform cameraRigTransform;
    private Transform cameraTransform;
    private Transform target;
    private Rigidbody targetRigid;

    // Parameters to set for fixed cam angles
    [SerializeField] private List<Vector3> offsets = new List<Vector3>();
    [SerializeField] private List<Vector3> lookOffsets = new List<Vector3>();
    [SerializeField] private List<float> distances = new List<float>();

    // Chase camera
    [SerializeField] private Vector3 chaseOffset = Vector3.zero;
    [SerializeField] private float distanceFromCenterChase = 12f;

    // Orbit camera
    [SerializeField] private float distanceFromCenterOrbit = 8f;
    [SerializeField] private float lookAroundSpeed = 20f;
    [SerializeField] private Vector3 offsetOrbit = Vector3.zero;

    private Vector3 nextPosRig;
    private Vector3 nextPosCam;
    private Vector3 rigidVelocity;
    private Vector3 rigidVelocitySmoothed = Vector3.zero;
    private Vector3 refVelocity = Vector3.zero;
    private Vector3 rigidVelocityNormalized;
    private float RotAroundY = -90f;
    private float RotAroundXZ = 75f;

    int i = 0;

    private void Awake()
    {
        InitializeCameraController();
    }

    void LateUpdate()
    {
        if (target)
        {
            // This is bad, need to write a proper switching function
            if (Input.GetKeyDown(Enums.CAMERA_MODE_NEXT))
            {
                ResetCameraVars();
                switch (cameraMode)
                {
                    case CameraMode.CHASE:
                        cameraMode = CameraMode.FIXED;
                        break;
                    case CameraMode.FIXED:
                        cameraMode = CameraMode.ORBIT;
                        break;
                    case CameraMode.ORBIT:
                        cameraMode = CameraMode.CHASE;
                        break;
                }
            }
            if (cameraMode == CameraMode.CHASE)
            {
                ChaseCameraMode();
            }
            else if (cameraMode == CameraMode.FIXED)
            {
                FixedCameraMode();
            }
            else if(cameraMode == CameraMode.ORBIT)
            {
                OrbitCameraMode();
            }
        }
    }

    /// <summary>
    /// When the object first spawns
    /// </summary>
    void InitializeCameraController()
    {
        if (!cameraRigPrefab)
        {
            Debug.LogWarning("NO CAMERA RIG PREFAB SET! ABORTING CAMERA MANAGER INITIALIZATION");
            this.enabled = false;
        }
        cameraMode = CameraMode.CHASE;
        cameraRigTransform = Instantiate(cameraRigPrefab).transform;
        cameraTransform = cameraRigTransform.GetComponentInChildren<Camera>().transform;
        ResetCameraVars();
    }
    void ResetCameraVars()
    {
        nextPosRig = Vector3.zero;
        nextPosCam = Vector3.zero;
        rigidVelocity = Vector3.zero;
        rigidVelocitySmoothed = Vector3.zero;
        refVelocity = Vector3.zero;
        rigidVelocityNormalized = Vector3.zero;
    }
    public void SetTargetVehicle(Vehicle t)
    {
        target = t.transform;
        targetRigid = t.GetComponent<Rigidbody>();
        ResetCameraVars();
    }
    public Transform GetCameraRigTransform()
    {
        if (cameraRigTransform)
        {
            return cameraRigTransform;
        }
        return null;
    }
    private void ChaseCameraMode()
    {
        if (targetRigid)
        {
            cameraRigTransform.rotation = Quaternion.identity;

            rigidVelocity = targetRigid.velocity;
            rigidVelocitySmoothed = Vector3.SmoothDamp(rigidVelocitySmoothed, rigidVelocity + target.forward, ref refVelocity, 1f, Mathf.Abs(Vector3.Distance(rigidVelocitySmoothed, rigidVelocity)) * 4f + 0.5f, Time.deltaTime);
            rigidVelocityNormalized = rigidVelocitySmoothed.normalized;
            
            nextPosRig = target.position;
            nextPosCam = -rigidVelocityNormalized * distanceFromCenterChase;

            cameraTransform.localPosition = nextPosCam + chaseOffset;
            cameraRigTransform.position= nextPosRig;
            cameraTransform.LookAt(target.position + rigidVelocityNormalized * 50f, Vector3.up);
        }
    }
    private void FixedCameraMode()
    {
        cameraTransform.localPosition = Vector3.zero;
        cameraRigTransform.rotation = Quaternion.identity;

        if (Input.GetKeyDown(KeyCode.V))
        {
            i = (i + 1) % offsets.Count;
        }

        if (Input.GetKey(KeyCode.C))
        {
            nextPosRig = target.forward * distances[i] + target.position + target.TransformDirection(offsets[i]);
        }
        else
        {
            nextPosRig = target.forward * -distances[i] + target.position + target.TransformDirection(offsets[i]);
        }

        cameraRigTransform.position = nextPosRig;
        cameraTransform.LookAt(target.position + target.up * lookOffsets[i].y + target.right * -lookOffsets[i].x + target.forward * lookOffsets[i].z, target.up);
    }
    private void OrbitCameraMode()
    {
        /*
         * For reference
            x = r * cos(s) * sin(t)
            y = r * sin(s) * sin(t)
            z = r * cos(t)
            here, s is the angle around the z-axis, and t is the height angle, measured 'down' from the z-axis.

            x=rsinθ, y=rcosθ
        */
        if (Input.GetKey(KeyCode.Keypad8))
        {
            RotAroundXZ = RotAroundXZ - lookAroundSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            RotAroundXZ = RotAroundXZ + lookAroundSpeed * Time.deltaTime;
        }
        RotAroundXZ = Mathf.Clamp(RotAroundXZ, 5f, 175f);

        if (Input.GetKey(KeyCode.Keypad6))
        {
            RotAroundY = RotAroundY + lookAroundSpeed * 2 * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Keypad4))
        {
            RotAroundY = RotAroundY - lookAroundSpeed * 2 * Time.deltaTime;
        }

        nextPosRig = target.position + target.up * offsetOrbit.y + target.right * -offsetOrbit.x + target.forward * offsetOrbit.z;

        float XPosCam = distanceFromCenterOrbit * Mathf.Cos(RotAroundY * Mathf.Deg2Rad) * Mathf.Sin(RotAroundXZ * Mathf.Deg2Rad);
        float ZPosCam = distanceFromCenterOrbit * Mathf.Sin(RotAroundY * Mathf.Deg2Rad) * Mathf.Sin(RotAroundXZ * Mathf.Deg2Rad);
        float YPosCam = distanceFromCenterOrbit * Mathf.Cos(RotAroundXZ * Mathf.Deg2Rad);
        nextPosCam = new Vector3(XPosCam, YPosCam, ZPosCam);

        cameraTransform.localPosition = nextPosCam;
        cameraRigTransform.position = nextPosRig;
        cameraTransform.LookAt(target.position + target.up * offsetOrbit.y + target.right * -offsetOrbit.x + target.forward * offsetOrbit.z, Vector3.up);
    }
}