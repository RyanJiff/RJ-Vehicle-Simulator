using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class WheelVisual : MonoBehaviour
{
    /*
     * All Wheel components have a WheelVisual to display the wheel.
     */

    [Header("Wheel Visual Settings")]
    [SerializeField] private GameObject wheelVisualPrefab = null;

    private WheelCollider wheelCollider = null;
    private Transform wheelVisualTransform = null;

    private Vector3 wheelColliderPos = Vector3.zero;
    private Quaternion wheelColliderRot = Quaternion.identity;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }
    private void Update()
    {
        if (wheelVisualTransform != null)
        {
            wheelCollider.GetWorldPose(out wheelColliderPos, out wheelColliderRot);
            wheelVisualTransform.position = wheelColliderPos;
            wheelVisualTransform.rotation = wheelColliderRot;
        }
        else
        {
            if (wheelVisualPrefab)
            {
                wheelVisualTransform = Instantiate(wheelVisualPrefab, Vector3.zero, Quaternion.identity, this.transform).transform;
            }
        }
    }
}
