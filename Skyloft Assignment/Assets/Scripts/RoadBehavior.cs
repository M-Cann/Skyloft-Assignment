using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBehavior : MonoBehaviour
{
    public float[] targetRoadAngle;

    void Start()
    {
        targetRoadAngle = new float[transform.childCount];
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (i > 2)
            {
                transform.GetChild(i).GetChild(0).transform.LookAt(transform.GetChild(i - 3));
                targetRoadAngle[i] = transform.GetChild(i).GetChild(0).transform.localEulerAngles.y - 270;
            }
        }
    }
}
