using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections;            // <- Để dùng IEnumerator và Coroutine

public class KatanaSwing : MonoBehaviour
{
    public GameObject katana;
    public Animator anim;
    private bool isAttacking = false;
    private float attackDuration;
    public bool haveKatana;
    public DamageZone DmgZone;
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float attackDuration = stateInfo.length;

        // Gọi zone gây sát thương
        DmgZone.EnableDamage();

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }
}
