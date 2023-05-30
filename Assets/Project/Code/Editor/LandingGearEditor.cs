using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LandingGear))]
public class LandingGearEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LandingGear myGearScript = (LandingGear)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Get Pos and Rot"))
        {
            myGearScript.GetPosRot();
        }
        if(GUILayout.Button("Gear Up"))
        {
            myGearScript.GearUpInstant();
        }
        if (GUILayout.Button("Gear Down"))
        {
            myGearScript.GearDownInstant();
        }

    }

}
