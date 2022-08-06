using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{
    public Transform mainCamera;
    public Movement movement;
    public Text coutdownText;

    private bool isStart = false;
    private float timeRemaining = 4;

    void Update()
    {
        if (isStart)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                coutdownText.text = ((int)timeRemaining).ToString();
            }
            else
            {
                coutdownText.gameObject.SetActive(false);
                timeRemaining = 4;
                isStart = false;
            }
            mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, new Vector3(0, 0.7f, -1.4f), Time.deltaTime * 2);
        }
    }

    public void PressStart()
    {
        isStart = true;
        StartCoroutine(Wait3second());
    }

    IEnumerator Wait3second()
    {
        yield return new WaitForSeconds(timeRemaining);
        movement.isStart = true;
    }
}
