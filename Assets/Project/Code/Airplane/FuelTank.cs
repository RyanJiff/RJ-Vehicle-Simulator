using UnityEngine;

public class FuelTank : VehicleSystem
{
    /*
     *Fuel tank to make engines run if they require fuel
     */
    public enum FuelValveState { OPEN, CLOSED}

    [SerializeField] private float maxAmountLitres = 200f;
    [SerializeField] private float currentFuelAmountLitres = 100f;
    [SerializeField] private FuelValveState fuelValve = FuelValveState.OPEN;
    
    /// <summary>
    /// Returns true if fuel tank is empty
    /// </summary>
    public bool IsDry()
    {
        return currentFuelAmountLitres <= 0;
    }
    /// <summary>
    /// Change the current amount of fuel by amount. Clamps from 0 to max capacity.
    /// </summary>
    public void ChangeAmount(float amount)
    {
        currentFuelAmountLitres += amount;
        currentFuelAmountLitres = Mathf.Clamp(currentFuelAmountLitres, 0.0f, maxAmountLitres);
    }
    /// <summary>
    /// Set the current amount of fuel manually. Clamps from 0 to max capacity.
    /// </summary>
    public void SetAmount(float amount)
    {
        currentFuelAmountLitres = Mathf.Clamp(amount, 0.0f, maxAmountLitres);
    }
    public float GetCurrentFuelAmount()
    {
        return currentFuelAmountLitres;
    }
    public float GetMaxFuelAmount()
    {
        return maxAmountLitres;
    }
    /// <summary>
    /// Returns whether the fuel valve is open or not.
    /// </summary>
    public bool IsFuelValveOpen()
    {
        return fuelValve == FuelValveState.OPEN;
    }
    /// <summary>
    /// Externally set fuel valve state
    /// </summary>
    public void SetFuelValveState(FuelValveState fv)
    {
        fuelValve = fv;
    }
    protected override void InitializeGUIElements()
    {
        vehicleGUIElements.Add(new VehicleGUIElement("Fuel", "L", "0.0", showGUIElements));
    }
    protected override void UpdateGUIElements()
    {
        vehicleGUIElements[0].SetValue((currentFuelAmountLitres).ToString("0.0"));
    }
}
