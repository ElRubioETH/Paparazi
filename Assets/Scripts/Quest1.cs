using UnityEngine;
using TMPro;

public class Quest1 : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI textUI;
    public GameObject questpan;

    [Header("Text Settings")]
    [TextArea] public string allCollectedText = "Sửa xe";
    [TextArea] public string questHeader = "Nhặt đồ sửa xe:\n";

    [Header("Tools (gán theo thứ tự: Búa, Máy Khoan, Hộp Ốc Vít, Máy Nâng)")]
    public GameObject[] tools;

    public void Active()
    {
        questpan.SetActive(true);
        if (textUI == null) return;

        if (IsAllCollected())
        {
            textUI.text = allCollectedText;
        }
        else
        {
            string status = questHeader;
            foreach (GameObject tool in tools)
            {
                if (tool != null)
                {
                    status += $" {tool.name} {BoolToText(tool)}\n";
                }
            }
            textUI.text = status;
        }
    }

    bool IsAllCollected()
    {
        foreach (GameObject tool in tools)
        {
            if (tool == null || !tool.activeInHierarchy)
                return false;
        }
        return true;
    }

    string BoolToText(GameObject obj)
    {
        return (obj != null && obj.activeInHierarchy) ? "1/1" : "0/1";
    }
}
