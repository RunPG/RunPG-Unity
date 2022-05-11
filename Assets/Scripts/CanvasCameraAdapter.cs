using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraAdapter : MonoBehaviour
{
    void Start()
    {
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        transform.rotation = cam.transform.rotation;
    }
}
