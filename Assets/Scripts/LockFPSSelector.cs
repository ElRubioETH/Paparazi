using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFPSSelector : MonoBehaviour
{
    public int frameRateOption1;
    public int frameRateOption2;
    public int frameRateOption3;


    public void FrameRate1()
    {
        Application.targetFrameRate = frameRateOption1;
    }

    public void FrameRate2()
    {
        Application.targetFrameRate = frameRateOption2;
    }

    public void FrameRate3()
    {
        Application.targetFrameRate = frameRateOption3;
    }
}
