using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage targetImage; // RawImage bạn muốn hiển thị khi hover

    [Header("Âm thanh Hover")]
    public AudioSource audioSource;  // AudioSource để phát âm thanh
    public AudioClip hoverSound;     // File âm thanh khi hover

    void Start()
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(false); // Ẩn lúc đầu
        }

        // Nếu chưa gắn AudioSource thì auto tạo
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true); // Hiện khi hover
        }

        // Phát âm thanh khi hover
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(false); // Ẩn khi rời chuột
        }
    }
}
