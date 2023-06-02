using UnityEngine;

public class SimpleVehicleGUI : MonoBehaviour
{
	/*
     *  written in 5 minutes, needs a rewrite to be modular.
     */
	public Vehicle vehicle = null;

	[SerializeField] Vector2 startOffset = new Vector2(10, 40);
	[SerializeField] Vector2 sizeOfInfoRect = new Vector2(300, 20);
	[SerializeField] Vector2 spacingOfInfoRect = new Vector2(0, 20);
	bool showControls = false;

    private void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 18;
		style.fontStyle = FontStyle.Bold;

		Airplane airplane = vehicle.GetComponent<Airplane>();

		if (airplane)
		{
			int y = 0;

			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Speed: {0:0} kn", airplane.GroundSpeed()), style);
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Throttle: {0:0}%", airplane.GetAxisInput(Enums.AxisInput.THROTTLE) * 100.0f), style);		
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Flaps: {0:0}%", airplane.GetCurrentFlapLevel() * 100.0f), style);
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Altitude: {0:0} Feet", Mathf.Abs(airplane.transform.position.y) * 3.28084f), style);
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("VSI: {0:0} Feet Per Minute", airplane.VerticalSpeed()), style);
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Elevator Trim: {0:0.00} ", airplane.GetTrim(Enums.Axis.VERTICAL)), style);
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Pitch: {0:0}", airplane.pitch), style);
			GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), string.Format("Fuel: {0:0.0} Litres", airplane.GetTotalFuelAmount(true)), style);

			if (GUI.Button(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), "Show Controls"))
			{
				showControls = !showControls;
			}
			if (showControls)
			{
				GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), "Throttle Control(1,2) Flaps(3,4) Brakes(B) Engine Toggle(I) Toggle Mouse Yoke(Y)", style);
				GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), "Elevator(W,S) Aileron(A,D) Rudder(Q,E) Trim(-,+)" , style);
				GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), "Switch Camera Mode(Numpad5) Look Around Orbit Camera(Numpad)", style);
				GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * y++, sizeOfInfoRect.x, sizeOfInfoRect.y), "R TO RELOAD SCENE", style);
			}
		}
	}
}
