﻿using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator door;
    public GameObject openText;
    public AudioSource doorSound;

    public bool inReach;
    public bool isOpen = false;
    public bool IsOpen => isOpen;

    void Start()
    {
        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            openText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            openText.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;

        door.SetBool("Open", isOpen);
        door.SetBool("Closed", !isOpen);

        if (doorSound != null)
            doorSound.Play();

        Debug.Log(isOpen ? "It Opens" : "It Closes");
    }
}
