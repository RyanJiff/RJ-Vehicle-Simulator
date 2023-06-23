using UnityEngine;
using UnityEditor;

public class Wing : MonoBehaviour
{
	/*
     * This is a modified version of SimpleWing by Brian Hernandez
     * Wing script that applies a force based on inputted parameters and velocity.
     */

	[Tooltip("Size of the wing. The bigger the wing, the more lift it provides.")]
	public Vector2 dimensions = new Vector2(5f, 2f);
	[Tooltip("Lift coefficient curve.")]
	public WingCurves wing;
	[Tooltip("The higher the value, the more lift the wing applie at a given angle of attack.")]
	public float liftMultiplier = 1f;
	[Tooltip("The higher the value, the more drag the wing incurs at a given angle of attack.")]
	public float dragMultiplier = 1f;
	[Tooltip("The amount of calculation points on the wing")]
	public int numberOfCalculationPoints = 10;

	private Rigidbody rigid;

	private Vector3 liftDirection = Vector3.up;
	private Vector3 globalWindVector = Vector3.zero;

	private Vector3 rigidVelocity = Vector3.zero;
	private Vector3 worldVelocity = Vector3.zero;
	private Vector3 localVelocity = Vector3.zero;

	private float liftCoefficient = 0f;
	private float dragCoefficient = 0f;
	private float liftForce = 0;
	private float dragForce = 0;
	private float angleOfAttack = 0;
	private Vector3 calculationPointPosition = Vector3.zero;
    private float controlSurfaceDeflection = 0f;
	//private float controlsurfaceLiftMultiplier = 1f;
	//private float controlsurfaceDragMultiplier = 1f;
	private float controlSurfaceAOACoefficientEffectStrength = 0;

	// If the origin shifted we will calculate based on last frame velocity because the shift will cause a spike in values.
	private Vector3 originShiftBy = Vector3.zero;

	public float WingArea
	{
		get { return dimensions.x * dimensions.y; }
	}

	public Rigidbody Rigidbody
	{
		set { rigid = value; }
	}

	private void Awake()
	{
		// I don't especially like doing this, but there are many cases where wings might not
		// have the rigidbody on themselves (e.g. they are on a child gameobject of a plane).
		rigid = GetComponentInParent<Rigidbody>();

		FloatingOrigin.OnOriginShiftEnded.AddListener(OriginShiftingEnded);
	}

	private void Start()
	{
		if (rigid == null)
		{
			Debug.LogError(name + ": Wing has no rigidbody on self or parent!");
		}

		if (wing == null)
		{
			Debug.LogError(name + ": Wing has no defined wing curves!");
		}
	}

	private void Update()
	{
		// Prevent division by zero.
		if (dimensions.x <= 0f)
			dimensions.x = 0.01f;
		if (dimensions.y <= 0f)
			dimensions.y = 0.01f;
	}

