using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCandles : MonoBehaviour
{
    public GameObject lighterOB;
    public GameObject flame;
    public GameObject lightText;

    public bool unlit;
    public bool inReach;



    void Start()
    {
        unlit = true;
        flame.SetActive(false);
        lightText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && unlit)
        {
            inReach = true;
            lightText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach" && unlit)
        {
            inReach = false;
            lightText.SetActive(false);
        }
    }




    void Update()
    {
        if(lighterOB.activeInHierarchy && inReach && unlit && Input.GetButtonDown("Interact"))
        {
            flame.SetActive(true);
            lightText.SetActive(false);
            unlit = false;

        }

    }
}
