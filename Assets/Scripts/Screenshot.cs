using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("Screenshot");
            ScreenCapture.CaptureScreenshot("Screenshot.png");
        }
    }
}