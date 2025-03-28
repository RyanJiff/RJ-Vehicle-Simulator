using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Airplane))]
public class AirplaneEngine : VehicleSystem
{
    /*
     * Simple airplane engine that adds a force based on inputted values
     */

    [Header("Engine")]
    [SerializeField] private bool ignition = false;
    [SerializeField] private Transform engineTransform;
    [SerializeField] private float maxThrust = 3000;
    [SerializeField] private float idleInput = 0.05f;
    [SerializeField] private float rampSpeed = 0.5f;
    [SerializeField] private AnimationCurve thrustAirspeedCurve;
    [Space]

    [Header("Visual")]
    [SerializeField] private float RPM = 0;
    [SerializeField] private AnimationCurve RPMCurve;
    [SerializeField] private float RPMMult = 3000;
    [SerializeField] private Transform engineAnimationMesh;
    [Space]

    [Header("Debug")]
    [SerializeField] float currentThrust; [Range(0.0f, 1.0f)]
    [SerializeField] float currentPower = 0.0f;
    [Space]

    [Range(0.0f, 1.0f)] private float throttleInput = 0.0f;
    private Rigidbody rigid;
    private Airplane myAirplane;

    const float msToKnots = 1.94384f;

    void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
        myAirplane = GetComponentInParent<Airplane>();
        if (!myAirplane)
        {
            // If we don't have an airplane on us or our parents then disable the script
            Debug.LogWarning(name + ": SimpleAirplaneEngine missing Airplane!");
            enabled = false;
            return;
        }
    }
    void FixedUpdate()
    {
        if (!engineTransform)
        {
            Debug.LogWarning(name + ": No engine Transform location assigned!");
            return;
        }
        
        if (rigid != null)
        {
            rigid.AddForceAtPosition(engineTransform.forward * currentPower * maxThrust * thrustAirspeedCurve.Evaluate(rigid.linearVelocity.magnitude * msToKnots), engineTransform.position, ForceMode.Force);
            currentThrust = currentPower * maxThrust * thrustAirspeedCurve.Evaluate(rigid.linearVelocity.magnitude * msToKnots);
        }
        if (engineAnimationMesh)
        {
            engineAnimationMesh.Rotate(0, 0, RPM * 6f * Time.fixedDeltaTime);
        }
    }
    void Update()
    {
        if (ignition)
        {
            // Power calculations
            currentPower = Mathf.MoveTowards(currentPower, Mathf.Max(idleInput, throttleInput), rampSpeed * Time.deltaTime * Mathf.Abs(Mathf.Max(idleInput, throttleInput) - currentPower));
        }
        else
        {
            currentPower = Mathf.MoveTowards(currentPower, 0, rampSpeed/2 * Time.deltaTime);
        }

        // RPM Calculation, used for visual prop and for UI elements
        RPM = RPMCurve.Evaluate(currentPower) * RPMMult;
    }
    public float GetRPM()
    {
        return RPM;
    }
    /// <summary>
    /// Current power of engine, between 0.0f and 1.0f
    /// </summary>
    public float GetPower()
    {
        return currentPower;
    }
    public void SetThrottle(float t)
    {
        throttleInput = Mathf.Clamp01(t);
    }
    public void ToggleIgnition()
    {
        ignition = !ignition;
    }
    public bool IsOn()
    {
        return ignition;
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
