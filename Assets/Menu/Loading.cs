using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro; // Dùng nếu bạn dùng TextMeshPro
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider loadingSlider;
    public GameObject loadingPanel;
    public PlayableDirector timeline;
    public TextMeshProUGUI loadingText; // Nếu bạn dùng Text thường thì dùng: public Text loadingText;

    void Start()
    {
        StartCoroutine(LoadSceneAndPlayTimeline());
    }

    IEnumerator LoadSceneAndPlayTimeline()
    {
        loadingPanel.SetActive(true);
        float progress = 0f;

        // Giai đoạn đầu: tăng đến 60% rồi tạm dừng
        while (progress < 0.6f)
        {
            progress += Time.deltaTime * 0.5f;
            UpdateUI(progress);
            yield return null;
        }

        // Khựng lại 1.2 giây tại 60%
        yield return new WaitForSeconds(1.2f);

        // Giai đoạn sau: tăng từ 60% đến 100% với tốc độ khác nhau
        while (progress < 1f)
        {
            // Chạy nhanh lúc đầu, chậm dần về cuối
            float speed = Mathf.Lerp(0.4f, 0.1f, (progress - 0.6f) / 0.4f);
            progress += Time.deltaTime * speed;
            UpdateUI(progress);
            yield return null;
        }

        // Đảm bảo chắc chắn progress = 1
        progress = 1f;
        UpdateUI(progress);

        yield return new WaitForSeconds(0.3f);
        loadingPanel.SetActive(false);

        if (timeline != null)
        {
            timeline.Play();
        }
    }

    void UpdateUI(float progress)
    {
        loadingSlider.value = progress;
        if (loadingText != null)
        {
            loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";
        }
    }
}
