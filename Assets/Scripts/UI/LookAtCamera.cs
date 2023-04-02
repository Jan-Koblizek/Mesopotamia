using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position, -Vector3.up);
        transform.eulerAngles = new Vector3(-transform.eulerAngles.x, 0, 0);
    }
}
