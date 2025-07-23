using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Gắn từ Inspector

    public void Show(string message)
    {
        if (dialogueText != null)
        {
            dialogueText.text = message;
            dialogueText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Dialogue Text is not assigned!");
        }
    }

    public void Hide()
    {
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }
}
