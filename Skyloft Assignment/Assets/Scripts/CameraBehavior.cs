using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform motor;

    private float zAngle;
    private float xPos;

    void Update()
    {
        zAngle = motor.localEulerAngles.z;
        if(zAngle > 180)
        {
            xPos = -(360 - zAngle);
        }
        else
        {
            xPos = zAngle;
        }
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, -xPos / 10, -xPos);
        transform.localPosition = new Vector3(xPos / 100, transform.localPosition.y, transform.localPosition.z);
    }
}
