using UnityEngine;

public class SimpleVehicleGUI : MonoBehaviour
{
	/*
     *  written in 5 minutes, needs a rewrite to be modular.
     */
	public Vehicle vehicle = null;

	bool showControls = false;

    private void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 16;
		style.fontStyle = FontStyle.Bold;

		Airplane airplane = vehicle.GetComponent<Airplane>();

		if (airplane)
		{
			GUI.Label(new Rect(10, 40, 300, 20), string.Format("Speed: {0:0} kn", airplane.GroundSpeed()), style);
			GUI.Label(new Rect(10, 60, 300, 20), string.Format("Throttle: {0:0}%", airplane.GetAxisInput(Enums.AxisInput.THROTTLE) * 100.0f), style);		
			GUI.Label(new Rect(10, 80, 300, 20), string.Format("Flaps: {0:0}%", airplane.GetCurrentFlapLevel() * 100.0f), style);
			GUI.Label(new Rect(10, 100, 300, 20), string.Format("Altitude: {0:0} Feet", Mathf.Abs(airplane.transform.position.y) * 3.28084f), style);
			GUI.Label(new Rect(10, 120, 300, 20), string.Format("VSI: {0:0} Feet Per Minute", airplane.VerticalSpeed()), style);
			GUI.Label(new Rect(10, 140, 300, 20), string.Format("Elevator Trim: {0:0.00} ", airplane.GetTrim(Enums.Axis.VERTICAL)), style);
			GUI.Label(new Rect(10, 160, 300, 20), string.Format("Pitch: {0:0}", airplane.pitch), style);
			GUI.Label(new Rect(10, 180, 300, 20), string.Format("Fuel: {0:0.0} Litres", airplane.GetTotalFuelAmount(true)), style);

			if (showControls)
			{
				GUI.Label(new Rect(10, 220, 300, 20), "Throttle Control(1,2) Flaps(3,4) Brakes(B) Engine Toggle(I) Toggle Mouse Yoke(Y)", style);
				GUI.Label(new Rect(10, 240, 400, 20), "Elevator(W,S) Aileron(A,D) Rudder(Q,E) Trim(-,+)" , style);
				GUI.Label(new Rect(10, 260, 300, 20), "Switch Camera Mode(Numpad5) Look Around Orbit Camera(Numpad)", style);
				GUI.Label(new Rect(10, 280, 300, 20), "R TO RELOAD SCENE", style);
			}

			if(GUI.Button(new Rect(10, 200, 200, 20), "Show Controls"))
			{
				showControls = !showControls;
            }
		}
	}
}
