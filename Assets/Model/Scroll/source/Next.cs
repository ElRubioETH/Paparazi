using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Next : MonoBehaviour
{
    public GameObject pickUpText;
    public GameObject ScrollPan;
    public bool inReach;

    public PlayableDirector timeline; // Gắn timeline vào đây từ Inspector

    private bool isActivated = false;

    void Start()
    {
        pickUpText.SetActive(false);
        ScrollPan.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            pickUpText.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact") && !isActivated)
        {
            isActivated = true;
            pickUpText.SetActive(false);
            ScrollPan.SetActive(false);
            StartCoroutine(TriggerSequence());
        }
    }

    IEnumerator TriggerSequence()
    {
        yield return new WaitForSeconds(5f); // Đợi 5 giây
        ScrollPan.SetActive(true); // Mở Scroll Panel

        // Chạy timeline
        if (timeline != null)
        {
            timeline.Play();

            // Chờ timeline chạy xong
            yield return new WaitForSeconds((float)timeline.duration);
        }

        // Load scene sau khi timeline kết thúc
        SceneManager.LoadScene(3);
    }
}
