using UnityEngine;

public class GateLeverInteraction : MonoBehaviour
{
    public DoorController doorController;
    private bool canPull = true;

    public GameObject InteractText;
    public bool inReach;
    public Animator LeverAnimation;
    public bool Electricfy = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            InteractText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            InteractText.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact") && canPull)
        {
            if (doorController != null && doorController.IsUnlocked()) // ✅ Kiểm tra đủ box chưa
            {
                doorController.PullLever();
                LeverAnimation.SetTrigger("Pull");
                canPull = false;
                InteractText.SetActive(false);
                Electricfy = true;
            }
            else
            {
                Debug.Log("Chưa đủ số hộp sửa, không thể gạt cần.");
            }
        }
    }
}
