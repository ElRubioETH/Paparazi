using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public GameObject flashLightON;
    public GameObject flashLightOFF;
    public GameObject flashLightOB;

    public GameObject katana;
    public GameObject katanaOB;

    public GameObject medkit;
    public GameObject medkitOB;

    public GameObject pickaxe;
    public GameObject pickaxeOB;

    public GameObject drink;
    public GameObject drinkOB;

    void Start()
    {
        flashLightON.SetActive(false);
    }

    void Update()
    {
        if (flashLightOB.activeInHierarchy)
        {
            flashLightON.SetActive(true);
            flashLightOFF.SetActive(false);
        }
        else
        {
            flashLightON.SetActive(false);
            flashLightOFF.SetActive(true);
        }

        if (katanaOB.activeInHierarchy)
        {
            katana.SetActive(true);
        }
        else
        {
            katana.SetActive(false);
        }

        if (medkitOB.activeInHierarchy)
        {
            medkit.SetActive(true);
        }
        else
        {
            medkit.SetActive(false);
        }

        if (pickaxeOB.activeInHierarchy)
        {
            pickaxe.SetActive(true);
        }
        else
        {
            pickaxe.SetActive(false);
        }

        if (drinkOB.activeInHierarchy)
        {
            drink.SetActive(true);
        }
        else
        {
            drink.SetActive(false);
        }
    }
}
