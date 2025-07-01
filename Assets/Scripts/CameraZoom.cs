using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera cameraView;

    public float initialFOV;
    public float zoomInFOV;

    private float currentFOV;

    void Start()
    {
        cameraView.fieldOfView = initialFOV;
        cameraView = GameObject.Find("cameraView").GetComponent<Camera>();
    }


    void Update()
    {

        if (Input.GetButton("Fire2"))
        {
            cameraView.fieldOfView = zoomInFOV;
        }

        else
        {
            cameraView.fieldOfView = initialFOV;
        }




        currentFOV = Camera.main.fieldOfView;

    }
}