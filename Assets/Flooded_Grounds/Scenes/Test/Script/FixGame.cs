using UnityEngine;
using TMPro;
using System;

public class FixGame : MonoBehaviour
{
    [Header("Pivots")]
    [SerializeField, Tooltip("Điểm trên cùng của đường di chuyển")] private Transform topPivot;
    [SerializeField, Tooltip("Điểm dưới cùng của đường di chuyển")] private Transform bottomPivot;

    [Header("Lightning")]
    [SerializeField, Tooltip("GameObject của lightning")] private Transform lightning;
    private float lightningPosition; // Vị trí hiện tại của lightning (0 đến 1)
    private float lightningTimer; // Bộ đếm thời gian để thay đổi vận tốc
    [SerializeField, Tooltip("Thời gian tối đa giữa các lần thay đổi vận tốc lightning")] private float timerMultiplicator = 1f;
    private float lightningSpeed; // Vận tốc hiện tại của lightning
    [SerializeField, Tooltip("Giới hạn vận tốc tối đa của lightning")] private float smoothMotion = 0.2f;
    private float noiseOffset; // Offset cho Perlin noise

    [Header("Hook")]
    [SerializeField, Tooltip("GameObject của hook")] private Transform hook;
    private float hookPosition; // Vị trí hiện tại của hook (0 đến 1)
    [SerializeField, Tooltip("Kích thước vùng hoạt động của hook")] private float hookSize = 0.15f;
    [SerializeField, Tooltip("Tốc độ tăng tiến độ khi lightning trong vùng hook")] private float hookProgressIncreasePower = 0.4f;
    private float hookProgress; // Tiến độ trò chơi (0 đến 1)
    private float hookPullVelocity; // Vận tốc kéo hook
    [SerializeField, Tooltip("Lực kéo hook khi nhấn chuột")] private float hookPullPower = 0.02f;
    [SerializeField, Tooltip("Lực trọng lực kéo hook xuống")] private float hookGravityPower = 0.01f;
    [SerializeField, Tooltip("Tốc độ giảm tiến độ khi lightning ngoài vùng hook")] private float hookProgressDegradationPower = 0.05f;
    [SerializeField, Tooltip("SpriteRenderer của hook để điều chỉnh kích thước")] private SpriteRenderer hookSpriteRenderer;
    [SerializeField, Tooltip("Container của thanh tiến độ")] private Transform progressBarContainer;

    [Header("Game Settings")]
    [SerializeField, Tooltip("Thời gian tối đa trước khi thua")] private float failTimer = 15f;
    private float currentFailTimer; // Bộ đếm thời gian hiện tại
    private bool isPaused; // Trạng thái tạm dừng trò chơi
    private bool hasWon; // Trạng thái chiến thắng

    [Header("UI")]
    [SerializeField, Tooltip("TextMeshPro để hiển thị trạng thái trò chơi")] private TextMeshProUGUI statusText;
    [SerializeField, Tooltip("TextMeshPro để hiển thị thời gian còn lại")] private TextMeshProUGUI timerText;

    private Vector3 topPivotPosition; // Lưu vị trí pivot để tối ưu
    private Vector3 bottomPivotPosition;

    public event Action OnGameEnded; // Sự kiện khi mini-game kết thúc

    private void Start()
    {
        if (!ValidateReferences()) return;
        InitializeGame();
    }

    /// <summary>
    /// Kiểm tra các tham chiếu cần thiết
    /// </summary>
    private bool ValidateReferences()
    {
        if (topPivot == null || bottomPivot == null || lightning == null ||
            hook == null || hookSpriteRenderer == null || progressBarContainer == null ||
            statusText == null || timerText == null)
        {
            Debug.LogError("Thiếu một hoặc nhiều tham chiếu trong Inspector! Vui lòng kiểm tra: topPivot, bottomPivot, lightning, hook, hookSpriteRenderer, progressBarContainer, statusText, timerText.");
            isPaused = true;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Khởi tạo trạng thái ban đầu của trò chơi
    /// </summary>
    private void InitializeGame()
    {
        Resize();
        topPivotPosition = topPivot.position;
        bottomPivotPosition = bottomPivot.position;
        hookPosition = 0.5f; // Bắt đầu hook ở giữa
        lightningPosition = 0.5f; // Bắt đầu lightning ở giữa
        lightningSpeed = 0f; // Vận tốc ban đầu
        noiseOffset = UnityEngine.Random.value * 100f; // Offset ngẫu nhiên cho Perlin noise
        hookProgress = 0f;
        currentFailTimer = failTimer;
        isPaused = false;
        hasWon = false;
        UpdateStatusText("");
        UpdateTimerText();
    }

    /// <summary>
    /// Điều chỉnh kích thước hook dựa trên khoảng cách giữa hai pivot
    /// </summary>
    private void Resize()
    {
        Bounds bounds = hookSpriteRenderer.bounds;
        float ySize = bounds.size.y;
        Vector3 localScale = hook.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        localScale.y = distance / ySize * hookSize;
        hook.localScale = localScale;
    }

    private void Update()
    {
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                QuitMiniGame();
            }
            return;
        }

        float deltaTime = Time.deltaTime;
        UpdateLightning(deltaTime);
        UpdateHook(deltaTime);
        UpdateProgress(deltaTime);
        UpdateTimerText();
    }

