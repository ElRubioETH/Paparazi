using UnityEngine;
using TMPro;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ElectricBox : MonoBehaviour
{
    [Header("Mini Game Settings")]
    [SerializeField, Tooltip("Container chứa các thành phần của mini-game")] private GameObject miniGameContainer;
    [SerializeField, Tooltip("TextMeshPro để hiển thị thông báo tương tác")] private TextMeshProUGUI interactionText;
    [SerializeField, Tooltip("Khoảng cách tối đa để tương tác với hộp điện")] private float interactionDistance = 2f;

    private bool isPlayerInRange; // Kiểm tra người chơi có trong phạm vi không
    private bool hasWon; // Trạng thái chiến thắng mini-game
    private GameObject player; // Tham chiếu đến người chơi
    private FixGame miniGameScript; // Tham chiếu đến script FixGame
    public GameObject InteractText;
    private bool inReach;
    private void Start()
    {
        // Tìm người chơi bằng tag
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Không tìm thấy GameObject với tag 'Player' trong cảnh!");
            return;
        }

        // Tìm script FixGame trong miniGameContainer
        if (miniGameContainer != null)
        {
            miniGameScript = miniGameContainer.GetComponent<FixGame>();
            if (miniGameScript == null)
            {
                Debug.LogError("Không tìm thấy script FixGame trong miniGameContainer!");
            }
            else
            {
                // Đăng ký sự kiện khi mini-game kết thúc
                miniGameScript.OnGameEnded += HandleGameEnded;
            }
        }
        else
        {
            Debug.LogError("miniGameContainer chưa được gán trong Inspector!");
        }

        // Tắt mini-game lúc khởi động
        if (miniGameContainer != null)
        {
            miniGameContainer.SetActive(false);
        }

        // Khởi tạo trạng thái
        hasWon = false;
        UpdateInteractionText("");
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ
        if (miniGameScript != null)
        {
            miniGameScript.OnGameEnded -= HandleGameEnded;
        }
    }

    private void Update()
    {
        if (player == null || miniGameContainer == null) return;

        // Kiểm tra khoảng cách đến người chơi
        float distance = Vector3.Distance(transform.position, player.transform.position);
        isPlayerInRange = distance <= interactionDistance;

        // Hiển thị thông báo tương tác
        if (isPlayerInRange)
        {
            if (!hasWon && !miniGameContainer.activeSelf)
            {
                UpdateInteractionText("Nhấn 'Q' để Sửa");
            }
            else if (hasWon)
            {
                UpdateInteractionText("Sửa đã xong!");
            }
            else
            {
                UpdateInteractionText("");
            }
        }
        else
        {
            UpdateInteractionText(""); // Xóa thông báo khi người chơi rời vùng
        }

        // Kích hoạt mini-game khi nhấn Q, nếu chưa thắng
        if (isPlayerInRange && !hasWon && Input.GetKeyDown(KeyCode.Q) && !miniGameContainer.activeSelf)
        {
            StartMiniGame();
        }
    }

    /// <summary>
    /// Kích hoạt mini-game
    /// </summary>
    private void StartMiniGame()
    {
        miniGameContainer.SetActive(true);
        UpdateInteractionText("");
        if (miniGameScript != null)
        {
            miniGameScript.ResetGame(); // Đặt lại mini-game
        }
        Debug.Log("Mini-game được kích hoạt!");
    }

    /// <summary>
    /// Xử lý khi mini-game kết thúc
    /// </summary>
    private void HandleGameEnded()
    {
        if (miniGameScript != null && miniGameScript.IsWon())
        {
            hasWon = true;
        }
        miniGameContainer.SetActive(false);
    }

    /// <summary>
    /// Cập nhật văn bản tương tác trên UI
    /// </summary>
    private void UpdateInteractionText(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
        }
    }

    private void OnDrawGizmos()
    {
        // Vẽ vùng tương tác trong Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            InteractText.SetActive(true);
            UpdateInteractionText("");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            InteractText.SetActive(false);
        }
    }



}