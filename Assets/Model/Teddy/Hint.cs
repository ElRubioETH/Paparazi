using TMPro;
using UnityEngine;
using System.Collections;
using System;

public class Hint : MonoBehaviour
{
    [Header("UI & Text")]
    public string dialogue = "Dialogue";
    public TextMeshProUGUI dialogueText; // Gắn từ Inspector
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dialogueClip;
    [Header("Settings")]
    public float timer = 2f;
    public void Show()
    {
        if (dialogueText != null)
        {
            dialogueText.enabled = true;
            dialogueText.text = dialogue;
            if (audioSource != null && dialogueClip != null)
                audioSource.PlayOneShot(dialogueClip);

            StartCoroutine(DisableText());
        }
        else
        {
            Debug.LogWarning("Dialogue Text is not assigned!");
        }
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);

        dialogueText.enabled = false;

    }
}
