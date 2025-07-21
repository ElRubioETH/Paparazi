using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;

    [Header("Loading Text Settings")]
    public float dotInterval = 0.5f;

    private string baseText = "Loading";
    private int dotCount = 0;
    private float timer = 0f;
    private bool increasing = true;
    private bool isLoading = false;

    private void Start()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isLoading) return;

        timer += Time.deltaTime;
        if (timer >= dotInterval)
        {
            timer = 0f;

            // Tăng rồi giảm số chấm: 1 → 2 → 3 → 0 → 1...
            if (increasing)
            {
                dotCount++;
                if (dotCount >= 3)
                    increasing = false;
            }
            else
            {
                dotCount--;
                if (dotCount <= 0)
                    increasing = true;
            }

            if (loadingText != null)
                loadingText.text = baseText + new string('.', dotCount);
        }
    }

    public void Starts()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        isLoading = true;

        // Load scene ngay
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        // Tuỳ chọn chưa dùng
    }

    public void Quit()
    {
        Application.Quit();
    }
}
