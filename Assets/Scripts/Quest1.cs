using UnityEngine;
using TMPro;  // Nếu bạn dùng TextMeshPro (khuyên dùng)

public class Quest1 : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI textUI;

    [Header("Tools")]
    public GameObject hammer;
    public GameObject drill;
    public GameObject boltBox;
    public GameObject lifter;

    void Update()
    {
        if (textUI == null) return;

        string status = "Nhặt đồ sửa xe:\n";

        status += " Búa " + BoolToText(hammer) + "\n";
        status += " Máy Khoan " + BoolToText(drill) + "\n";
        status += " Hộp Ốc vít " + BoolToText(boltBox) + "\n";
        status += " Máy Nâng " + BoolToText(lifter) + "\n";

        textUI.text = status;
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
