using UnityEngine;

public class ShaderGraphSliderLerp : MonoBehaviour
{
    public Renderer targetRenderer;       // Renderer của object
    public string propertyName = "_MyFloat"; // Tên property trong Shader Graph
    public float duration = 5f;           // Thời gian từ 0 -> 1

    private float elapsedTime = 0f;
    private bool isRunning = false;

    void OnEnable()
    {
        // Reset khi object vừa được bật
        elapsedTime = 0f;
        isRunning = true;

        // Đảm bảo slider bắt đầu từ 0
        if (targetRenderer != null)
            targetRenderer.material.SetFloat(propertyName, 0f);
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        if (targetRenderer != null)
            targetRenderer.material.SetFloat(propertyName, t);

        if (t >= 1f)
            isRunning = false; // Dừng khi đạt 1
    }
}
