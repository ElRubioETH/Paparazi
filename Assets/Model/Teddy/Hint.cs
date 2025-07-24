using TMPro;
using UnityEngine;
using System.Collections;

public class Hint : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Gắn từ Inspector
    public GameObject Dialuoge;
    public void Show(string message)
    {
        if (dialogueText != null)
        {
            dialogueText.text = message;
            dialogueText.gameObject.SetActive(true);
            Dialuoge.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Dialogue Text is not assigned!");
        }
    }

    public void HideAfterDelay()
    {
        StartCoroutine(HideCoroutine());
    }

    IEnumerator HideCoroutine()
    {
        yield return new WaitForSeconds(3f); // delay 3 giây
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
            Dialuoge.SetActive(false);
        }
    }
}
