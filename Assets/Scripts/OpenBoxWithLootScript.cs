using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBoxWithLootScript : MonoBehaviour
{
    public Animator boxOB;
    public GameObject keyOBNeeded;
    public GameObject openText;
    public GameObject keyMissingText;
    public AudioSource openSound;

    public GameObject drop1;
    public GameObject drop2;
    public GameObject drop3;
    public GameObject drop4;
    public GameObject drop5;
    public GameObject drop6;

    public bool inReach;
    public bool isOpen;

    public int randomNumber;



    void Start()
    {
        randomNumber = Random.Range(0, 5);
        inReach = false;
        openText.SetActive(false);
        keyMissingText.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            openText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
            keyMissingText.SetActive(false);
        }
    }


    void Update()
    {
        if (!isOpen && keyOBNeeded.activeInHierarchy && inReach && Input.GetButtonDown("Interact"))
        {
            keyOBNeeded.SetActive(false);
            openSound.Play();
            boxOB.SetBool("open", true);
            openText.SetActive(false);
            keyMissingText.SetActive(false);
            isOpen = true;

            switch (randomNumber)
            {
                case 0: drop1.SetActive(true); break;
                case 1: drop2.SetActive(true); break;
                case 2: drop3.SetActive(true); break;
                case 3: drop4.SetActive(true); break;
                case 4: drop5.SetActive(true); break;
                case 5: drop6.SetActive(true); break;
            }
        }

        else if (!isOpen && !keyOBNeeded.activeInHierarchy && inReach && Input.GetButtonDown("Interact"))
        {
            openText.SetActive(false);
            keyMissingText.SetActive(true);
        }

        if (isOpen)
        {
            boxOB.GetComponent<BoxCollider>().enabled = false;
            boxOB.GetComponent<OpenBoxScript>().enabled = false;
        }
    }

}
