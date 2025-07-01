using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLight : MonoBehaviour
{
    public GameObject light;
    public GameObject onText;
    public GameObject offText;

    private bool inReach;

    private bool lightIsOn;


    void Start()
    {
        onText.SetActive(false);
        offText.SetActive(false);
        light.SetActive(false);

        lightIsOn = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && lightIsOn)
        {
            inReach = true;
            offText.SetActive(true);
        }

        if (other.gameObject.tag == "Reach" && !lightIsOn)
        {
            inReach = true;
            onText.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            offText.SetActive(false);
            onText.SetActive(false);
        }
    }




    void Update()
    {
        if (Input.GetButtonDown("Interact") && inReach && !lightIsOn)
        {
            light.SetActive(true);
            onText.SetActive(false);
            lightIsOn = true;
        }

        else if (Input.GetButtonDown("Interact") && inReach && lightIsOn)
        {
            light.SetActive(false);
            offText.SetActive(false);
            lightIsOn = false;
        }

    }
}
