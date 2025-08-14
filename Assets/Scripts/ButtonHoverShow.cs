using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage targetImage; // RawImage bạn muốn hiển thị khi hover

    void Start()
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(false); // Ẩn lúc đầu
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true); // Hiện khi hover
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