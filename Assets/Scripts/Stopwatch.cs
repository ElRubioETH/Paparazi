using System;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    float currentTime;
    public Text currentTimeText;


    void Start()
    {
        currentTime = 0;
    }


    void Update()
    {
        currentTime = currentTime + Time.deltaTime;

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = time.ToString(@"mm\:ss\:fff");

    }
}
