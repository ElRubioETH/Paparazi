using UnityEngine;
using UnityEngine.AI;

public class EnemyBossAI : MonoBehaviour
{
    // Tham chi?u ??n NavMeshAgent và Animator
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource; // Tham chi?u ??n AudioSource

    // Tham chi?u ??n ng??i ch?i
    public Transform player;

    // ?i?m tu?n tra (kéo th? GameObject)
    public Transform patrolPointA; // ?i?m A tu?n tra
    public Transform patrolPointB; // ?i?m B tu?n tra
    private Transform currentPatrolTarget;
    public float patrolSpeed = 2f;

    // Ph?m vi phát hi?n và t?n công
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 25f; // Kho?ng cách t?i ?a ?u?i theo

    // Tr?ng thái hi?n t?i
    private enum State { Patrol, Chase, Attack }
    private State currentState;

    // Th?i gian gi?a các l?n t?n công
    public float attackCooldown = 2f;
    private float lastAttackTime;

    // AudioClip cho các tr?ng thái
    public AudioClip patrolSound; // Âm thanh tu?n tra (b??c chân)
    public AudioClip chaseSound;  // Âm thanh ?u?i theo (ch?y)
    public AudioClip attackSound; // Âm thanh t?n công (có th? dùng chung ho?c riêng cho t?ng ki?u)

    // Qu?n lý ki?u t?n công
    private int currentAttackIndex = 0; // 0: Attack1, 1: Attack2, 2: Attack3

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentState = State.Patrol;
        currentPatrolTarget = patrolPointA; // B?t ??u tu?n tra t?i ?i?m A
        agent.speed = patrolSpeed;

        // C?u hình AudioSource
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false; // Không l?p t? ??ng, script s? qu?n lý
        }
    }

    void Update()
    {
        // Tính kho?ng cách ??n ng??i ch?i
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Chuy?n ??i tr?ng thái d?a trên kho?ng cách
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= detectionRange && distanceToPlayer <= maxChaseDistance)
        {
            currentState = State.Chase;
        }
        else if (distanceToPlayer > maxChaseDistance && currentState == State.Chase)
        {
            currentState = State.Patrol; // Quay v? tu?n tra n?u ra ngoài 25f
        }
        else
        {
            currentState = State.Patrol;
        }

        // Qu?n lý âm thanh d?a trên tr?ng thái và kho?ng cách
        ManageAudio(distanceToPlayer);

        // Th?c hi?n hành vi d?a trên tr?ng thái
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        // C?p nh?t t?c ?? và animation
        agent.speed = patrolSpeed;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking1", false);
        animator.SetBool("isAttacking2", false);
        animator.SetBool("isAttacking3", false);

        // Di chuy?n ??n ?i?m tu?n tra hi?n t?i
        agent.SetDestination(currentPatrolTarget.position);

        // N?u ??n g?n ?i?m tu?n tra, chuy?n sang ?i?m còn l?i
        if (Vector3.Distance(transform.position, currentPatrolTarget.position) < 1f)
        {
            currentPatrolTarget = currentPatrolTarget == patrolPointA ? patrolPointB : patrolPointA;
        }
    }

    void Chase()
    {
        // C?p nh?t t?c ?? và animation
        agent.speed = chaseSpeed;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking1", false);
        animator.SetBool("isAttacking2", false);
        animator.SetBool("isAttacking3", false);

        // ?u?i theo ng??i ch?i, nh?ng ch? trong maxChaseDistance
        if (Vector3.Distance(transform.position, player.position) <= maxChaseDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(currentPatrolTarget.position); // Quay v? ?i?m tu?n tra n?u quá 25f
        }
    }

    void Attack()
    {
        // D?ng di chuy?n khi t?n công
        agent.SetDestination(transform.position);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        // T?n công n?u ?? th?i gian cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetBool("isAttacking1", false);
            animator.SetBool("isAttacking2", false);
            animator.SetBool("isAttacking3", false);

            // Ch?n ki?u t?n công theo th? t?
            switch (currentAttackIndex)
            {
                case 0:
                    animator.SetBool("isAttacking1", true);
                    break;
                case 1:
                    animator.SetBool("isAttacking2", true);
                    break;
                case 2:
                    animator.SetBool("isAttacking3", true);
                    break;
            }

            lastAttackTime = Time.time;
            Debug.Log("Boss attacks with style " + (currentAttackIndex + 1) + "!");

            // Chuy?n sang ki?u t?n công ti?p theo
            currentAttackIndex = (currentAttackIndex + 1) % 3;
        }
    }

    void ManageAudio(float distanceToPlayer)
    {
        // Qu?n lý âm thanh tu?n tra
        if (patrolSound != null && currentState == State.Patrol && distanceToPlayer <= 20f && !audioSource.isPlaying)
        {
            audioSource.clip = patrolSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (distanceToPlayer > 20f && audioSource.isPlaying && audioSource.clip == patrolSound)
        {
            audioSource.Stop();
        }

        // Qu?n lý âm thanh ?u?i theo
        if (chaseSound != null && currentState == State.Chase && distanceToPlayer <= detectionRange && audioSource.clip != chaseSound)
        {
            audioSource.Stop();
            audioSource.clip = chaseSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if ((distanceToPlayer > detectionRange || currentState != State.Chase) && audioSource.isPlaying && audioSource.clip == chaseSound)
        {
            audioSource.Stop();
        }

        // Qu?n lý âm thanh t?n công
        if (attackSound != null && currentState == State.Attack && distanceToPlayer <= attackRange && !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = attackSound;
            audioSource.loop = false;
            audioSource.Play();
        }
        else if (distanceToPlayer > attackRange && audioSource.isPlaying && audioSource.clip == attackSound)
        {
            audioSource.Stop();
        }
    }

    // Hi?n th? ph?m vi phát hi?n và t?n công trong Scene view
    void OnDrawGizmos()
    {
        // Ph?m vi phát hi?n (màu vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Ph?m vi t?n công (màu ??)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ?i?m tu?n tra A và B (màu xanh)
        Gizmos.color = Color.blue;
        if (patrolPointA != null) Gizmos.DrawWireCube(patrolPointA.position, Vector3.one);
        if (patrolPointB != null) Gizmos.DrawWireCube(patrolPointB.position, Vector3.one);

        // Hi?n th? ph?m vi 20f cho âm thanh tu?n tra (màu xanh nh?t)
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 20f);

        // Hi?n th? ph?m vi t?i ?a ?u?i theo 25f (màu tím nh?t)
        Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }
}