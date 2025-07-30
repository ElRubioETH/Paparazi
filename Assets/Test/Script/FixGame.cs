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
    private float lightningPosition;
    private float lightningTimer;
    [SerializeField, Tooltip("Thời gian tối đa giữa các lần thay đổi vận tốc lightning")] private float timerMultiplicator = 1f;
    private float lightningSpeed;
    [SerializeField, Tooltip("Giới hạn vận tốc tối đa của lightning")] private float smoothMotion = 0.2f;
    private float noiseOffset;

    [Header("Hook")]
    [SerializeField, Tooltip("GameObject của hook")] private Transform hook;
    private float hookPosition;
    [SerializeField, Tooltip("Kích thước vùng hoạt động của hook")] private float hookSize = 0.15f;
    [SerializeField, Tooltip("Tốc độ tăng tiến độ khi lightning trong vùng hook")] private float hookProgressIncreasePower = 0.4f;
    private float hookProgress;
    private float hookPullVelocity;
    [SerializeField, Tooltip("Lực kéo hook khi nhấn chuột")] private float hookPullPower = 0.02f;
    [SerializeField, Tooltip("Lực trọng lực kéo hook xuống")] private float hookGravityPower = 0.01f;
    [SerializeField, Tooltip("Tốc độ giảm tiến độ khi lightning ngoài vùng hook")] private float hookProgressDegradationPower = 0.05f;
    [SerializeField, Tooltip("SpriteRenderer của hook để điều chỉnh kích thước")] private SpriteRenderer hookSpriteRenderer;
    [SerializeField, Tooltip("Container của thanh tiến độ")] private Transform progressBarContainer;

    [Header("Game Settings")]
    [SerializeField, Tooltip("Thời gian tối đa trước khi thua")] private float failTimer = 15f;
    private float currentFailTimer;
    private bool isPaused;
    private bool hasWon;

    [Header("UI")]
    [SerializeField, Tooltip("TextMeshPro để hiển thị trạng thái trò chơi")] private TextMeshProUGUI statusText;
    [SerializeField, Tooltip("TextMeshPro để hiển thị thời gian còn lại")] private TextMeshProUGUI timerText;

    private Vector3 topPivotPosition;
    private Vector3 bottomPivotPosition;

    public event Action OnGameEnded;

    private void Awake()
    {
        hasWon = false;
        isPaused = false;
        Debug.Log($"Awake FixGame trên {gameObject.name} (InstanceID: {GetInstanceID()})");
    }

    private void Start()
    {
        if (!ValidateReferences()) return;
        InitializeGame();
        Debug.Log($"Khởi tạo FixGame trên {gameObject.name} (InstanceID: {GetInstanceID()})");
    }

    private bool ValidateReferences()
    {
        if (topPivot == null || bottomPivot == null || lightning == null ||
            hook == null || hookSpriteRenderer == null || progressBarContainer == null ||
            statusText == null || timerText == null)
        {
            Debug.LogError($"Thiếu một hoặc nhiều tham chiếu trong Inspector cho FixGame trên {gameObject.name} (InstanceID: {GetInstanceID()})! Vui lòng kiểm tra: topPivot, bottomPivot, lightning, hook, hookSpriteRenderer, progressBarContainer, statusText, timerText.");
            isPaused = true;
            return false;
        }
        return true;
    }

    private void InitializeGame()
    {
        Resize();
        topPivotPosition = topPivot.position;
        bottomPivotPosition = bottomPivot.position;
        hookPosition = 0.5f;
        lightningPosition = 0.5f;
        lightningSpeed = 0f;
        noiseOffset = UnityEngine.Random.value * 100f;
        hookProgress = 0f;
        currentFailTimer = failTimer;
        isPaused = false;
        hasWon = false;
        UpdateStatusText("");
        UpdateTimerText();
    }

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

    private void UpdateLightning(float deltaTime)
    {
        lightningTimer -= deltaTime;
        if (lightningTimer < 0)
        {
            lightningTimer = UnityEngine.Random.value * timerMultiplicator;
            lightningSpeed = UnityEngine.Random.Range(-smoothMotion, smoothMotion);
        }

        float noise = Mathf.PerlinNoise(Time.time * 0.5f + noiseOffset, 0f) * 2f - 1f;
        lightningSpeed += noise * smoothMotion * 0.5f * deltaTime;
        lightningSpeed = Mathf.Clamp(lightningSpeed, -smoothMotion, smoothMotion);

        lightningPosition += lightningSpeed * deltaTime;

        if (lightningPosition < 0f)
        {
            lightningPosition = 0f;
            lightningSpeed = Mathf.Abs(lightningSpeed);
        }
        else if (lightningPosition > 1f)
        {
            lightningPosition = 1f;
            lightningSpeed = -Mathf.Abs(lightningSpeed);
        }

        lightning.position = Vector3.Lerp(bottomPivotPosition, topPivotPosition, lightningPosition);
    }

    private void UpdateHook(float deltaTime)
    {
        if (Input.GetKey(KeyCode.Mouse0))
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

    private void Lose()
    {
        isPaused = true;
        UpdateStatusText("Bạn đã thua! Nhấn 'E' để thoát.");
        Debug.Log($"Mini-game trên {gameObject.name} (InstanceID: {GetInstanceID()}) thua, kích hoạt OnGameEnded");
        OnGameEnded?.Invoke();
    }

    private void Win()
    {
        if (hasWon) return; // Ngăn gọi nhiều lần
        isPaused = true;
        hasWon = true;
        UpdateStatusText("Bạn đã thắng! Nhấn 'E' để thoát.");
        Debug.Log($"Mini-game trên {gameObject.name} (InstanceID: {GetInstanceID()}) thắng, kích hoạt OnGameEnded");
        OnGameEnded?.Invoke();
    }

    private void QuitMiniGame()
    {
        Debug.Log($"Thoát mini-game trên {gameObject.name} (InstanceID: {GetInstanceID()})!");
        isPaused = true;
        OnGameEnded?.Invoke();
        gameObject.SetActive(false);
    }

    public void ResetGame()
    {
        InitializeGame();
        Debug.Log($"Đặt lại mini-game trên {gameObject.name} (InstanceID: {GetInstanceID()})");
    }

    public bool IsWon()
    {
        return hasWon;
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = $"Thời gian: {Mathf.CeilToInt(currentFailTimer)}s";
        }
    }
}