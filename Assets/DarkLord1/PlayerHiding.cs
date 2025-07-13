using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public bool isHiding = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Closet"))
        {
            isHiding = true;
            Debug.Log("Player is hiding in closet!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Closet"))
        {
            isHiding = false;
            Debug.Log("Player left closet!");
        }
    }
}