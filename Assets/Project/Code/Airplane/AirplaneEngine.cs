using UnityEngine;
using System.Collections.Generic;
public class AirplaneEngine : MonoBehaviour
{
    /*
     * Simple airplane engine that adds a force based on inputted values
     */

    [Header("Engine")]
    [SerializeField] private bool ignition = false;
    [SerializeField] private float fuelUsePerSecondAtMaxPower = 0.012f;
    [SerializeField] private bool requireFuel = true;
    [SerializeField] private Transform engineTransform;
    [SerializeField] private float maxThrust = 3000;
    [SerializeField] private float idleInput = 0.05f;
    [SerializeField] private float rampSpeed = 0.5f;
    [SerializeField] private AnimationCurve thrustAirspeedCurve;

    [Header("Visual")]
    [SerializeField] private float RPM = 0;
    [SerializeField] private AnimationCurve RPMCurve;
    [SerializeField] private float RPMMult = 3000;
    [SerializeField] private Transform engineAnimationMesh;

    [Header("Debug")]
    [SerializeField] float currentThrust; [Range(0.0f, 1.0f)]
    [SerializeField] float currentPower = 0.0f;

    [Range(0.0f, 1.0f)] private float throttleInput = 0.0f;
    private Rigidbody rigid;
    private List<FuelTank> fuelTanks = new List<FuelTank>();
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
        fuelTanks = myAirplane.GetFuelTanks();
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
            rigid.AddForceAtPosition(engineTransform.forward * currentPower * maxThrust * thrustAirspeedCurve.Evaluate(rigid.velocity.magnitude * msToKnots), engineTransform.position, ForceMode.Force);
            currentThrust = currentPower * maxThrust * thrustAirspeedCurve.Evaluate(rigid.velocity.magnitude * msToKnots);
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
        if (requireFuel)
        {
            // Check how many tanks we can use
            int numberOfTanksWithOpenValves = 0;
            for(int i = 0;i < fuelTanks.Count; i++)
            {
                if (fuelTanks[i].IsFuelValveOpen())
                {
                    numberOfTanksWithOpenValves++;
                }
            }

            // Fuel consumption calculations
            float calculatedFuelUse = ((Mathf.Abs(fuelUsePerSecondAtMaxPower) * -1)/numberOfTanksWithOpenValves) * currentPower * Time.deltaTime;
            bool allDry = true;
            for (int i = 0; i < fuelTanks.Count; i++)
            {
                if (fuelTanks[i].IsFuelValveOpen())
                {
                    fuelTanks[i].ChangeAmount(calculatedFuelUse);
                    if (!fuelTanks[i].IsDry())
                    {
                        allDry = false;
                    }
                }
            }
            if (allDry)
            {
                // A real engine would still have ignition on if there is no fuel, it would just not have any combustion.
                // Need to re write the Airplane Engine to be a little bit more realistic and maybe implement temperatures and different systems? 
                ignition = false;
            }
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
}
