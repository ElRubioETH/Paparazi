using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int requiredBoxes = 5;
    public Animator doorAnimator;
    public GameObject lever; // để bật tương tác sau khi đủ
    public AudioSource DoorOpen;
    private int fixedCount = 0;
    private bool isUnlocked = false;

    void Start()
    {
        if (lever != null)
            lever.SetActive(false); // ẩn lever ban đầu
    }

    public void ReportBoxFixed()
    {
        fixedCount++;
        Debug.Log($"Đã sửa {fixedCount}/{requiredBoxes} hộp.");

        if (fixedCount >= requiredBoxes && !isUnlocked)
        {
            isUnlocked = true;
            if (lever != null)
                lever.SetActive(true);
            Debug.Log("Tất cả hộp đã sửa. Cần gạt đã bật.");
        }
    }
    public bool IsUnlocked()
    {
        return fixedCount >= requiredBoxes;
    }

    public void PullLever()
    {
        if (isUnlocked && doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            Debug.Log("Đã mở cửa!");
            DoorOpen.Play();
        }
    }
}
