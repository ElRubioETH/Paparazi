using UnityEngine;
using TMPro;

public class ElectricBox : MonoBehaviour
{
    [Header("Mini Game Settings")]
    [SerializeField, Tooltip("Container chứa các thành phần của mini-game")] private GameObject miniGameContainer;
    [SerializeField, Tooltip("TextMeshPro để hiển thị thông báo tương tác")] private TextMeshProUGUI interactionText;
    [SerializeField, Tooltip("Khoảng cách tối đa để tương tác với hộp điện")] private float interactionDistance = 2f;

    private bool isPlayerInRange; // Kiểm tra người chơi có trong phạm vi không
    private bool hasWon; // Trạng thái chiến thắng mini-game
    private bool isMiniGameActive; // Kiểm tra mini-game đang active
    private GameObject player; // Tham chiếu đến người chơi
    private FixGame miniGameScript; // Tham chiếu đến script FixGame
    private NewDoors door; // Tham chiếu đến cửa
    [SerializeField] private MeshRenderer signalLight;
    [SerializeField] private Material onMaterial;
    [SerializeField] private DoorController doorController;

    private void Awake()
    {
        hasWon = false;
        isPlayerInRange = false;
        isMiniGameActive = false;
        Debug.Log($"Awake ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})");
    }

    private void Start()
    {
        // Tìm người chơi bằng tag
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError($"Không tìm thấy GameObject với tag 'Player' trong cảnh cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})!");
            return;
        }

        // Kiểm tra và khởi tạo miniGameContainer
        if (miniGameContainer == null)
        {
            Debug.LogError($"miniGameContainer chưa được gán trong Inspector cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})!");
            return;
        }

        miniGameScript = miniGameContainer.GetComponent<FixGame>();
        if (miniGameScript == null)
        {
            Debug.LogError($"Không tìm thấy script FixGame trong miniGameContainer cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})!");
        }
        else
        {
            // Đăng ký sự kiện khi mini-game kết thúc
            miniGameScript.OnGameEnded += HandleGameEnded;
            Debug.Log($"Đã đăng ký sự kiện OnGameEnded cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()}), FixGame InstanceID: {miniGameScript.GetInstanceID()}");
        }
        miniGameContainer.SetActive(false); // Tắt mini-game lúc khởi động

        // Kiểm tra interactionText
        if (interactionText == null)
        {
            Debug.LogError($"interactionText chưa được gán trong Inspector cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})!");
        }
        else
        {
            UpdateInteractionText("");
        }
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ
        if (miniGameScript != null)
        {
            miniGameScript.OnGameEnded -= HandleGameEnded;
            Debug.Log($"Hủy đăng ký sự kiện OnGameEnded cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})");
        }
    }

    private void Update()
    {
        if (player == null || miniGameContainer == null || miniGameScript == null) return;

        // Kiểm tra khoảng cách đến người chơi
        float distance = Vector3.Distance(transform.position, player.transform.position);
        isPlayerInRange = distance <= interactionDistance;

        // Hiển thị thông báo tương tác
        if (isPlayerInRange)
        {
            if (!hasWon && !isMiniGameActive)
            {
                UpdateInteractionText("Nhấn 'Q' để sửa");
                Debug.Log($"Hiển thị thông báo 'Nhấn Q để sửa' cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()}), distance: {distance}");
            }
            else if (hasWon)
            {
                UpdateInteractionText("Hộp điện đã sửa!");
            }
            else
            {
                UpdateInteractionText("");
            }
        }
        else
        {
            UpdateInteractionText("");
        }

        // Kích hoạt mini-game khi nhấn Q, nếu chưa thắng
        if (isPlayerInRange && !hasWon && Input.GetKeyDown(KeyCode.Q) && !isMiniGameActive)
        {
            StartMiniGame();
        }
    }

    /// <summary>
    /// Kích hoạt mini-game
    /// </summary>
    private void StartMiniGame()
    {
        if (miniGameContainer != null && miniGameScript != null)
        {
            isMiniGameActive = true;
            miniGameContainer.SetActive(true);
            UpdateInteractionText("");
            miniGameScript.ResetGame();
            Debug.Log($"Mini-game được kích hoạt cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()}), FixGame InstanceID: {miniGameScript.GetInstanceID()}");
        }
    }

    /// <summary>
    /// Xử lý khi mini-game kết thúc
    /// </summary>
    private void HandleGameEnded()
    {
        if (!isMiniGameActive)
        {
            Debug.Log($"HandleGameEnded bị gọi nhưng mini-game không active cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})!");
            return;
        }

        if (miniGameScript != null && miniGameScript.IsWon() && !hasWon)
        {
            hasWon = true;
            isMiniGameActive = false;
            // 🔴 Đổi màu đèn signal
            if (signalLight != null && onMaterial != null)
            {
                signalLight.material = onMaterial;
                Debug.Log($"Đã đổi signal light thành ON cho {gameObject.name}");
            }

            // 🟢 Thông báo cho hệ thống cửa
            if (doorController != null)
            {
                doorController.ReportBoxFixed();
            }
        }

        if (miniGameContainer != null)
        {
            miniGameContainer.SetActive(false);
            isMiniGameActive = false;
        }
        Debug.Log($"HandleGameEnded called for ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()}), hasWon: {hasWon}, isMiniGameActive: {isMiniGameActive}");
    }

    /// <summary>
    /// Đăng ký cửa để nhận thông báo khi hộp được sửa
    /// </summary>
    public void RegisterDoor(NewDoors door)
    {
        this.door = door;
        Debug.Log($"NewDoors đã được đăng ký cho ElectricBox: {gameObject.name} (InstanceID: {GetInstanceID()})");
    }

    /// <summary>
    /// Cập nhật văn bản tương tác trên UI
    /// </summary>
    private void UpdateInteractionText(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
            Debug.Log($"Cập nhật UI cho ElectricBox {gameObject.name} (InstanceID: {GetInstanceID()}): {message}, isActive: {interactionText.gameObject.activeSelf}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}