using UnityEngine;

public class GateLeverInteraction : MonoBehaviour
{
    public DoorController doorController;
    private bool canPull = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && canPull)
        {
            Debug.Log("Nhấn E để gạt cần");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Reach") && canPull && Input.GetKeyDown(KeyCode.E))
        {
            doorController.PullLever();
            canPull = false; // chỉ cho gạt 1 lần
        }
    }
}
