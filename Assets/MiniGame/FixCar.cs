using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;  // ← THÊM DÒNG NÀY

public class FixCar : MonoBehaviour
{
    public GameObject fix;
    public bool inReach;

    public GameObject[] Tools;
    public PlayableDirector timeline;  // ← Kéo Timeline vào đây

    void Start()
    {
        inReach = false;
        if (fix != null) fix.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (fix != null) fix.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (fix != null) fix.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && inReach && AllToolsActive())
        {
            if (timeline != null)
            {
                timeline.Play();  // ← CHẠY TIMELINE
            }
        }
    }

    bool AllToolsActive()
    {
        foreach (GameObject tool in Tools)
        {
            if (tool == null || !tool.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

}
