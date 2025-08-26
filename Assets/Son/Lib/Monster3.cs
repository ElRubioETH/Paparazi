using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI3 : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    public Transform player;
    private Rigidbody playerRigidbody;
    private CharacterController playerCharacterController;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public float patrolSpeed = 2f;
    public float patrolWaitTime = 2f;

    [Header("Detection Ranges")]
    public float nearRange = 5f;
    public float farRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 15f;   // chỉnh chase speed ở đây
    public float maxChaseDistance = 25f;

    [Header("NavMesh Settings")]
    public float chaseAcceleration = 80f; // tốc độ tăng tốc khi đuổi
    public float chaseAngularSpeed = 720f; // tốc độ xoay khi đuổi

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

    private float lastDestinationUpdateTime;
    private Vector3 lastPlayerPosition;
    private float destinationUpdateInterval = 0.3f;
    private float minDistanceToUpdateDestination = 1f;

    private Vector3 previousPlayerPosition;

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

        playerRigidbody = player.GetComponent<Rigidbody>();
        playerCharacterController = player.GetComponent<CharacterController>();

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
            {
                Debug.LogError($"EnemyAI3: Patrol point {i} is null!");
                enabled = false;
                return;
            }
        }

        // cấu hình mặc định cho agent
        agent.speed = patrolSpeed;
        agent.acceleration = 20f;   // nhanh hơn mặc định
        agent.angularSpeed = 360f;
        agent.stoppingDistance = attackRange - 0.2f;

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.updatePosition = true;

        if (audioSource != null) audioSource.playOnAwake = false;

        currentState = State.Patrol;
        previousPlayerPosition = player.position;

        Debug.Log("EnemyAI3: Starting patrol system...");
        StartCoroutine(InitializePatrol());
    }

    IEnumerator InitializePatrol()
    {
        yield return null;

        if (patrolPoints.Length > 0 && patrolPoints[currentPatrolIndex] != null)
        {
            Vector3 targetPos = patrolPoints[currentPatrolIndex].position;
            Debug.Log($"Setting initial destination to patrol point {currentPatrolIndex}: {targetPos}");
            if (agent.SetDestination(targetPos))
            {
                if (animator != null) animator.SetBool("isWalking", true);
            }
        }
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"State: {currentState}, Distance: {distanceToPlayer:F2}");
            Debug.Log($"Agent: velocity={agent.velocity.magnitude:F2}, hasPath={agent.hasPath}, remaining={agent.remainingDistance:F2}");
        }

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

        if (currentState == State.Chase && player != null)
        {
            if (Time.time - lastDestinationUpdateTime >= destinationUpdateInterval ||
                Vector3.Distance(player.position, lastPlayerPosition) > minDistanceToUpdateDestination)
            {
                agent.SetDestination(player.position);
                lastDestinationUpdateTime = Time.time;
                lastPlayerPosition = player.position;
            }

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
                SwitchState(State.Patrol);
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
            Vector3 targetPos = patrolPoints[currentPatrolIndex].position;
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(targetPos, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(path);
                }
                else
                {
                    agent.SetDestination(targetPos);
                }
            }
        }
    }

    void DetermineNextState(float distanceToPlayer)
    {
        if (currentState == State.PatrolWaiting) return;

        State nextState = currentState;

        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            nextState = State.Attack;
        }
        else if (distanceToPlayer <= nearRange)
        {
            nextState = State.Chase;
        }
        else if (distanceToPlayer <= farRange && distanceToPlayer <= maxChaseDistance)
        {
            nextState = State.Chase;
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
        Debug.Log($"Switching from {currentState} to {newState}");
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
                agent.acceleration = 20f;
                agent.angularSpeed = 360f;
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
                agent.acceleration = chaseAcceleration;
                agent.angularSpeed = chaseAngularSpeed;
                if (animator != null) animator.SetBool("isRunning", true);
                if (player != null)
                {
                    agent.SetDestination(player.position);
                    lastPlayerPosition = player.position;
                    lastDestinationUpdateTime = Time.time;
                }
                break;

            case State.Attack:
                agent.isStopped = true;
                Attack();
                break;

            case State.Idle:
                agent.isStopped = true;
                if (animator != null) animator.SetBool("isIdle", true);
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
        if (player != null) transform.LookAt(player);

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);

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
            if (targetClip != null) audioSource.Play();
        }
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
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

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.color = (i == currentPatrolIndex) ? Color.green : Color.blue;
                    Gizmos.DrawWireCube(patrolPoints[i].position, Vector3.one * 1.2f);

#if UNITY_EDITOR
                    UnityEditor.Handles.Label(patrolPoints[i].position + Vector3.up, i.ToString());
#endif

                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                    else if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }
        }

        if (Application.isPlaying && agent != null && agent.hasPath)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawWireSphere(agent.destination, 0.3f);
        }
    }
}
