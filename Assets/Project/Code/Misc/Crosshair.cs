using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture tex;
    public Transform aimPoint;

    public float size;

    private void OnGUI()
    {
        if (aimPoint && tex)
        {
            Vector2 camPos = Camera.main.WorldToScreenPoint(aimPoint.position);
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(aimPoint.position);
            if (CheckViewPortLimits(viewportPos))
            {
                GUI.DrawTexture(new Rect(new Vector2(camPos.x - size / 2, -camPos.y - size / 2 + Screen.height), new Vector2(size, size)), tex);
            }
        }
    }

    bool CheckViewPortLimits(Vector3 pos)
    {
        if(pos.z > 0)
        {
            return true;
        }
        return false;
    }
}
