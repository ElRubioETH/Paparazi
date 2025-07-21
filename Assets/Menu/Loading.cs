using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider loadingSlider;
    public GameObject loadingPanel; // Panel để ẩn hiện loading UI
    public PlayableDirector timeline; // Timeline bạn muốn chạy

    void Start()
    {
        StartCoroutine(LoadSceneAndPlayTimeline());
    }

    IEnumerator LoadSceneAndPlayTimeline()
    {
        loadingPanel.SetActive(true);
        float progress = 0f;

        // Giả lập quá trình loading
        while (progress < 1f)
        {
            progress += Time.deltaTime * 0.5f; // tốc độ loading
            loadingSlider.value = progress;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); // delay nhỏ cho đẹp

        loadingPanel.SetActive(false);

        if (timeline != null)
        {
            timeline.Play();
        }
    }
}
