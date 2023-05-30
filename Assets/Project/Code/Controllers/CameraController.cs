using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
     * Camera controller to control camera rig transform around target vehicle
     */
    public enum CameraMode { normal, orbit }
    public CameraMode cameraMode = CameraMode.normal;

    // This is the camera rig prefab, it must contain a camera as a child gameobjects
    [SerializeField] private GameObject cameraRigPrefab;

    // Transforms to act on
    private Transform cameraRigTransform;
    private Transform cameraTransform;
    private Transform target;

    // Parameters to set for cam angles
    public List<Vector3> offsets = new List<Vector3>();
    public List<Vector3> lookOffsets = new List<Vector3>();
    public List<float> distances = new List<float>();

    // Orbit camera DEV
    public float distanceFromCenterOrbit = 8f;
    public float lookAroundSpeed = 20f;
    public Vector3 OffsetOribt = Vector3.zero;

    private Vector3 nextPosRig;
    private Vector3 nextPosCam;
    [SerializeField] private float RotAroundY = -90f;
    [SerializeField] private float RotAroundXZ = 75f;

    int i = 0;

    private void Awake()
    {
        InitializeCameraController();
    }

    void LateUpdate()
    {
        if (target)
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                if(cameraMode == CameraMode.normal)
                {
                    cameraMode = CameraMode.orbit;
                }
                else
                {
                    cameraMode = CameraMode.normal;
                }
            }

            if (cameraMode == CameraMode.normal)
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
            else if(cameraMode == CameraMode.orbit)
            {
                /*
                 *  x = r * cos(s) * sin(t)
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

                nextPosRig = target.position + target.up * OffsetOribt.y + target.right * -OffsetOribt.x + target.forward * OffsetOribt.z;

                float XPosCam = distanceFromCenterOrbit * Mathf.Cos(RotAroundY * Mathf.Deg2Rad) * Mathf.Sin(RotAroundXZ * Mathf.Deg2Rad);
                float ZPosCam = distanceFromCenterOrbit * Mathf.Sin(RotAroundY * Mathf.Deg2Rad) * Mathf.Sin(RotAroundXZ * Mathf.Deg2Rad);
                float YPosCam = distanceFromCenterOrbit * Mathf.Cos(RotAroundXZ * Mathf.Deg2Rad);
                nextPosCam = new Vector3(XPosCam, YPosCam, ZPosCam);

                cameraTransform.localPosition = nextPosCam;
                cameraRigTransform.position = nextPosRig;
                cameraTransform.LookAt(target.position + target.up * OffsetOribt.y + target.right * -OffsetOribt.x + target.forward * OffsetOribt.z, Vector3.up);
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
        cameraRigTransform = Instantiate(cameraRigPrefab).transform;
        cameraTransform = cameraRigTransform.GetComponentInChildren<Camera>().transform;
    }

    public void SetTargetVehicle(Transform t)
    {
        target = t;
    }

    public Transform GetCameraRigTransform()
    {
        if (cameraRigTransform)
        {
            return cameraRigTransform;
        }
        return null;
    }
}