using UnityEngine;

public class FogSwitcher : MonoBehaviour
{
    public bool enableFog = true;

    void OnEnable()
    {
        RenderSettings.fog = enableFog;
    }
}
