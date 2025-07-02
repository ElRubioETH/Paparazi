using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtObjects : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip dialogueClip;

    public bool inReach;


    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;

        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            if (audioSource != null && dialogueClip != null)
                audioSource.PlayOneShot(dialogueClip);
        }   

    }
}
