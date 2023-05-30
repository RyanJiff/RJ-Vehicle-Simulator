using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    public int fpsLimit = 60;


    void Awake()
    {
        Application.targetFrameRate = fpsLimit;
    }
}
