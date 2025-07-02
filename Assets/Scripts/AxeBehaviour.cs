using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AxeBehaviour : MonoBehaviour
{
    public GameObject AxeOBNeeded;
    public GameObject DestroyText;
    public GameObject AxeMissingText;
    public AudioSource openSound;

    public bool inReach;
    public bool isOpen;



    void Start()
    {
        inReach = false;
        DestroyText.SetActive(false);
        AxeMissingText.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            DestroyText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            DestroyText.SetActive(false);
            AxeMissingText.SetActive(false);
        }
    }


    void Update()
    {
        if (AxeOBNeeded.activeInHierarchy == true && inReach && Input.GetButtonDown("Interact"))
        {
            AxeOBNeeded.SetActive(false);
            openSound.Play();
            DestroyText.SetActive(false);
            AxeMissingText.SetActive(false);
            isOpen = true;
        }

        else if (AxeOBNeeded.activeInHierarchy == false && inReach && Input.GetButtonDown("Interact"))
        {
            DestroyText.SetActive(false);
            AxeMissingText.SetActive(true);
        }

        if (isOpen)
        {
            Destroy(gameObject);
        }
    }
}