    /// <summary>
    /// Cập nhật chuyển động ngẫu nhiên của lightning sử dụng Perlin noise và vận tốc
    /// </summary>
    private void UpdateLightning(float deltaTime)
    {
        lightningTimer -= deltaTime;
        if (lightningTimer < 0)
        {
            lightningTimer = UnityEngine.Random.value * timerMultiplicator;
            lightningSpeed = UnityEngine.Random.Range(-smoothMotion, smoothMotion);
        }

        float noise = Mathf.PerlinNoise(Time.time * 0.5f + noiseOffset, 0f) * 2f - 1f; // Giá trị từ -1 đến 1
        lightningSpeed += noise * smoothMotion * 0.5f * deltaTime; // Điều chỉnh vận tốc bằng noise
        lightningSpeed = Mathf.Clamp(lightningSpeed, -smoothMotion, smoothMotion); // Giới hạn vận tốc

        lightningPosition += lightningSpeed * deltaTime;

        if (lightningPosition < 0f)
        {
            lightningPosition = 0f;
            lightningSpeed = Mathf.Abs(lightningSpeed); // Đảo hướng khi chạm đáy
        }
        else if (lightningPosition > 1f)
        {
            lightningPosition = 1f;
            lightningSpeed = -Mathf.Abs(lightningSpeed); // Đảo hướng khi chạm đỉnh
        }

        lightning.position = Vector3.Lerp(bottomPivotPosition, topPivotPosition, lightningPosition);
    }

    /// <summary>
    /// Cập nhật chuyển động và vận tốc của hook
    /// </summary>
    private void UpdateHook(float deltaTime)
    {
        if (Input.GetMouseButton(0))
        {
            hookPullVelocity += hookPullPower * deltaTime;
        }
        hookPullVelocity -= hookGravityPower * deltaTime;

        hookPosition += hookPullVelocity;

        if (hookPosition - hookSize / 2 <= 0f && hookPullVelocity < 0f)
        {
            hookPullVelocity = 0f;
        }
        if (hookPosition + hookSize / 2 >= 1f && hookPullVelocity > 0f)
        {
            hookPullVelocity = 0f;
        }

        hookPosition = Mathf.Clamp(hookPosition, hookSize / 2, 1 - hookSize / 2);
        hook.position = Vector3.Lerp(bottomPivotPosition, topPivotPosition, hookPosition);
    }

    /// <summary>
    /// Cập nhật tiến độ và kiểm tra điều kiện thắng/thua
    /// </summary>
    private void UpdateProgress(float deltaTime)
    {
        Vector3 localScale = progressBarContainer.localScale;
        localScale.y = hookProgress;
        progressBarContainer.localScale = localScale;

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if (min < lightningPosition && lightningPosition < max)
        {
            hookProgress += hookProgressIncreasePower * deltaTime;
        }
        else
        {
            hookProgress -= hookProgressDegradationPower * deltaTime;
            currentFailTimer -= deltaTime;
            if (currentFailTimer < 0)
            {
                Lose();
            }
        }

        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
        if (hookProgress >= 1f)
        {
            Win();
        }
    }

    /// <summary>
    /// Xử lý khi người chơi thua
    /// </summary>
    private void Lose()
    {
        isPaused = true;
        UpdateStatusText("Bạn đã thua! Nhấn 'E' để thoát.");
        OnGameEnded?.Invoke();
    }

    /// <summary>
    /// Xử lý khi người chơi thắng
    /// </summary>
    private void Win()
    {
        isPaused = true;
        hasWon = true;
        UpdateStatusText("Bạn đã thắng! Nhấn 'E' để thoát.");
        OnGameEnded?.Invoke();
    }

    /// <summary>
    /// Thoát mini-game và tắt container
    /// </summary>
    private void QuitMiniGame()
    {
        Debug.Log("Thoát mini-game!");
        OnGameEnded?.Invoke();
        gameObject.SetActive(false); // Tắt miniGameContainer
    }

    /// <summary>
    /// Đặt lại trạng thái mini-game
    /// </summary>
    public void ResetGame()
    {
        InitializeGame();
    }

    /// <summary>
    /// Kiểm tra trạng thái chiến thắng
    /// </summary>
    public bool IsWon()
    {
        return hasWon;
    }

    /// <summary>
    /// Cập nhật văn bản trạng thái trên UI
    /// </summary>
    private void UpdateStatusText(string message)
    {
        statusText.text = message;
    }

    /// <summary>
    /// Cập nhật văn bản thời gian trên UI
    /// </summary>
    private void UpdateTimerText()
    {
        timerText.text = $"Thời gian: {Mathf.CeilToInt(currentFailTimer)}s";
    }
}