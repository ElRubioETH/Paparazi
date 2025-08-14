using UnityEngine;
using UnityEngine.AI;

public class SimplePatrolTest : MonoBehaviour
{
    public Transform[] patrolPoints;
    private NavMeshAgent agent;
    private int currentIndex = 0;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Kiểm tra cơ bản
        Debug.Log("=== PATROL TEST START ===");
        Debug.Log($"Agent exists: {agent != null}");
        
        if (agent == null)
        {
            Debug.LogError("NO NAVMESHAGENT FOUND!");
            return;
        }
        
        Debug.Log($"Agent on NavMesh: {agent.isOnNavMesh}");
        Debug.Log($"Agent enabled: {agent.enabled}");
        Debug.Log($"Patrol points count: {(patrolPoints != null ? patrolPoints.Length : 0)}");
        
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("NO PATROL POINTS!");
            return;
        }
        
        // Test di chuyển đến điểm đầu tiên
        MoveToCurrentPoint();
    }
    
    void Update()
    {
        if (agent == null || patrolPoints == null) return;
        
        // Debug mỗi giây
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Position: {transform.position}");
            Debug.Log($"Destination: {agent.destination}");
            Debug.Log($"Remaining distance: {agent.remainingDistance}");
            Debug.Log($"Velocity: {agent.velocity.magnitude}");
            Debug.Log($"Has path: {agent.hasPath}");
            Debug.Log($"Path pending: {agent.pathPending}");
            Debug.Log($"Is stopped: {agent.isStopped}");
            Debug.Log("---");
        }
        
        // Kiểm tra đã đến điểm chưa
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Debug.Log("Reached point! Moving to next...");
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
            MoveToCurrentPoint();
        }
    }
    
    void MoveToCurrentPoint()
    {
        if (patrolPoints[currentIndex] != null)
        {
            Vector3 target = patrolPoints[currentIndex].position;
            Debug.Log($"Moving to point {currentIndex}: {target}");
            
            bool success = agent.SetDestination(target);
            Debug.Log($"SetDestination success: {success}");
        }
        else
        {
            Debug.LogError($"Patrol point {currentIndex} is NULL!");
        }
    }
    
    void OnDrawGizmos()
    {
        // Vẽ patrol points
        if (patrolPoints != null)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.color = (i == currentIndex) ? Color.red : Color.blue;
                    Gizmos.DrawWireCube(patrolPoints[i].position, Vector3.one);
                    
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(patrolPoints[i].position + Vector3.up, i.ToString());
                    #endif
                }
            }
        }
        
        // Vẽ đường đến destination
        if (Application.isPlaying && agent != null && agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}