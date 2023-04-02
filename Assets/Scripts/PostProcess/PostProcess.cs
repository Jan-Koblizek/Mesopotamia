using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcess : MonoBehaviour
{
    public Material blackRing;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        blackRing.SetMatrix("_ViewProjectInverse", (cam.projectionMatrix * cam.worldToCameraMatrix).inverse);
        blackRing.SetTexture("_VisibilityTexture", Visibility.instance.visibilityMap);

        Graphics.Blit(source, destination, blackRing);
    }
}
