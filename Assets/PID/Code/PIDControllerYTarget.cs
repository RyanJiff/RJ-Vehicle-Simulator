using UnityEngine;
namespace PID_Controller
{
    public class PIDControllerYTarget : MonoBehaviour
    {
        // Example PID controller
        [SerializeField] PIDController controller;
        [SerializeField] float currentMeasurement;
        // Transform to affect
        [SerializeField] Transform targetTransform;
        // Target Y position -- Change this to see the PID controller working. Play with the PIDController Values to see what they do.
        [SerializeField] float targetYPos = 5f;

        private void Start()
        {
            if (!controller)
            {
                enabled = false;
                return;
            }

            controller.PID_Init();
        }
        void Update()
        {
            if (!controller || !transform)
            {
                return;
            }

            // Get the current measurement, in this case it is the target transform Y position
            currentMeasurement = targetTransform.position.y;

            // Get the output from the PID system and apply it to the 
            float controllerOutput = controller.PID_Update(targetYPos, currentMeasurement, Time.deltaTime);
            targetTransform.Translate(0, controllerOutput * Time.deltaTime, 0);
        }
    }
}
