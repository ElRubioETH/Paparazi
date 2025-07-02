using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject hud;
    public GameObject inv;
    public GameObject deathScreen;
    public GameObject player;

    public float health = 100f;
    private float currentHealth;



    void Start()
    {
        deathScreen.SetActive(false);
        currentHealth = maxHealth;

    }

    public float maxHealth = 100f;
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        Debug.Log("Player took damage! Current HP: " + health);
        if (health <= 0)
        {
            Debug.Log("Player died!");
        }
    }
    void Update()
    {

        if(health <= 0)
        {
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            hud.SetActive(false);
            inv.SetActive(false);
            deathScreen.SetActive(true);
        }

        if (health > 100)
        {
            health = 100;
        }
        
    }
}
