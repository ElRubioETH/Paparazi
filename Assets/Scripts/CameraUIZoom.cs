using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIZoom : MonoBehaviour
{
    public Animator cam;

    void Update()
    {
        if(Input.GetButton("ZoomButton"))
        {
            cam.SetBool("Zoom", true);
        }

        else
        {
            cam.SetBool("Zoom", false);
        }
        
    }
}
