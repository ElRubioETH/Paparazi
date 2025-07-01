using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    public GameObject pickupOB;
    public GameObject activatingOB;

    public GameObject pickupText;

    public bool inReach;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickupText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            pickupText.SetActive(false);
        }
    }


    void Update()
    {
        if(inReach && Input.GetButtonDown("Interact"))
        {
            pickupOB.SetActive(false);
            activatingOB.SetActive(true);
            pickupText.SetActive(false);
        }
        
    }
}
