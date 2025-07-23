using UnityEngine;
using UnityEngine.UI;

public class ElectricBoxInteraction : MonoBehaviour
{
    private bool isPlayerNear = false;
    public GameObject promptText; // Đối tượng UI hiển thị "E"
    public GameObject miniGameCanvas; // Canvas chứa mini game

    void Start()
    {
        // Ẩn prompt và mini game ban đầu
        if (promptText != null) promptText.SetActive(false);
        if (miniGameCanvas != null) miniGameCanvas.SetActive(false);
    }

    void Update()
    {
        // Kiểm tra nếu người chơi nhấn "E" khi gần hộp
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (promptText != null) promptText.SetActive(false);
            if (miniGameCanvas != null) miniGameCanvas.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Phát hiện người chơi vào phạm vi
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (promptText != null) promptText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Người chơi rời khỏi phạm vi
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptText != null) promptText.SetActive(false);
            if (miniGameCanvas != null) miniGameCanvas.SetActive(false);
        }
    }
}