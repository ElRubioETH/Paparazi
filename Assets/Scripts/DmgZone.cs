using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damageAmount = 20f;
    private bool canDamage = false;

    void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                canDamage = false; // Đảm bảo mỗi swing chỉ đánh trúng 1 lần
            }
        }
    }

    public void EnableDamage()
    {
        canDamage = true;
    }
}