	private void FixedUpdate()
	{
		if (rigid != null && wing != null)
		{
			globalWindVector = Vector3.zero;

			// Check if we have an environment system singleton, will contain any global calculation modifiers
			if (EnvironmentSystem.instance)
			{
				globalWindVector = EnvironmentSystem.instance.globalWindVector;
			}

			for (int i = 0; i < numberOfCalculationPoints; i++)
			{

				calculationPointPosition = transform.position + (-transform.right * (dimensions.x / 2)) + ((transform.right * (dimensions.x / (numberOfCalculationPoints - 1))) * i);

				// Origin shift hack
				calculationPointPosition += originShiftBy;

				Vector3 forceApplyPos = calculationPointPosition;

				rigidVelocity = rigid.velocity;

				worldVelocity = rigid.GetPointVelocity(calculationPointPosition);
				worldVelocity += -globalWindVector;

				localVelocity = transform.InverseTransformDirection(worldVelocity);
				localVelocity.x = 0f;

				// Angle of attack is used as the look up for the lift and drag curves. We also add the control surface lift Coefficient to the the liftCoefficient to affect the lift curve verticaly.
				// TODO: When we change the control surface angle we should probably shift the lift curve to the right, not just upwards.
				angleOfAttack = Vector3.Angle(Vector3.forward, localVelocity);
				liftCoefficient = wing.GetLiftAtAOA(angleOfAttack) + ((controlSurfaceDeflection / 45f) * Mathf.Sign(localVelocity.y) * controlSurfaceAOACoefficientEffectStrength);
				dragCoefficient = wing.GetDragAtAOA(angleOfAttack);
				
				// Here I tried to make the control surface change the wing Drag/Lift multiplier, this was not a very good idea. Below is the implementation attempt

				// Drag Multiplier based on control surface deflection
				//controlsurfaceDragMultiplier = 1 + (Mathf.Abs((controlSurfaceDeflection / 45f)) * controlSurfaceEffectStrengthMult);
				// Lift Multiplier based on control surface deflection
				// Multiplying by the sign of localVelocity.y will make sure the lift Multiplier behaves corectly in cases where the wing is pushing the opposite way.
				//controlsurfaceLiftMultiplier = 1 + (controlSurfaceDeflection / 45f) * Mathf.Sign(localVelocity.y * controlSurfaceEffectStrengthMult);

				// Calculate lift/drag.
				liftForce = localVelocity.sqrMagnitude * liftCoefficient * WingArea * liftMultiplier / numberOfCalculationPoints;
				dragForce = localVelocity.sqrMagnitude * dragCoefficient * WingArea * dragMultiplier / numberOfCalculationPoints;

				// Vector3.Angle always returns a positive value, so add the sign back in.
				liftForce *= -Mathf.Sign(localVelocity.y);

				// Lift is always perpendicular to air flow.
				liftDirection = Vector3.Cross(rigidVelocity - globalWindVector, transform.right).normalized;
				rigid.AddForceAtPosition(liftDirection * liftForce, forceApplyPos, ForceMode.Force);

				// Drag is always opposite of the wind velocity.
				rigid.AddForceAtPosition((-rigidVelocity + globalWindVector).normalized * dragForce, forceApplyPos, ForceMode.Force);
				
				// DEBUG
				Debug.DrawRay(calculationPointPosition, liftDirection * liftForce * 0.005f, Color.blue);
				Debug.DrawRay(calculationPointPosition, -rigidVelocity.normalized * dragForce * 0.001f, Color.red);
				//Debug.DrawRay(calculationPointPosition, rigid.velocity * 0.01f, Color.yellow);
			}
		}
		
		originShiftBy = Vector3.zero;
		
	}
	public void SetControlSurfaceDeflection(float deflectionAngle, float strength)
    {
		// Deflection angle cant be more than 45 degrees at the meantime, this is because I am not sure whether to to keep increasing lift or not past this point
		// TODO: Needs more research and improvement
		float clampedAngle = Mathf.Clamp(deflectionAngle, -45, 45);
		controlSurfaceDeflection = clampedAngle;
		controlSurfaceAOACoefficientEffectStrength = strength;
	}
	public float GetAOA()
    {
		if (rigid != null)
		{
			globalWindVector = Vector3.zero;
			// Check if we have an environment system singleton, will contain any global calculation modifiers
			if (EnvironmentSystem.instance)
			{
				globalWindVector = EnvironmentSystem.instance.globalWindVector;
			}

			Vector3 worldVelocity = rigid.GetPointVelocity(transform.position);
			worldVelocity += -globalWindVector;

			Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
			return angleOfAttack * -Mathf.Sign(localVelocity.y);
		}
		else
		{
			return 0.0f;
		}
	}
	public void OriginShiftingEnded(Vector3 v)
    {
		Debug.Log("Origin shifted!");
		originShiftBy = v;
    }

	// Prevent this code from throwing errors in a built game.
	#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Matrix4x4 oldMatrix = Gizmos.matrix;

		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

		Gizmos.DrawWireCube(Vector3.zero, new Vector3(dimensions.x, 0f, dimensions.y));

		Gizmos.matrix = oldMatrix;
	}
	#endif
}