using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    public Transform player;
    public Transform patrolPointA;
    public Transform patrolPointB;
    private Transform currentPatrolTarget;
    public float patrolSpeed = 2f;

    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 25f;
    public float fieldOfViewAngle = 60f;

    private enum State { Patrol, Chase, Attack, PullFromHiding }
    private State currentState;

    public float attackCooldown = 2f;
    private float lastAttackTime;

    public AudioClip patrolSound;
    public AudioClip chaseSound;
    public AudioClip attackSound;
    public AudioClip pullSound;

    public LayerMask obstacleLayer;
    public LayerMask hidingLayer;
    private Transform hidingObject;
    public float pullRange = 2f;
    public float pullDuration = 2f;
    private float pullStartTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentState = State.Patrol;
        currentPatrolTarget = patrolPointA;
        agent.speed = patrolSpeed;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = IsPlayerInFOVAndVisible();
        bool isPlayerHiding = CheckPlayerHiding();

        if (isPlayerHiding && canSeePlayer && distanceToPlayer <= pullRange)
        {
            currentState = State.PullFromHiding;
        }
        else if (distanceToPlayer <= attackRange && canSeePlayer && !isPlayerHiding)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= detectionRange && distanceToPlayer <= maxChaseDistance && canSeePlayer && !isPlayerHiding)
        {
            currentState = State.Chase;
        }
        else if (distanceToPlayer > maxChaseDistance && currentState == State.Chase)
        {
            currentState = State.Patrol;
        }
        else if (!canSeePlayer || isPlayerHiding)
        {
            currentState = State.Patrol;
        }
        else
        {
            currentState = State.Patrol;
        }

        ManageAudio(distanceToPlayer);

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
            case State.PullFromHiding:
                PullFromHiding();
                break;
        }
    }

    bool IsPlayerInFOVAndVisible()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToPlayer) <= fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer, out hit, detectionRange, obstacleLayer))
            {
                if (hit.transform == player)
                    return true;
                return false;
            }
            return true;
        }
        return false;
    }

    bool CheckPlayerHiding()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, 1.0f, hidingLayer);
        if (hits.Length > 0 && Vector3.Distance(player.position, hits[0].transform.position) < 0.5f)
        {
            hidingObject = hits[0].transform;
            return true;
        }
        hidingObject = null;
        return false;
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        agent.SetDestination(currentPatrolTarget.position);
        if (Vector3.Distance(transform.position, currentPatrolTarget.position) < 1f)
            currentPatrolTarget = currentPatrolTarget == patrolPointA ? patrolPointB : patrolPointA;
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);
        if (Vector3.Distance(transform.position, player.position) <= maxChaseDistance)
            agent.SetDestination(player.position);
        else
            agent.SetDestination(currentPatrolTarget.position);
    }

    void Attack()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
        }
    }

    void PullFromHiding()
    {
        if (hidingObject != null && Vector3.Distance(transform.position, hidingObject.position) > pullRange)
        {
            agent.SetDestination(hidingObject.position);
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            pullStartTime = 0f;
        }
        else
        {
            agent.SetDestination(transform.position);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);

            if (pullStartTime == 0f)
                pullStartTime = Time.time;

            if (pullSound != null && !audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = pullSound;
                audioSource.loop = false;
                audioSource.Play();
            }

            if (Time.time - pullStartTime >= pullDuration)
            {
                if (hidingObject != null)
                {
                    Vector3 pullOutPosition = hidingObject.position + (transform.position - hidingObject.position).normalized * 2f;
                    player.position = pullOutPosition;
                    pullStartTime = 0f;
                    hidingObject = null;
                    currentState = State.Chase;
                }
            }
        }
    }

    void ManageAudio(float distanceToPlayer)
    {
        if (patrolSound != null && currentState == State.Patrol && distanceToPlayer <= 20f && !audioSource.isPlaying)
        {
            audioSource.clip = patrolSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (distanceToPlayer > 20f && audioSource.isPlaying && audioSource.clip == patrolSound)
            audioSource.Stop();

        if (chaseSound != null && currentState == State.Chase && distanceToPlayer <= detectionRange && audioSource.clip != chaseSound)
        {
            audioSource.Stop();
            audioSource.clip = chaseSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if ((distanceToPlayer > detectionRange || currentState != State.Chase) && audioSource.isPlaying && audioSource.clip == chaseSound)
            audioSource.Stop();

        if (attackSound != null && currentState == State.Attack && distanceToPlayer <= attackRange && !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = attackSound;
            audioSource.loop = false;
            audioSource.Play();
        }
        else if (distanceToPlayer > attackRange && audioSource.isPlaying && audioSource.clip == attackSound)
            audioSource.Stop();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pullRange);
        Gizmos.color = Color.blue;
        if (patrolPointA != null) Gizmos.DrawWireCube(patrolPointA.position, Vector3.one);
        if (patrolPointB != null) Gizmos.DrawWireCube(patrolPointB.position, Vector3.one);
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 20f);
        Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Vector3 forward = transform.forward;
        Vector3 leftRay = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * forward;
        Vector3 rightRay = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * forward;
        Gizmos.DrawRay(transform.position, leftRay * detectionRange);
        Gizmos.DrawRay(transform.position, rightRay * detectionRange);
        Gizmos.DrawLine(transform.position + leftRay * detectionRange, transform.position + rightRay * detectionRange);
    }
}