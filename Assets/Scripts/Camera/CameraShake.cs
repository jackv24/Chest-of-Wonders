//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Camera mainCamera;

    float shakeAmt;

    public float TestIntensity = 0.1f;
    public float TestDuration = 0.2f;

    public bool EnableXShake = true;
    public bool EnableYShake = true;

    
    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; //sets this camera to follow the tagged Main Camera if unspecified
        }
    }
    

    void Update() //currently just for testing
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShakeCam(TestIntensity, TestDuration);
        }
    }

    public void ShakeCam(float amt, float duration)
    {
        shakeAmt = amt;
        InvokeRepeating("StartShake", 0, 0.001f);
        Invoke("StopShake", duration);
    }

    void StartShake()
    {
        if (shakeAmt > 0)
        {
            Vector3 camPos = mainCamera.transform.position;

            float shakeX = Random.value * shakeAmt * 2 - shakeAmt;
            float shakeY = Random.value * shakeAmt * 2 - shakeAmt;

            if (EnableXShake)
            {
                camPos.x += shakeX;
            }

            if (EnableYShake)
            {
                camPos.y += shakeY;
            }

            mainCamera.transform.position = camPos;
        }
    }

    void StopShake()
    {
        Vector3 camPos = mainCamera.transform.position;
        CancelInvoke("StartShake");
        mainCamera.transform.localPosition = camPos;
    }
}
