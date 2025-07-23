using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Book : MonoBehaviour
{
    public GameObject player;
    public GameObject BookUI;
    public GameObject hud;
    public GameObject inv;
    public GameObject pickUpText;
    public AudioSource pickUpSound;
    public bool inReach;

    public GameObject Symbol;

    void Start()
    {
        BookUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        pickUpText.SetActive(false);

        inReach = false;

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
            BookUI.SetActive(true);
            pickUpSound.Play();
            hud.SetActive(false);
            inv.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }


    public void ExitButton()
    {

        BookUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Symbol.SetActive(true);
    }
}
