using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public Transform mainCamera;
    public GameObject uiPanel;
    public GameObject startButton;
    public GameObject timeRemaning;
    public RoadBehavior roadBehavior;
    public Transform road;
    public float torqueForce;
    public float brakeForce;
    public float maxSpeed;
    public WheelCollider frontWheelCollider;
    public WheelCollider rearWheelCollider;
    public Transform frontWheelTransform;
    public Transform rearWheelTransform;
    public Text speedText;
    public RectTransform speedPointer;
    public bool isStart = false;

    private Vector3 lastPos;
    private Vector3 startPos;
    private float currentSpeed;
    private float width;
    private float startAngle;
    private float targetAngle;
    private float leanAngle;
    private float turnLeanAngle;
    private float beganTouchXPos;
    private float xDifference;
    private float currentBreakForce;
    private bool isBrake;

    void Start()
    {
        width = Screen.width;
        startAngle = transform.localEulerAngles.y;
        startPos = transform.position;
    }

    void Update()
    {
        currentSpeed = (int)(-GetComponent<Rigidbody>().velocity.x * 5);
        if (Input.touchCount > 0)
        {
            isBrake = false;
            if (isStart)
            {
                if (-GetComponent<Rigidbody>().velocity.x * 5 < maxSpeed)
                {
                    frontWheelCollider.motorTorque = torqueForce;
                    rearWheelCollider.motorTorque = torqueForce;
                }
                else
                {
                    frontWheelCollider.motorTorque = 0;
                    rearWheelCollider.motorTorque = 0;
                }
            }
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                beganTouchXPos = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (isStart)
                {
                    xDifference = touch.position.x - beganTouchXPos;
                }
            }
        }
        else
        {
            isBrake = true;
            leanAngle = 0;
            turnLeanAngle = 0;
            xDifference = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isStart)
        {
            BrakeControl();
            MotorAngleWithPath();
            MovementHorizontal();
            UIChange();

            UpdateWheel(frontWheelCollider, frontWheelTransform);
            UpdateWheel(rearWheelCollider, rearWheelTransform);
        }
    }

    private void UIChange()
    {
        speedText.text = currentSpeed.ToString();
        speedPointer.localRotation = Quaternion.Lerp(speedPointer.localRotation, Quaternion.Euler(0, 0, 120 - currentSpeed * 3 / 2), Time.deltaTime * 5);
    }

    private void MovementHorizontal()
    {
        turnLeanAngle = -100 * xDifference / width;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + xDifference / (width * 2));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, turnLeanAngle), Time.deltaTime);
    }

    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void MotorAngleWithPath()
    {
        leanAngle = 2 * -(targetAngle - startAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, leanAngle), Time.deltaTime);
    }

    private void BrakeControl()
    {
        currentBreakForce = isBrake ? brakeForce : 0;
        frontWheelCollider.brakeTorque = currentBreakForce;
        rearWheelCollider.brakeTorque = currentBreakForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Block")
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * currentSpeed * 5000);
        }
        else if (other.tag == "Road")
        {
            targetAngle = startAngle + roadBehavior.targetRoadAngle[other.transform.GetSiblingIndex()];
        }
        else if (other.tag == "FallGround")
        {
            xDifference = 0;
            transform.position = lastPos;
        }
        else if (other.tag == "Finish")
        {
            frontWheelCollider.motorTorque = 0;
            rearWheelCollider.motorTorque = 0;
            GetComponent<Rigidbody>().Sleep();
            OpenMenuPanel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Road")
        {
            lastPos = other.transform.position;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (Mathf.Abs(transform.localEulerAngles.y - targetAngle) > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, targetAngle, transform.rotation.z), Time.deltaTime);
        }
    }

    private void OpenMenuPanel()
    {
        isStart = false;
        timeRemaning.SetActive(true);
        transform.position = startPos;
        mainCamera.localPosition = new Vector3(0, 7, -1.5f);
        uiPanel.SetActive(false);
        startButton.transform.GetChild(0).GetComponent<Text>().text = "Yeniden Basla";
        startButton.SetActive(true);
    }
}
