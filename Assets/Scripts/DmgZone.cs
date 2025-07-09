using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damageAmount = 20f;
    private bool canDamage = false;

    void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Enemy"))
        {
            FieldOfView enemy = other.GetComponent<FieldOfView>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                canDamage = false;
            }
        }
    }

    public void EnableDamage()
    {
        canDamage = true;
    }
}
