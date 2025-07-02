using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [Header("UI & Text")]
    public TextMeshProUGUI textOB;
    public string dialogue = "Dialogue";

    [Header("Settings")]
    public float timer = 2f;
    public GameObject Activator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dialogueClip;

    [Header("Event Called After Dialogue Ends")]
    public UnityEvent onDialogueEnd;

    void Start()
    {
        textOB.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textOB.enabled = true;
            textOB.text = dialogue;

            // Phát âm thanh nếu có
            if (audioSource != null && dialogueClip != null)
                audioSource.PlayOneShot(dialogueClip);

            StartCoroutine(DisableText());
        }
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);

        textOB.enabled = false;
        Destroy(Activator);

        if (onDialogueEnd != null)
            onDialogueEnd.Invoke();
    }
}
