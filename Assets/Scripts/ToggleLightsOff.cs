using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLightsOff : MonoBehaviour
{
    public GameObject lightswitchOB;
    public GameObject lightOB;

    public float useTimes = 3;

    void OnTriggerEnter(Collider other)
    {
        if (lightOB.activeInHierarchy && other.gameObject.tag == "Player")
        {
            LightSwitch LightSwitch = lightswitchOB.GetComponent<LightSwitch>();
            LightSwitch.lightOB.SetActive(false);
            LightSwitch.onOB.SetActive(false);
            LightSwitch.offOB.SetActive(true);
            LightSwitch.switchClick.Play();
            LightSwitch.lightsAreOff = true;
            LightSwitch.lightsAreOn = false;
            useTimes -= 1;
        }
    }

    void Update()
    {
        if(useTimes == 0)
        {
            Destroy(gameObject);
        }
        
    }
}
