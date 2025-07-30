using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NewDoors : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField, Tooltip("Danh sách các hộp điện trong cảnh")] private ElectricBox[] electricBoxes;
    [SerializeField, Tooltip("Animator để điều khiển animation mở cửa")] private Animator doorAnimator;
    [SerializeField, Tooltip("AudioSource để phát âm thanh mở cửa")] private AudioSource openSound;
    [SerializeField, Tooltip("TextMeshPro để hiển thị số hộp đã sửa")] private TextMeshProUGUI statusText;

    private int boxesFixed; // Số hộp điện đã sửa
    private bool isOpen; // Trạng thái cửa (đã mở hay chưa)
    private HashSet<ElectricBox> fixedBoxes; // Theo dõi các hộp đã sửa

    private void Start()
    {
        boxesFixed = 0;
        isOpen = false;
        fixedBoxes = new HashSet<ElectricBox>();

        // Kiểm tra danh sách electricBoxes
        if (electricBoxes == null || electricBoxes.Length != 5)
        {
            Debug.LogError($"Danh sách electricBoxes phải có đúng 5 phần tử, hiện tại có: {(electricBoxes == null ? 0 : electricBoxes.Length)}!");
            return;
        }

        // Kiểm tra trùng lặp và null
        HashSet<ElectricBox> uniqueBoxes = new HashSet<ElectricBox>();
        for (int i = 0; i < electricBoxes.Length; i++)
        {
            var box = electricBoxes[i];
            if (box == null)
            {
                Debug.LogError($"ElectricBox tại vị trí {i} trong danh sách là null!");
                continue;
            }
            if (!uniqueBoxes.Add(box))
            {
                Debug.LogError($"ElectricBox {box.name} (InstanceID: {box.GetInstanceID()}) bị trùng lặp trong danh sách electricBoxes!");
            }
            else
            {
                box.RegisterDoor(this);
                Debug.Log($"Đã đăng ký ElectricBox: {box.name} (InstanceID: {box.GetInstanceID()})");
            }
        }

        UpdateStatusText();
    }

    /// <summary>
    /// Gọi khi một hộp điện được sửa thành công
    /// </summary>
    public void OnBoxFixed(ElectricBox box)
    {
        if (isOpen || fixedBoxes.Contains(box))
        {
            Debug.Log($"Hộp điện {box.name} (InstanceID: {box.GetInstanceID()}) đã được sửa trước đó hoặc cửa đã mở, bỏ qua!");
            return;
        }

        fixedBoxes.Add(box);
        boxesFixed++;
        Debug.Log($"Hộp điện {box.name} (InstanceID: {box.GetInstanceID()}) được sửa, tổng số: {boxesFixed}/5, fixedBoxes count: {fixedBoxes.Count}");

        UpdateStatusText();

        if (boxesFixed >= 5)
        {
            OpenDoor();
        }
    }

    /// <summary>
    /// Mở cửa bằng Animator và phát âm thanh
    /// </summary>
    private void OpenDoor()
    {
        if (isOpen)
        {
            Debug.Log("Cửa đã mở, bỏ qua!");
            return;
        }

        isOpen = true;
        if (openSound != null)
        {
            openSound.Play();
            Debug.Log("Phát âm thanh mở cửa!");
        }
        else
        {
            Debug.LogWarning("AudioSource cho âm thanh mở cửa chưa được gán!");
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            Debug.Log("Đã kích hoạt trigger 'Open' cho cửa!");
        }
        else
        {
            Debug.LogWarning("Animator cho cửa chưa được gán!");
        }

        UpdateStatusText();
    }

    /// <summary>
    /// Cập nhật văn bản trạng thái
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            if (isOpen)
            {
                statusText.text = "Cửa đã mở!";
            }
            else
            {
                statusText.text = $"Hộp điện đã sửa: {boxesFixed}/5";
            }
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI cho statusText chưa được gán!");
        }
    }
}