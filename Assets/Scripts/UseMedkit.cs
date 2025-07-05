using UnityEngine;

public class UseMedkit : MonoBehaviour
{
    public GameObject HaveMedkit;          // GameObject của medkit (nếu muốn ẩn sau khi dùng)
    public AudioSource use;            // Âm thanh dùng medkit
    public PlayerHealth playerHealth;  // Tham chiếu tới PlayerHealth để tăng máu
    public GameObject medkit;
    public float healAmount = 25f;     // Lượng máu hồi

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && HaveMedkit.activeInHierarchy)
        {
            Use();
        }
    }

    public void Use()
    {
        if (playerHealth != null && playerHealth.health < playerHealth.maxHealth)
        {
            playerHealth.health += healAmount;

            // Clamp giới hạn máu không vượt quá max
            if (playerHealth.health > playerHealth.maxHealth)
                playerHealth.health = playerHealth.maxHealth;

            // Cập nhật thanh máu
            if (playerHealth.healthSlider != null)
                playerHealth.healthSlider.value = playerHealth.health;

            // Phát âm thanh
            if (use != null)
                use.Play();

            // Ẩn hoặc phá huỷ medkit (tùy theo thiết kế)
            if (HaveMedkit != null)
                HaveMedkit.SetActive(false);
            medkit.SetActive(false);
        }
    }
}
