using UnityEngine;

public class ControlSurface : VehicleSystem
{
	public enum ControlType { STATIC, PITCH, ROLL, YAW};
	public ControlType controlType = ControlType.STATIC;

	[Header("Deflection")]

	[Tooltip("Deflection with max positive input."), Range(0, 90)]
	public float max = 15f;

	[Tooltip("Deflection with max negative input"), Range(0, 90)]
	public float min = 15f;

	[Tooltip("Speed of the control surface deflection.")]
	public float moveSpeed = 90f;

	[Tooltip("Requested deflection of the control surface normalized to [-1, 1]. "), Range(-1, 1)]
	public float targetDeflection = 0f;

	[Tooltip("When set to true the target deflection is multiplied by -1")]
	public bool inverted = false;
	[Space]

	[Header("Affects Parent Wing")]
	[SerializeField] private bool affectsWing = false;
	[SerializeField][Range(0, 0.5f)]  float effectOnAOACoeffecient = 0.25f;
	[Space]

	[Header("Speed Stiffening")]

	[Tooltip("Should we stiffen based on velocity?")]
	[SerializeField] private bool stiffenWithSpeed = false;

	[Tooltip("How much force the control surface can exert. The lower this is, " +
		"the more the control surface stiffens with speed.")]
	public float maxTorque = 6000f;
	[Space]

	private Rigidbody rigid = null;
	private Quaternion startLocalRotation = Quaternion.identity;

	[SerializeField] private Wing myWing;

	private float angle = 0f;

    protected override void VehicleSystemAwake()
    {
        base.VehicleSystemAwake();

		if (affectsWing)
		{
			myWing = GetComponentInParent<Wing>();
			rigid = GetComponentInParent<Rigidbody>();
		}
	}
    protected override void VehicleSystemStart()
    {
        base.VehicleSystemStart();

		// Dirty hack so that the rotation can be reset before applying the deflection.
		startLocalRotation = transform.localRotation;
	}
	protected override void VehicleSystemFixedUpdate()
	{
		base.VehicleSystemFixedUpdate();

		// -1 if invert is true
		float targetDeflectionInvert = 1 * targetDeflection;
        if (inverted)
        {
			targetDeflectionInvert = -1 * targetDeflection;
        }

		// Different angles depending on positive or negative deflection.
		float targetAngle = targetDeflectionInvert > 0f ? targetDeflectionInvert * max : targetDeflectionInvert * min;

		// How much you can deflect, depends on how much force it would take
		if (rigid != null && stiffenWithSpeed && rigid.velocity.sqrMagnitude > 1f)
		{
			float torqueAtMaxDeflection = rigid.velocity.sqrMagnitude * myWing.WingArea;
			float maxAvailableDeflection = Mathf.Asin(maxTorque / torqueAtMaxDeflection) * Mathf.Rad2Deg;

			// Asin(x) where x > 1 or x < -1 is not a number.
			if (float.IsNaN(maxAvailableDeflection) == false)
				targetAngle *= Mathf.Clamp01(maxAvailableDeflection);
		}

		// Move the control surface.
		angle = Mathf.MoveTowards(angle, targetAngle, moveSpeed * Time.fixedDeltaTime);

		// Hacky way to do this!
		transform.localRotation = startLocalRotation;
		transform.Rotate(Vector3.right, angle, Space.Self);

        // Tell the wing we control our current angle
        if (affectsWing && myWing) 
		{
			myWing.SetControlSurfaceDeflection(angle, effectOnAOACoeffecient);
		}
	}
}