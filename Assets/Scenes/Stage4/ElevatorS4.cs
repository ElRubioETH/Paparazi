using UnityEngine;

public class OpenAndDestroy : MonoBehaviour
{
    public Animator animator; // Animator của object
    public float destroyDelay = 2f;     // Thời gian chờ trước khi destroy


    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            if (animator != null)
            {
                animator.SetTrigger("Open");
            }

            Destroy(gameObject, destroyDelay);
        }
    }
}
