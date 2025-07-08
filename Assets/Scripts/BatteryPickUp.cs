using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BatteryPickUp : MonoBehaviour
{
    private bool inReach;
    public TMP_Text Battery;
    public GameObject pickUpText;
    public GameObject flashlight;

    public AudioSource pickUpSound;

    private FlashlightAdvanced flashlightScript;

    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        flashlightScript = flashlight.GetComponent<FlashlightAdvanced>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
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
            flashlightScript.batteries += 1; // 👉 Cập nhật vào script đèn pin
            inReach = false;
            pickUpText.SetActive(false);
            Destroy(gameObject);
        }
    }
}