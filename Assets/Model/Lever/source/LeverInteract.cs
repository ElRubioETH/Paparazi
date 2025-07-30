using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public LeverPuzzle puzzleManager;
    public int leverIndex;
    public GameObject interactText;

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
            puzzleManager.PullLever(leverIndex);
            if (interactText != null)
                interactText.SetActive(false);
        }
    }
}
