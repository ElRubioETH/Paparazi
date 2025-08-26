using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI3 : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    public Transform player;
    private PlayerStateTracker playerController;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // mảng patrol point
    private int currentPatrolIndex = 0;
    public float patrolSpeed = 2f;

    [Header("Range Settings")]
    public float nearRange = 5f;
    public float farRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 25f;

    private enum State { Patrol, Chase, Attack, Idle }
    private State currentState;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Audio Clips")]
    public AudioClip patrolSound;
    public AudioClip chaseSound;
    public AudioClip attackSound;

    private bool isWaitingToPatrol = false;
    private float idleTimer = 0f;
    public float maxIdleTime = 3f;

    private float lastDestinationUpdateTime;
    private Vector3 lastPlayerPosition;
    private float destinationUpdateInterval = 0.3f;
    private float minDistanceToUpdateDestination = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (agent == null || !agent.isOnNavMesh || animator == null || audioSource == null || player == null || player.GetComponent<PlayerStateTracker>() == null || patrolPoints.Length == 0)
        {
            enabled = false;
            return;
        }

        playerController = player.GetComponent<PlayerStateTracker>();
        currentState = State.Patrol;
        agent.speed = patrolSpeed;
        audioSource.playOnAwake = false;

        // bắt đầu với patrol point đầu tiên
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        SwitchState(State.Patrol);
    }

    void Update()
    {
        if (playerController == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        DetermineNextState(distanceToPlayer);
        ManageAudio(distanceToPlayer);

        if (currentState == State.Chase && player != null)
        {
            if (Time.time - lastDestinationUpdateTime >= destinationUpdateInterval || Vector3.Distance(player.position, lastPlayerPosition) > minDistanceToUpdateDestination)
            {
                agent.SetDestination(player.position);
                lastDestinationUpdateTime = Time.time;
                lastPlayerPosition = player.position;
            }

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        if (currentState == State.Patrol && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            // tăng index và quay vòng
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        if (currentState == State.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= maxIdleTime)
            {
                isWaitingToPatrol = false;
                if (distanceToPlayer > nearRange || !playerController.isMoving)
                {
                    SwitchState(State.Patrol);
                }
                else
                {
                    SwitchState(State.Chase);
                }
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
            isWaitingToPatrol = false;
        }
    }

    void DetermineNextState(float distanceToPlayer)
    {
        bool isCrouching = playerController.isCrouching;
        bool isMoving = playerController.isMoving;
        State nextState = currentState;

        if (distanceToPlayer <= attackRange && isMoving && Time.time - lastAttackTime >= attackCooldown)
        {
            nextState = State.Attack;
        }
        else if (distanceToPlayer <= nearRange)
        {
            if (isMoving)
            {
                nextState = State.Chase;
            }
            else if (currentState == State.Chase || currentState == State.Attack)
            {
                nextState = State.Idle;
                isWaitingToPatrol = true;
            }
            else if (currentState != State.Idle)
            {
                nextState = State.Patrol;
            }
        }
        else if (distanceToPlayer <= farRange && distanceToPlayer <= maxChaseDistance)
        {
            if (isMoving && !isCrouching)
            {
                nextState = State.Chase;
            }
            else if (currentState == State.Chase || currentState == State.Attack)
            {
                nextState = State.Idle;
                isWaitingToPatrol = true;
            }
            else
            {
                nextState = State.Patrol;
            }
        }
        else if (currentState == State.Chase || currentState == State.Attack)
        {
            nextState = State.Idle;
            isWaitingToPatrol = true;
        }
        else
        {
            nextState = State.Patrol;
        }

        if (nextState != currentState)
        {
            SwitchState(nextState);
        }
    }

    void SwitchState(State newState)
    {
        currentState = newState;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdle", false);

        agent.isStopped = (newState == State.Idle || newState == State.Attack);

        switch (currentState)
        {
            case State.Patrol:
                agent.speed = patrolSpeed;
                animator.SetBool("isWalking", true);
                if (patrolPoints.Length > 0)
                {
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
                break;
            case State.Chase:
                agent.speed = chaseSpeed;
                animator.SetBool("isRunning", true);
                if (player != null)
                {
                    agent.SetDestination(player.position);
                    lastPlayerPosition = player.position;
                    lastDestinationUpdateTime = Time.time;
                }
                break;
            case State.Attack:
                Attack();
                break;
            case State.Idle:
                animator.SetBool("isIdle", true);
                if (player != null)
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
                break;
        }
    }

    void Attack()
    {
        agent.isStopped = true;
        if (player != null)
        {
            transform.LookAt(player);
        }

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdle", true);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("attack");
            lastAttackTime = Time.time;
        }
    }

    void ManageAudio(float distanceToPlayer)
    {
        AudioClip targetClip = null;
        bool loop = false;

        switch (currentState)
        {
            case State.Patrol:
                if (distanceToPlayer <= 20f)
                {
                    targetClip = patrolSound;
                    loop = true;
                }
                break;
            case State.Chase:
                targetClip = chaseSound;
                loop = true;
                break;
        }

        if (audioSource.clip != targetClip)
        {
            audioSource.Stop();
            audioSource.clip = targetClip;
            audioSource.loop = loop;
            if (targetClip != null)
            {
                audioSource.Play();
            }
        }
    }

    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, nearRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        if (patrolPoints != null)
        {
            foreach (var p in patrolPoints)
            {
                if (p != null)
                    Gizmos.DrawWireCube(p.position, Vector3.one);
            }
        }
    }
}
