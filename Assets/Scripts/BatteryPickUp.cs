using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BatteryPickUp : MonoBehaviour
{
    private bool inReach;
    public GameObject pickUpText;
    public AudioSource pickUpSound;

    public GameObject player; // <- Kéo object Player vào đây
    private FlashlightAdvanced flashlightScript;

    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);

        flashlightScript = player.GetComponent<FlashlightAdvanced>();
        if (flashlightScript == null)
        {
            Debug.LogWarning("FlashlightAdvanced không được tìm thấy trên Player!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            pickUpText.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && inReach)
        {
            pickUpSound.Play();
            flashlightScript.batteries += 1;
            inReach = false;
            pickUpText.SetActive(false);
            Destroy(gameObject);
        }
    }
}
