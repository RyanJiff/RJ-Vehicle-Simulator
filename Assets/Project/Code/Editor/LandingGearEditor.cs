using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RetractableWheels))]
public class LandingGearEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RetractableWheels myRetractableWheelsScript = (RetractableWheels)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Get Pos and Rot"))
        {
            myRetractableWheelsScript.GetPosRot();
        }
        if(GUILayout.Button("Wheels Up"))
        {
            myRetractableWheelsScript.WheelsUpInstant();
        }
        if (GUILayout.Button("Wheels Down"))
        {
            myRetractableWheelsScript.WheelsDownInstant();
        }

    }

}
