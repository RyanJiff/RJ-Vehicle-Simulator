using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCamera : MonoBehaviour
{
    public Transform target;
    public List<Vector3> offsets = new List<Vector3>();
    public List<Vector3> lookOffsets = new List<Vector3>();

    int i = 0;

    void LateUpdate()
    {
        if (target)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                i = (i + 1) % offsets.Count;
            }
            Vector3 nextPos;
            if (Input.GetKey(KeyCode.C))
            {
                nextPos = target.forward * 10 + target.position + target.TransformDirection(offsets[i]);
            }
            else
            {
                nextPos = target.forward * -10 + target.position + target.TransformDirection(offsets[i]);
            }

            transform.position = nextPos;
            transform.LookAt(target.position + target.up * lookOffsets[i].y + target.right * -lookOffsets[i].x + target.forward * lookOffsets[i].z, target.up);
        }
    }
}