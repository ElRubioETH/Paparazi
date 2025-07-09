using UnityEngine;
using System.Collections;

public class EnemyDamageZone : MonoBehaviour
{
    public float damageAmount = 10f;
    private bool playerInside = false;
    private bool isAttacking = false;

    private FieldOfView enemyFOV;

    void Start()
    {
        enemyFOV = GetComponentInParent<FieldOfView>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            playerInside = true;
            StartCoroutine(AttackLoop(other));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    IEnumerator AttackLoop(Collider player)
    {
        isAttacking = true;

        while (playerInside)
        {
            // Trigger animation
            if (enemyFOV != null)
                enemyFOV.Attack();

            // Wait 1s then deal damage
            yield return new WaitForSeconds(1f);

            PlayerHealth ph = player.GetComponent<PlayerHealth>(); // thay bằng script player onii-chan dùng
            if (ph != null)
            {
                ph.TakeDamage(damageAmount);
            }

            // Đợi 3s trước lần đánh tiếp theo
            yield return new WaitForSeconds(3f);
        }

        isAttacking = false;
    }
}
