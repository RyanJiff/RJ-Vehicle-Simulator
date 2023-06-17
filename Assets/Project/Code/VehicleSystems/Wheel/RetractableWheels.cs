using System.Collections.Generic;
using UnityEngine;

public class RetractableWheels : VehicleSystem
{
    /*
     * Retractable Wheels script resposible for handling wheel retract/extend logic
     * has some editor tools for helping get/set positions of up down.
     * Needs to be documeted better
     */

    public List<Transform> transforms = new List<Transform>();
    public List<WheelCollider> wheelColliders = new List<WheelCollider>();

    [Header("UP POS AND ROT")]
    public List<Vector3> UpPositions = new List<Vector3>();
    public List<Quaternion> UpRotations = new List<Quaternion>();
    [Space]
    [Header("DOWN POS AND ROT")]
    public List<Vector3> DownPositions = new List<Vector3>();
    public List<Quaternion> DownRotations = new List<Quaternion>();
    [Space]
    [Header("EDITOR ONLY")]
    public Vector3 GottenPos = Vector3.zero;
    public Quaternion GottenRot = Quaternion.identity;
    [Space]
    public Transform toGetTransform;

    public bool extended = true;
    public float speed = 0.5f;
    void Update()
    {
        if (!extended)
        {
            for(int i = 0; i < transforms.Count; i++)
            {
                transforms[i].localPosition = Vector3.MoveTowards(transforms[i].localPosition, UpPositions[i], speed * Time.deltaTime);
                transforms[i].localRotation = Quaternion.RotateTowards(transforms[i].localRotation, UpRotations[i], speed * Time.deltaTime);
            }
        }
        else
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                transforms[i].localPosition = Vector3.MoveTowards(transforms[i].localPosition, DownPositions[i], speed * Time.deltaTime);
                transforms[i].localRotation = Quaternion.RotateTowards(transforms[i].localRotation, DownRotations[i], speed * Time.deltaTime);
            }
            foreach (WheelCollider w in wheelColliders)
            {
                w.enabled = true;
            }
        }
    }

    /// <summary>
    /// Toggle if wheels are extended
    /// </summary>
    public void ToggleExtendableWheels()
    {
        extended = !extended;
    }
    public void GetPosRot()
    {
        GottenRot = toGetTransform.localRotation;
        GottenPos = toGetTransform.localPosition;
    }

    /// <summary>
    /// Instantly extend wheels
    /// </summary>
    public void WheelsDownInstant()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].localPosition = DownPositions[i];
            transforms[i].localRotation = DownRotations[i];
        }
        
        extended = true;
    }

    /// <summary>
    /// Instantly retract wheels
    /// </summary>
    public void WheelsUpInstant()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].localPosition =  UpPositions[i];
            transforms[i].localRotation =  UpRotations[i];
        }
        
        extended = false;
    }
}
