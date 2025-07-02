using UnityEngine;

public class PlaySoundOnce : MonoBehaviour
{
    public AudioSource audioSource;
    private bool hasPlayed = false;

    public void PlayOnce()
    {
        if (!hasPlayed && audioSource != null)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
