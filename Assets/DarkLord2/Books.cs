using UnityEngine;
using TMPro;

public class Book : MonoBehaviour
{
    public string bookContent = "N?i dung m?c ??nh c?a sách."; // N?i dung riêng cho t?ng sách
    public TextMeshProUGUI canvasText; // Tham chi?u ??n TextMeshProUGUI trên Canvas

    public void DisplayContent()
    {
        if (canvasText != null)
        {
            canvasText.text = bookContent;
        }
    }
}