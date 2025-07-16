using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    public int frameLimit;

    void Update()
    {
        Application.targetFrameRate = frameLimit;
    }
}
