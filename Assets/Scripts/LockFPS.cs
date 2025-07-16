using UnityEngine;

public class LockFPS : MonoBehaviour
{
    public int frameRate;


    void Update()
    {
        Application.targetFrameRate = frameRate;
    }
}
