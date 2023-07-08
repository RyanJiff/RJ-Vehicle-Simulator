using UnityEngine;
using System.Collections.Generic;
public class SimpleVehicleGUI : MonoBehaviour
{
	/*
     *  Simple Vehicle GUI used by the PlayerController
     */
	
	[Header("GUI properties")]
	[SerializeField] Vector2 startOffset = new Vector2(10, 40);
	[SerializeField] Vector2 sizeOfInfoRect = new Vector2(300, 20);
	[SerializeField] Vector2 spacingOfInfoRect = new Vector2(0, 20);
	[SerializeField] float suffixXOffset = 30f;
	[Space]

	private Vehicle vehicle = null;
	private List<VehicleSystem> vehicleSystems = new List<VehicleSystem>();

	private void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 18;
		style.fontStyle = FontStyle.Bold;
		style.alignment = TextAnchor.MiddleLeft;

		int yCount = 0;

		if (vehicle)
		{
			// To display all vehicle system info we have to go through all of the vehicle systems and append all information neccesary to the GUI
			for(int i = 0;i < vehicleSystems.Count; i++)
            {
				// Each vehicle system has a list of VehicleGUIElement
				List<VehicleGUIElement> vehicleGUIElements = vehicleSystems[i].GetGUIElements();
				for(int j = 0;j < vehicleGUIElements.Count; j++)
                {
					// Is this GUI element showable?
                    if (vehicleGUIElements[j].GetShowOnGUI())
                    {
						// Here we draw the GUI element
						style.alignment = TextAnchor.MiddleLeft;
						GUI.Label(new Rect(startOffset.x, startOffset.y + spacingOfInfoRect.y * yCount, sizeOfInfoRect.x, sizeOfInfoRect.y), vehicleGUIElements[j].GetGUIPrefix(), style);
						style.alignment = TextAnchor.MiddleRight;
						GUI.Label(new Rect(startOffset.x + suffixXOffset, startOffset.y + spacingOfInfoRect.y * yCount, sizeOfInfoRect.x, sizeOfInfoRect.y), vehicleGUIElements[j].GetValue() + " " + vehicleGUIElements[j].GetGUISuffix(), style);
						style.alignment = TextAnchor.MiddleLeft;
						yCount++;
					}
                }
            }
		}
	}

	public void SetVehicle(Vehicle v)
    {
		vehicle = v;
		vehicleSystems = v.GetAllVehicleSystems();
    }
}
