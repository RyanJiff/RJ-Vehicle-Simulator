using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInitialVelocity : MonoBehaviour
{
    /*
     * Not meant to be used outside of development.
     * Sets the velocity of the gameobject set in inspector on start. The velocity always points towards the forward axis of the gameobject
     */

    public GameObject objectToAffect;
    public float speedToSet = 10f;

    void Start()
    {
        Rigidbody rigid = objectToAffect.GetComponent<Rigidbody>();
        if (rigid)
        {
            rigid.velocity = objectToAffect.transform.forward * speedToSet;
        }    
    }
}
