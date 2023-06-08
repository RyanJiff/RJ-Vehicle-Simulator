using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTorque : MonoBehaviour
{
    /*
     * quick testing
     */
    public Rigidbody toEffect;
    public float forceAmount = 100f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            toEffect.AddTorque(Vector3.right * forceAmount, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            toEffect.AddTorque(-Vector3.right * forceAmount, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.E))
        {
            toEffect.AddTorque(Vector3.forward * forceAmount, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            toEffect.AddTorque(-Vector3.forward * forceAmount, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            toEffect.AddTorque(Vector3.up * forceAmount, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            toEffect.AddTorque(Vector3.up * forceAmount, ForceMode.Force);
        }


    }
}
