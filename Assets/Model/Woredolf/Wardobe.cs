using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    public bool safe = false;
    public Doors doorScript;

    private bool playerInside = false;

    private void Start()
    {
        safe = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player vô tủ");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            safe = false;
            Debug.Log("Player ra khỏi tủ => Safe: false");
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            if (doorScript != null && !doorScript.IsOpen)
            {
                if (!safe)
                {
                    safe = true;
                    Debug.Log("SAFE bật: Player trong tủ + cửa đã đóng");
                }
            }
            else
            {
                if (safe)
                {
                    safe = false;
                    Debug.Log("SAFE tắt: Cửa mở toang hoặc cửa null");
                }
            }
        }
    }
}
