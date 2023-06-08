using UnityEngine;

public class SimpleWing : MonoBehaviour
{
	/*
     * README in script folder for credits
     * Simple wing script that applies a force based on parameters/velocity/environmentsystem.
     */

	[Tooltip("Size of the wing. The bigger the wing, the more lift it provides.")]
	public Vector2 dimensions = new Vector2(5f, 2f);
	[Tooltip("When true, wing forces will be applied only at the center of mass.")]
	public bool applyForcesToCenter = false;
	[Tooltip("Lift coefficient curve.")]
	public WingCurves wing;
	[Tooltip("The higher the value, the more lift the wing applie at a given angle of attack.")]
	public float liftMultiplier = 1f;
	[Tooltip("The higher the value, the more drag the wing incurs at a given angle of attack.")]
	public float dragMultiplier = 1f;

	private Rigidbody rigid;

	private Vector3 liftDirection = Vector3.up;
	private Vector3 globalWindVector = Vector3.zero;

	private float liftCoefficient = 0f;
	private float dragCoefficient = 0f;
	private float liftForce = 0f;
	private float dragForce = 0f;
	private float angleOfAttack = 0f;
	

	public float AngleOfAttack
	{
		get
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
	}

	public float WingArea
	{
		get { return dimensions.x * dimensions.y; }
	}

	public float LiftCoefficient { get { return liftCoefficient; } }
	public float LiftForce { get { return liftForce; } }
	public float DragCoefficient { get { return dragCoefficient; } }
	public float DragForce { get { return dragForce; } }

	public Rigidbody Rigidbody
	{
		set { rigid = value; }
	}

	private void Awake()
	{
		// I don't especially like doing this, but there are many cases where wings might not
		// have the rigidbody on themselves (e.g. they are on a child gameobject of a plane).
		rigid = GetComponentInParent<Rigidbody>();
	}

	private void Start()
	{
		if (rigid == null)
		{
			Debug.LogError(name + ": SimpleWing has no rigidbody on self or parent!");
		}

		if (wing == null)
		{
			Debug.LogError(name + ": SimpleWing has no defined wing curves!");
		}
	}

	private void Update()
	{
		// Prevent division by zero.
		if (dimensions.x <= 0f)
			dimensions.x = 0.01f;
		if (dimensions.y <= 0f)
			dimensions.y = 0.01f;

		// DEBUG
		if (rigid != null)
		{
			Debug.DrawRay(transform.position, liftDirection * liftForce * 0.001f, Color.blue);
			Debug.DrawRay(transform.position, -rigid.velocity.normalized * dragForce * 0.001f, Color.red);
			Debug.DrawRay(transform.position, rigid.velocity * 0.1f, Color.yellow);
		}
	}

	private void FixedUpdate()
	{
		if (rigid != null && wing != null)
		{
			Vector3 forceApplyPos = (applyForcesToCenter) ? rigid.transform.TransformPoint(rigid.centerOfMass) : transform.position;

			globalWindVector = Vector3.zero;

			// Check if we have an environment system singleton, will contain any global calculation modifiers
            if (EnvironmentSystem.instance)
            {
				globalWindVector = EnvironmentSystem.instance.globalWindVector;
            }

			Vector3 worldVelocity = rigid.GetPointVelocity(transform.position);
			worldVelocity += -globalWindVector;

			Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
			localVelocity.x = 0f;

			// Angle of attack is used as the look up for the lift and drag curves.
			angleOfAttack = Vector3.Angle(Vector3.forward, localVelocity);
			liftCoefficient = wing.GetLiftAtAOA(angleOfAttack);
			dragCoefficient = wing.GetDragAtAOA(angleOfAttack);

			// Calculate lift/drag.
			liftForce = localVelocity.sqrMagnitude * liftCoefficient * WingArea * liftMultiplier;
			dragForce = localVelocity.sqrMagnitude * dragCoefficient * WingArea * dragMultiplier;

			// Vector3.Angle always returns a positive value, so add the sign back in.
			liftForce *= -Mathf.Sign(localVelocity.y);

			// Lift is always perpendicular to air flow.
			liftDirection = Vector3.Cross(rigid.velocity - globalWindVector, transform.right).normalized;
			rigid.AddForceAtPosition(liftDirection * liftForce, forceApplyPos, ForceMode.Force);

			// Drag is always opposite of the wind velocity.
			rigid.AddForceAtPosition((-rigid.velocity + globalWindVector).normalized * dragForce, forceApplyPos, ForceMode.Force);
		}
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