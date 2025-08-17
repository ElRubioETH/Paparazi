using UnityEngine;

public class LeverPuzzle : MonoBehaviour
{
    [Header("Lever Animators")]
    public Animator[] leverAnimators = new Animator[4];

    [Header("Door Animator")]
    public Animator doorAnimator;

    [Header("Target Pull Counts")]
    public int[] correctCounts = new int[4] { 2, 4, 1, 5 };

    private int[] currentCounts = new int[4];
    private bool[] leverStates = new bool[4]; // true = up, false = down

    [Header("Audio")]
    public AudioSource errorSound;
    public AudioSource doorOpenSound; // âm thanh mở cửa

    public void PullLever(int index)
    {
        if (index < 0 || index >= 4) return;

        // Toggle lever state
        leverStates[index] = !leverStates[index];

        // Play corresponding animation
        leverAnimators[index].SetTrigger(leverStates[index] ? "Up" : "Down");

        // Increase pull count
        currentCounts[index]++;

        // Check if this lever was pulled too many times
        if (currentCounts[index] > correctCounts[index])
        {
            Debug.LogWarning($"Lever {index + 1} bị gạt quá số lần cho phép!");
            if (errorSound) errorSound.Play();
            ResetAllLevers();
            return;
        }

        // Check full solution
        if (CheckPuzzleSolved())
        {
            Debug.Log("Puzzle completed thành công!");
            doorAnimator.SetTrigger("Open");

            // Phát âm thanh mở cửa
            if (doorOpenSound != null)
                doorOpenSound.Play();
        }
    }

    private bool CheckPuzzleSolved()
    {
        for (int i = 0; i < 4; i++)
        {
            if (currentCounts[i] != correctCounts[i])
                return false;
        }
        return true;
    }

    private void ResetAllLevers()
    {
        for (int i = 0; i < 4; i++)
        {
            currentCounts[i] = 0;

            // Only animate down if lever is up
            if (leverStates[i])
            {
                leverAnimators[i].SetTrigger("Down");
            }

            leverStates[i] = false;
        }
    }
}
