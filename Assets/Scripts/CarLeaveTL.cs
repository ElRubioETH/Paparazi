using UnityEngine;
using UnityEngine.Playables;

public class CarLeaving : MonoBehaviour
{
    [Header("References")]
    public Doors doorScript;
    public PlayableDirector timeline;

    private bool hasOpenedOnce = false;
    private bool hasPlayedTimeline = false;

    void Update()
    {
        if (doorScript == null) return;

        // Check khi cửa mở hoàn tất (chỉ cần set flag 1 lần)
        if (!hasOpenedOnce && doorScript.IsOpen)
        {
            hasOpenedOnce = true;
            Debug.Log("🚪 Đã mở cửa lần đầu. Giờ có thể bấm lần nữa để chạy Timeline.");
            return; // dừng ở đây để không chạy ngay lúc mở
        }

        // Chỉ cho tương tác timeline sau khi cửa mở
        if (hasOpenedOnce && !hasPlayedTimeline && doorScript.inReach && Input.GetButtonDown("Interact"))
        {
            ToggleTimeline();
        }
    }

    void ToggleTimeline()
    {
        if (timeline == null)
        {
            Debug.LogWarning("❌ Timeline chưa được gán!");
            return;
        }

        if (timeline.state == PlayState.Playing)
        {
            Debug.Log("⏳ Timeline đang chạy rồi.");
            return;
        }

        timeline.Play();
        hasPlayedTimeline = true;
        Debug.Log("🎬 Timeline CarLeaving được chạy!");
    }
}
