using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEngine : VehicleSystem
{
    /*
     * Works as long as it is under water
     */

    [Header("Engine")]
    [SerializeField] private float maxThrust = 3000;
    [SerializeField] private float maxYaw = 45f;
    [SerializeField] private Transform engineTransform = null;

    private float throttleInput = 0f;
    private float yawInput = 0f;
    private int reverse = 1;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (!engineTransform)
        {
            return;
        }
        if (EnvironmentSystem.instance) 
        {
            if (transform.position.y <= EnvironmentSystem.instance.GetSeaLineYPos())
            {
                rigid.AddForceAtPosition(engineTransform.forward * maxThrust * throttleInput * reverse, engineTransform.position, ForceMode.Force);
            }
        }
        engineTransform.localEulerAngles = new Vector3(0, maxYaw * -yawInput , 0);
    }

    public void SetThrottle(float t)
    {
        throttleInput = t;
    }
    public void SetYaw(float y)
    {
        yawInput = y;
    }
    public void ToggleReverse()
    {
        reverse *= -1;
    }
    #region VEHICLESYSTEMGUI
    protected override void InitializeGUIElements()
    {
        vehicleGUIElements.Add(new VehicleGUIElement("Throttle", "%", "0.0", showGUIElements));
    }
    protected override void UpdateGUIElements()
    {
        vehicleGUIElements[0].SetValue((throttleInput * 100).ToString("0"));
    }
    #endregion
}
