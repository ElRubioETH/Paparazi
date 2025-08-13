using UnityEngine;

public class TriggerInteract : MonoBehaviour
{
    public Animator playerAnimator; // Kéo Animator của Player vào đây trong Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Chỉ kích hoạt khi Player chạm
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Open");
            }
        }
    }
}
