using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel; // panel "Game Over" + nút Play Again

    [Header("Options")]
    public bool pauseOnGameOver = true; // dừng thời gian khi chết
    public KeyCode quickRetryKey = KeyCode.R; // bấm R để chơi lại
    public float autoRetryAfter = -1f; // set >0 để auto reload sau X giây

    private bool showing;

    void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void Show()
    {
        if (showing) return;
        showing = true;

        if (gameOverPanel) gameOverPanel.SetActive(true);

        if (pauseOnGameOver)
            Time.timeScale = 0f; // freeze game cho đỡ “quạu”

        if (autoRetryAfter > 0f)
            StartCoroutine(AutoRetryCoroutine());
    }

    System.Collections.IEnumerator AutoRetryCoroutine()
    {
        // vì đã pause Time.timeScale, dùng realtime
        float t = autoRetryAfter;
        while (t > 0f)
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        }
        Retry();
    }

    void Update()
    {
        if (!showing) return;

        if (Input.GetKeyDown(quickRetryKey))
        {
            Retry();
        }
    }

    // Gắn hàm này cho nút "Play Again" trong Button.onClick
    public void Retry()
    {
        // nhớ bỏ pause trước khi load
        Time.timeScale = 1f;
        string current = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(current);
    }

    // Tuỳ thích: nút "Quit to Menu"
    public void QuitToMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
