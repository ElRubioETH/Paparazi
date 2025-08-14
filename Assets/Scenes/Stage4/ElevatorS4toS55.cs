using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseAndLoad : MonoBehaviour
{
    public Animator animator;            // Animator của object
    public string closeTrigger = "Close";
    public GameObject panel;              // Panel cần hiển thị
    public float delayBeforePanel = 2f;   // Thời gian chờ sau khi Close
    public float delayBeforeLoad = 1f;    // Thời gian chờ sau khi hiện Panel

    public GateLeverInteraction lever;    // Tham chiếu tới script Lever

    private bool hasTriggered = false;
    private void Start()
    {
        panel.SetActive(false);

    }
    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // Đảm bảo chỉ chạy 1 lần
        if (!lever || !lever.Electricfy) return; // Chỉ chạy khi Lever Electricfy = true

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Gọi animation Close
            if (animator != null)
            {
                animator.SetTrigger(closeTrigger);
            }

            // Sau 2 giây thì bật panel
            Invoke(nameof(ShowPanel), delayBeforePanel);
        }
    }

    void ShowPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        // Sau 1 giây load scene Stage5
        Invoke(nameof(LoadStage5), delayBeforeLoad);
    }

    void LoadStage5()
    {
        SceneManager.LoadScene("Stage5");
    }
}
