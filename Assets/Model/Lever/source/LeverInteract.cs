using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public LeverPuzzle puzzleManager;
    public int leverIndex;
    public GameObject interactText;

    [Header("Audio Settings")]
    public AudioSource leverSound; // âm thanh khi gạt cần

    private bool inReach = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (interactText != null)
                interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (interactText != null)
                interactText.SetActive(false);
        }
    }

    private void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            // Phát âm thanh gạt cần
            if (leverSound != null)
                leverSound.Play();

            // Báo cho PuzzleManager
            puzzleManager.PullLever(leverIndex);

            if (interactText != null)
                interactText.SetActive(false);
        }
    }
}
