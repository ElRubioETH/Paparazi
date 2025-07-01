using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtObjects : MonoBehaviour
{
    public Text textOB;
    public string description = "Description";

    public bool inReach;


    void Start()
    {
        textOB.GetComponent<Text>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            textOB.GetComponent<Text>().enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            textOB.GetComponent<Text>().enabled = false;
            textOB.GetComponent<Text>().text = "";
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            textOB.text = description.ToString();
        }   

    }
}
