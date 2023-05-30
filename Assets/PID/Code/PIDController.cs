using UnityEngine;
namespace PID_Controller
{
    public class PIDController : MonoBehaviour
    {
        // Gains
        [Header("Gains")]
        [SerializeField] float Kp = 2.0f;
        [SerializeField] float Ki = 0.5f;
        [SerializeField] float Kd = 0.25f;

        // Low Pass filter for the derivative
        [Header("Derevative filter")]
        [SerializeField] float tau = 0.02f;

        // Output Min and Max
        [Header("Output min and max")]
        [SerializeField] float minVal = -10f;
        [SerializeField] float maxVal = 10f;

        // Integrator Min and Max
        [Header("Integrator min and max")]
        [SerializeField] float minValInt = -5f;
        [SerializeField] float maxValInt = 5f;

        // Runtime
        float integrator;
        float prevError;
        float diffrentiator;
        float prevMeasurement;

        [Space]
        // Output
        [Header("Output")]
        [SerializeField] float output;

        /// <summary>
        /// Initialize the PID controller
        /// </summary>
        public void PID_Init()
        {
            integrator = 0f;
            prevError = 0f;
            diffrentiator = 0f;
            output = 0f;
        }

        /// <summary>
        /// Update or tick the PID controller
        /// </summary>
        /// <returns>output</returns>
        public float PID_Update(float setpoint, float measurement, float deltaTime)
        {
            // Get current error
            float error = setpoint - measurement;

            // Proportional
            float proportional = Kp * error;

            // Integral
            integrator = integrator + 0.5f * Ki * deltaTime * (error + prevError);
            integrator = Mathf.Clamp(integrator, minValInt, maxValInt);

            // Dereviative
            diffrentiator = -(2.0f * Kd * (measurement - prevMeasurement)
                            + (2.0f * tau - deltaTime) * diffrentiator)
                            / (2.0f * tau + deltaTime);

            // Output calculation and clamp
            output = proportional + integrator + diffrentiator;
            output = Mathf.Clamp(output, minVal, maxVal);

            // Store prev as current
            prevError = error;
            prevMeasurement = measurement;

            return output;
        }
    }
}
