using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightAdvanced : MonoBehaviour
{
    public GameObject flashlight;

    public Slider flashlightSlider; // <- Slider mới
    public float drainRate = 0.25f; // đơn vị %/giây

    public TMP_Text batteryText;

    public float lifetime = 100;
    public float batteries = 0;

    public AudioSource flashON;
    public AudioSource flashOFF;

    private bool on;
    private bool off;

    void Start()
    {
        off = true;
        flashlight.SetActive(false);

        flashlightSlider.maxValue = 100;
        flashlightSlider.minValue = 0;
        flashlightSlider.value = lifetime;
    }

    void Update()
    {
        flashlightSlider.value = lifetime;
        batteryText.text = batteries.ToString();

        if (Input.GetButtonDown("flashlight") && off)
        {
            flashON.Play();
            flashlight.SetActive(true);
            on = true;
            off = false;
        }
        else if (Input.GetButtonDown("flashlight") && on)
        {
            flashOFF.Play();
            flashlight.SetActive(false);
            on = false;
            off = true;
        }

        if (on)
        {
            lifetime -= drainRate * Time.deltaTime;
        }

        if (lifetime <= 0)
        {
            flashlight.SetActive(false);
            on = false;
            off = true;
            lifetime = 0;
        }

        if (lifetime >= 100)
        {
            lifetime = 100;
        }

        if (Input.GetButtonDown("Reload") && batteries >= 1)
        {
            batteries -= 1;
            lifetime += 50;
        }

        if (Input.GetButtonDown("Reload") && batteries == 0)
        {
            return;
        }

        if (batteries <= 0)
        {
            batteries = 0;
        }
    }
}
