using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI3 : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    public Transform player;
    private PlayerStateTracker playerState; // dùng để lấy trạng thái player

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public float patrolSpeed = 2f;
    public float patrolWaitTime = 2f;

    [Header("Detection Ranges")]
    public float nearRange = 5f;
    public float farRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 25f;

    private enum State { Patrol, Chase, Attack, Idle, PatrolWaiting }
    private State currentState;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Sounds")]
    public AudioClip patrolSound;
    public AudioClip chaseSound;
    public AudioClip attackSound;

    private bool isWaitingToPatrol = false;
    private float idleTimer = 0f;
    public float maxIdleTime = 3f;
    private float patrolWaitTimer = 0f;

    private bool playerIsMoving = false;
    private bool playerIsCrouching = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (agent == null)
        {
            Debug.LogError("EnemyAI3: NavMeshAgent component missing!");
            enabled = false;
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("EnemyAI3: Agent is not on NavMesh! Make sure NavMesh is baked and enemy is on it.");
            enabled = false;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("EnemyAI3: No patrol points assigned!");
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("EnemyAI3: Player not assigned!");
            enabled = false;
            return;
        }

        playerState = player.GetComponent<PlayerStateTracker>();
        if (playerState == null)
        {
            Debug.LogError("EnemyAI3: PlayerStateTracker missing on Player!");
        }

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
            {
                Debug.LogError($"EnemyAI3: Patrol point {i} is null!");
                enabled = false;
                return;
            }
        }

        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.updatePosition = true;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        currentState = State.Patrol;
        Debug.Log("EnemyAI3: Starting patrol system...");

        StartCoroutine(InitializePatrol());
    }

    IEnumerator InitializePatrol()
    {
        yield return null;
        if (patrolPoints.Length > 0 && patrolPoints[currentPatrolIndex] != null)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            if (animator != null)
            {
                animator.SetBool("isWalking", true);
            }
        }
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        // lấy trạng thái từ PlayerStateTracker
        if (playerState != null)
        {
            playerIsMoving = playerState.isMoving;
            playerIsCrouching = playerState.isCrouching;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == State.PatrolWaiting)
        {
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                SwitchState(State.Patrol);
                return;
            }
        }

        if (distanceToPlayer <= farRange)
        {
            DetermineNextState(distanceToPlayer);
        }
        else if (currentState != State.Patrol && currentState != State.PatrolWaiting)
        {
            SwitchState(State.Patrol);
        }

        ManageAudio(distanceToPlayer);

        // --- FIX CHASE ---
        if (currentState == State.Chase && player != null)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false; // đảm bảo không bị stop
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);

                Debug.Log($"[CHASE] isStopped={agent.isStopped}, hasPath={agent.hasPath}, " +
                          $"pathStatus={agent.pathStatus}, velocity={agent.velocity.magnitude:F2}, " +
                          $"remaining={agent.remainingDistance:F2}");
            }

            // quay mặt về phía player
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        if (currentState == State.Patrol)
        {
            if (!agent.pathPending && agent.hasPath)
            {
                if (agent.remainingDistance < 0.5f && agent.velocity.magnitude < 0.1f)
                {
                    SwitchState(State.PatrolWaiting);
                }
            }
            else if (!agent.hasPath && !agent.pathPending)
            {
                GoToNextPatrolPoint();
            }
        }

        if (currentState == State.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= maxIdleTime)
            {
                isWaitingToPatrol = false;
                if (distanceToPlayer > nearRange || !playerIsMoving)
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
        else if (currentState != State.PatrolWaiting)
        {
            idleTimer = 0f;
            isWaitingToPatrol = false;
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        if (patrolPoints[currentPatrolIndex] != null)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void DetermineNextState(float distanceToPlayer)
    {
        if (currentState == State.PatrolWaiting) return;

        State nextState = currentState;

        if (distanceToPlayer <= attackRange && playerIsMoving && Time.time - lastAttackTime >= attackCooldown)
        {
            nextState = State.Attack;
        }
        else if (distanceToPlayer <= nearRange)
        {
            if (playerIsMoving)
            {
                nextState = State.Chase;
            }
            else if (currentState == State.Chase || currentState == State.Attack)
            {
                nextState = State.Idle;
                isWaitingToPatrol = true;
            }
        }
        else if (distanceToPlayer <= farRange && distanceToPlayer <= maxChaseDistance)
        {
            if (playerIsMoving && !playerIsCrouching)
            {
                nextState = State.Chase;
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

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }

        agent.isStopped = false;

        switch (currentState)
        {
            case State.Patrol:
                agent.speed = patrolSpeed;
                if (animator != null) animator.SetBool("isWalking", true);
                GoToNextPatrolPoint();
                break;

            case State.PatrolWaiting:
                agent.isStopped = true;
                if (animator != null) animator.SetBool("isIdle", true);
                patrolWaitTimer = 0f;
                break;

            case State.Chase:
                agent.speed = chaseSpeed;
                if (animator != null) animator.SetBool("isRunning", true);
                if (player != null) agent.SetDestination(player.position);
                break;

            case State.Attack:
                agent.isStopped = true;
                Attack();
                break;

            case State.Idle:
                agent.isStopped = true;
                if (animator != null) animator.SetBool("isIdle", true);
                break;
        }
    }

    void Attack()
    {
        agent.isStopped = true;
        if (player != null) transform.LookAt(player);
        if (animator != null)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("attack");
                lastAttackTime = Time.time;
            }
        }
    }

    void ManageAudio(float distanceToPlayer)
    {
        if (audioSource == null) return;
        AudioClip targetClip = null;
        bool loop = false;
        switch (currentState)
        {
            case State.Patrol:
            case State.PatrolWaiting:
                if (distanceToPlayer <= 20f) { targetClip = patrolSound; loop = true; }
                break;
            case State.Chase:
                targetClip = chaseSound; loop = true;
                break;
        }
        if (audioSource.clip != targetClip)
        {
            audioSource.Stop();
            audioSource.clip = targetClip;
            audioSource.loop = loop;
            if (targetClip != null) audioSource.Play();
        }
    }
}
