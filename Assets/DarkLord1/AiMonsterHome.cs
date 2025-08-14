using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;
    public Animator animator;

    [Header("Movement Settings")]
    public float patrolSpeed = 1f;
    public float chaseSpeed = 6f;
    public float waitTimeAfterChase = 3f;

    [Header("Combat Settings")]
    public float attackRange = 2f;

    [Header("Vision Settings")]
    public float fieldOfView = 90f; // Góc nhìn
    public float viewDistance = 15f; // T?m nhìn t?i ?a
    public LayerMask playerLayer;
    public LayerMask obstacleMask;
    public LayerMask safeRoomMask;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = patrolSpeed;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (isWaiting) return;

        bool playerInSafeRoom = CheckPlayerInSafeRoom();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (CanSeePlayer() && !playerInSafeRoom && !isChasing)
        {
            StartChasing();
        }
        else if (isChasing && playerInSafeRoom)
        {
            StopChasing();
        }
        else if (isChasing && distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (isChasing)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isChasing", true);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void StartChasing()
    {
        isChasing = true;
        agent.speed = chaseSpeed;
        animator.SetBool("isPatrolling", false);
        animator.SetTrigger("Roar");
        agent.SetDestination(player.position);
    }

    void StopChasing()
    {
        isChasing = false;
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
        StartCoroutine(WaitAndResumePatrol());
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        animator.SetBool("isAttacking", true);
        animator.SetBool("isChasing", false);
    }

    IEnumerator WaitAndResumePatrol()
    {
        isWaiting = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTimeAfterChase);
        agent.isStopped = false;
        isWaiting = false;
        GoToNextPatrolPoint();
    }

    bool CheckPlayerInSafeRoom()
    {
        Collider[] colliders = Physics.OverlapSphere(player.position, 0.5f, safeRoomMask);
        return colliders.Length > 0;
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle <= fieldOfView / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        // T?m nhìn
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 left = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);

        // V? ph?m vi t?n công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
