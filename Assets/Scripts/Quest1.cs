using UnityEngine;
using TMPro;  // Nếu bạn dùng TextMeshPro (khuyên dùng)

public class Quest1 : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI textUI;
    public GameObject questpan;
    [Header("Tools")]
    public GameObject hammer;
    public GameObject drill;
    public GameObject boltBox;
    public GameObject lifter;

    public void Active()
    {
        questpan.SetActive(true);
        if (textUI == null) return;

        // Kiểm tra nếu tất cả đều đã nhặt
        if (IsAllCollected())
        {
            textUI.text = "Sửa xe";
        }
        else
        {
            string status = "Nhặt đồ sửa xe:\n";
            status += " Búa " + BoolToText(hammer) + "\n";
            status += " Máy Khoan " + BoolToText(drill) + "\n";
            status += " Hộp Ốc vít " + BoolToText(boltBox) + "\n";
            status += " Máy Nâng " + BoolToText(lifter) + "\n";
            textUI.text = status;
        }
    }

    bool IsAllCollected()
    {
        return hammer != null && hammer.activeInHierarchy &&
               drill != null && drill.activeInHierarchy &&
               boltBox != null && boltBox.activeInHierarchy &&
               lifter != null && lifter.activeInHierarchy;
    }


    string BoolToText(GameObject obj)
    {
        if (obj != null && obj.activeInHierarchy)
        {
            return "1/1";
        }
        return "0/1";
    }
}
