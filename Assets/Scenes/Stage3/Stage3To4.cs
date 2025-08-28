using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadStage4OnTouch : MonoBehaviour
{
    public GameObject panel;      // Panel hiển thị trước khi chuyển cảnh
    public float delay = 2f;      // Thời gian chờ (giây)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (panel != null)
                panel.SetActive(true);   // bật panel lên

            StartCoroutine(LoadSceneWithDelay());
        }
    }

    private System.Collections.IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Stage4");
    }
}
