using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public GameObject hud;
    public GameObject inv;
    public GameObject deathScreen;
    public GameObject player;

    public float health = 100f;
    private float currentHealth;

    public Slider healthSlider;

    void Start()
    {
        deathScreen.SetActive(false);
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

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
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
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
