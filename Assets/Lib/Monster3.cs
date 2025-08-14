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
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 25f;

    [Header("Player Detection")]
    public float movementThreshold = 0.1f; // Tốc độ tối thiểu để coi là đang di chuyển
    public float crouchDetectionHeight = 1.5f; // Chiều cao để phát hiện crouch

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

    // Để theo dõi movement của player
    private Vector3 previousPlayerPosition;
    private bool playerIsMoving = false;
    private bool playerIsCrouching = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Kiểm tra components cơ bản
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

        // Tìm component di chuyển của player
        playerRigidbody = player.GetComponent<Rigidbody>();
        playerCharacterController = player.GetComponent<CharacterController>();

        if (playerRigidbody == null && playerCharacterController == null)
        {
            Debug.LogWarning("EnemyAI3: Player has no Rigidbody or CharacterController. Will use position tracking for movement detection.");
        }

        // Kiểm tra patrol points hợp lệ
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
            {
                Debug.LogError($"EnemyAI3: Patrol point {i} is null!");
                enabled = false;
                return;
            }
        }

        // Setup agent
        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.updatePosition = true;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        currentState = State.Patrol;
        previousPlayerPosition = player.position;
        
        Debug.Log("EnemyAI3: Starting patrol system...");
        
        // Bắt đầu patrol
        StartCoroutine(InitializePatrol());
    }

    IEnumerator InitializePatrol()
    {
        yield return null; // Wait one frame
        
        if (patrolPoints.Length > 0 && patrolPoints[currentPatrolIndex] != null)
        {
            Vector3 targetPos = patrolPoints[currentPatrolIndex].position;
            Debug.Log($"Setting initial destination to patrol point {currentPatrolIndex}: {targetPos}");
            
            if (agent.SetDestination(targetPos))
            {
                Debug.Log("Successfully set destination");
                if (animator != null)
                {
                    animator.SetBool("isWalking", true);
                }
            }
            else
            {
                Debug.LogError("Failed to set destination");
            }
        }
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        // Cập nhật thông tin player
        UpdatePlayerState();
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Debug thông tin cơ bản
        if (Time.frameCount % 60 == 0) // Log mỗi 60 frames (khoảng 1 giây)
        {
            Debug.Log($"State: {currentState}, Distance: {distanceToPlayer:F2}, Player moving: {playerIsMoving}, Player crouching: {playerIsCrouching}");
            Debug.Log($"Agent: velocity={agent.velocity.magnitude:F2}, hasPath={agent.hasPath}, remaining={agent.remainingDistance:F2}");
        }
        
        // Xử lý patrol waiting state
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
        
        // Chỉ thay đổi state nếu player ở gần
        if (distanceToPlayer <= farRange)
        {
            DetermineNextState(distanceToPlayer);
        }
        else if (currentState != State.Patrol && currentState != State.PatrolWaiting)
        {
            // Nếu player ở xa, trở về patrol
            SwitchState(State.Patrol);
        }
        
        ManageAudio(distanceToPlayer);

        // Chase behavior
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

        // Patrol movement logic
        if (currentState == State.Patrol)
        {
            if (!agent.pathPending && agent.hasPath)
            {
                if (agent.remainingDistance < 0.5f && agent.velocity.magnitude < 0.1f)
                {
                    Debug.Log("Reached patrol point, switching to waiting");
                    SwitchState(State.PatrolWaiting);
                }
            }
            else if (!agent.hasPath && !agent.pathPending)
            {
                Debug.Log("No path found, retrying...");
                GoToNextPatrolPoint();
            }
        }

        // Idle behavior
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

    void UpdatePlayerState()
    {
        // Phát hiện movement bằng cách so sánh position
        Vector3 currentPlayerPosition = player.position;
        float movementSpeed = 0f;

        if (playerRigidbody != null)
        {
            movementSpeed = playerRigidbody.linearVelocity.magnitude;
        }
        else if (playerCharacterController != null)
        {
            movementSpeed = playerCharacterController.velocity.magnitude;
        }
        else
        {
            // Fallback: tính toán từ position
            movementSpeed = Vector3.Distance(currentPlayerPosition, previousPlayerPosition) / Time.deltaTime;
        }

        playerIsMoving = movementSpeed > movementThreshold;
        
        // Phát hiện crouching bằng cách kiểm tra chiều cao
        // Giả sử player bình thường cao hơn crouchDetectionHeight
        if (playerCharacterController != null)
        {
            playerIsCrouching = playerCharacterController.height < crouchDetectionHeight;
        }
        else
        {
            // Fallback: kiểm tra scale hoặc position Y
            playerIsCrouching = player.localScale.y < 1f || player.position.y < transform.position.y - 0.5f;
        }

        previousPlayerPosition = currentPlayerPosition;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) 
        {
            Debug.LogError("No patrol points available");
            return;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        
        if (patrolPoints[currentPatrolIndex] != null)
        {
            Vector3 targetPos = patrolPoints[currentPatrolIndex].position;
            Debug.Log($"Moving to patrol point {currentPatrolIndex}: {targetPos}");
            
            // Kiểm tra path
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(targetPos, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(path);
                    Debug.Log("Path calculated successfully");
                }
                else
                {
                    Debug.LogWarning($"Incomplete path to patrol point {currentPatrolIndex}, trying SetDestination anyway");
                    agent.SetDestination(targetPos);
                }
            }
            else
            {
                Debug.LogError($"Cannot calculate path to patrol point {currentPatrolIndex}");
                // Thử điểm tiếp theo
                if (patrolPoints.Length > 1)
                {
                    GoToNextPatrolPoint();
                }
            }
        }
        else
        {
            Debug.LogWarning($"Patrol point {currentPatrolIndex} is null!");
        }
    }

    void DetermineNextState(float distanceToPlayer)
    {
        // Không thay đổi state nếu đang trong PatrolWaiting
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
            else if (currentState != State.Idle)
            {
                nextState = State.Patrol;
            }
        }
        else if (distanceToPlayer <= farRange && distanceToPlayer <= maxChaseDistance)
        {
            if (playerIsMoving && !playerIsCrouching)
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
        Debug.Log($"Switching from {currentState} to {newState}");
        currentState = newState;

        // Reset animation flags
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }

        // Reset agent state
        agent.isStopped = false;

        switch (currentState)
        {
            case State.Patrol:
                agent.speed = patrolSpeed;
                if (animator != null) animator.SetBool("isWalking", true);
                agent.isStopped = false;
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
                agent.isStopped = false;
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
        if (player != null)
        {
            transform.LookAt(player);
        }

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
            if (targetClip != null)
            {
                audioSource.Play();
            }
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
        // Vẽ detection ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, nearRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ patrol points và đường đi
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    // Highlight current patrol point
                    if (i == currentPatrolIndex)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(patrolPoints[i].position, Vector3.one * 1.5f);
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(patrolPoints[i].position, Vector3.one);
                    }

                    // Vẽ số thứ tự
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(patrolPoints[i].position + Vector3.up, i.ToString());
                    #endif

                    // Vẽ đường nối giữa các patrol points
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                    else if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
                    {
                        // Nối điểm cuối với điểm đầu
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }
        }

        // Vẽ current destination
        if (Application.isPlaying && agent != null && agent.hasPath)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawWireSphere(agent.destination, 0.3f);
        }

        // Vẽ player info
        if (Application.isPlaying && player != null)
        {
            Gizmos.color = playerIsMoving ? Color.green : Color.red;
            Gizmos.DrawWireSphere(player.position, 0.5f);
        }
    }
}