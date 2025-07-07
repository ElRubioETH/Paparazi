using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FixCar : MonoBehaviour
{
    public GameObject fix;
    public bool inReach;

    public GameObject[] Tools;
    public PlayableDirector timeline;

    public GameObject miniGameUI; // ← Kéo UI MiniGame vào

    private bool miniGameIsOpen = false;

    void Start()
    {
        inReach = false;
        if (fix != null) fix.SetActive(false);
        if (miniGameUI != null) miniGameUI.SetActive(false);
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
        if (Input.GetButtonDown("Interact") && inReach && AllToolsActive() && !miniGameIsOpen)
        {
            OpenMiniGame(); // ← Gọi mini-game
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

    public void OnMiniGameSuccess()
    {
        if (timeline != null)
        {
            timeline.Play();
        }
        Destroy(gameObject);
        if (fix != null) fix.SetActive(false);
    }

    public void OnMiniGameFail()
    {
        // Đóng mini-game, cho retry
        if (miniGameUI != null) miniGameUI.SetActive(false);
        miniGameIsOpen = false;
    }

    void OpenMiniGame()
    {
        if (miniGameUI != null)
        {
            miniGameUI.SetActive(true);
            miniGameIsOpen = true;
        }
    }
}
