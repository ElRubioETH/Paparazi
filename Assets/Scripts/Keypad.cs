﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Keypad : MonoBehaviour
{

    private bool hasPlayed = false;
    public AudioSource audioSource;
    public GameObject ToDestroy;

    public GameObject player;
    public GameObject keypadOB;
    public GameObject hud;
    public GameObject inv;


    public GameObject animateOB;
    public Animator ANI;
    public GameObject keypadText;

    public Text textOB;
    public string answer = "12345";

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;


    void Start()
    {
        keypadOB.SetActive(false);

    }


    public void Number(int number)
    {
        textOB.text += number.ToString();
        button.Play();
    }

    public void Execute()
    {
        if (textOB.text == answer)
        {
            correct.Play();
            textOB.text = "Right";

        }
        else
        {
            wrong.Play();
            textOB.text = "Wrong";
        }


    }

    public void Clear()
    {
        {
            textOB.text = "";
            button.Play();
        }
    }

    public void Exit()
    {
        if( hasPlayed == false)
        {
            audioSource.Play();
        }
        keypadOB.SetActive(false);
        inv.SetActive(true);
        hud.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (textOB.text == "Right" && animate && !hasPlayed)
        {
            keypadText.GetComponent<OpenKeyPad>().enabled = false;
            hasPlayed = true;
            ANI.SetBool("animate", true);
            Debug.Log("its open");

            Destroy(ToDestroy);
        }


        if(keypadOB.activeInHierarchy)
        {
            hud.SetActive(false);
            inv.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }


}
