using UnityEngine;
using UnityEngine.AI;

public class EnemyAI3 : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    public Transform player;
    private PlayerStateTracker playerController;

    public Transform patrolPointA;
    public Transform patrolPointB;
    private Transform currentPatrolTarget;
    public float patrolSpeed = 2f;

    public float nearRange = 5f;
    public float farRange = 15f;
    public float attackRange = 2f;
    public float chaseSpeed = 5f;
    public float maxChaseDistance = 25f;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    public float attackCooldown = 2f;
    private float lastAttackTime;

    public AudioClip patrolSound;
    public AudioClip chaseSound;
    public AudioClip attackSound;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerController = player.GetComponent<PlayerStateTracker>();

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
        if (playerController == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool inNearZone = distanceToPlayer <= nearRange;
        bool inFarZone = distanceToPlayer > nearRange && distanceToPlayer <= farRange;

        bool isMoving = playerController.isMoving;
        bool isCrouching = playerController.isCrouching;

        currentState = State.Patrol;

        if (inFarZone)
        {
            if (isMoving && !isCrouching)
            {
                currentState = State.Chase;
            }
        }
        else if (inNearZone)
        {
            if (isMoving)
            {
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attack;
                }
                else
                {
                    currentState = State.Chase;
                }
            }
            else
            {
                currentState = State.Patrol;
            }
        }

        if (distanceToPlayer > maxChaseDistance && currentState == State.Chase)
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
        }
    }

    void Patrol()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            currentPatrolTarget = currentPatrolTarget == patrolPointA ? patrolPointB : patrolPointA;
            agent.SetDestination(currentPatrolTarget.position);
        }
        else if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        {
            agent.SetDestination(currentPatrolTarget.position);
        }
    }

    void Chase()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);

        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (!agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
            return;
        }

        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("attack");
            lastAttackTime = Time.time;
            
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
        {
            audioSource.Stop();
        }

        if (chaseSound != null && currentState == State.Chase && distanceToPlayer <= farRange && audioSource.clip != chaseSound)
        {
            audioSource.Stop();
            audioSource.clip = chaseSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if ((distanceToPlayer > farRange || currentState != State.Chase) && audioSource.isPlaying && audioSource.clip == chaseSound)
        {
            audioSource.Stop();
        }

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, nearRange);

        Gizmos.color = Color.blue;
        if (patrolPointA != null) Gizmos.DrawWireCube(patrolPointA.position, Vector3.one);
        if (patrolPointB != null) Gizmos.DrawWireCube(patrolPointB.position, Vector3.one);

        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 20f);

        Gizmos.color = new Color(0.5f, 0f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }
}
