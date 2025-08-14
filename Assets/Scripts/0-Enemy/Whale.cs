using UnityEngine;

public class FlyingWhale : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float rotationSpeed = 2f; // tốc độ xoay mượt
    public float arriveDistance = 0.5f;
    private int currentWaypointIndex = 0;

    [Header("Attack Settings")]
    public float damage = 10f;
    public Collider[] attackColliders;

    [Header("Sound")]
    public AudioSource audioSource;     // Dùng chung cho cả whaleSound và screamSound
    public AudioClip whaleSound;
    public AudioClip screamSound;
    public float whaleSoundIntervalMin = 5f;
    public float whaleSoundIntervalMax = 10f;
    private float whaleSoundTimer;

    private bool isScreaming = false;   // Tránh overlap nhiều scream liên tiếp

    void Start()
    {
        foreach (Collider col in attackColliders)
        {
            col.isTrigger = true;
            TriggerForwarder forwarder = col.gameObject.AddComponent<TriggerForwarder>();
            forwarder.Setup(this);
        }

        whaleSoundTimer = Random.Range(whaleSoundIntervalMin, whaleSoundIntervalMax);
    }

    void Update()
    {
        MoveBetweenWaypoints();
        WhaleSoundTimer();
    }

    void MoveBetweenWaypoints()
    {
        if (waypoints.Length == 0) return;

        Transform targetPoint = waypoints[currentWaypointIndex];

        // Di chuyển
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        // Xoay mượt về hướng target
        Vector3 dir = (targetPoint.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Kiểm tra tới waypoint
        if (Vector3.Distance(transform.position, targetPoint.position) <= arriveDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void WhaleSoundTimer()
    {
        if (isScreaming) return; // đang scream thì không phát whaleSound

        whaleSoundTimer -= Time.deltaTime;
        if (whaleSoundTimer <= 0)
        {
            audioSource.clip = whaleSound;
            audioSource.Play();
            whaleSoundTimer = Random.Range(whaleSoundIntervalMin, whaleSoundIntervalMax);
        }
    }

    public void OnPlayerEnter(Collider playerCollider)
    {
        PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
        if (playerHealth != null && !isScreaming)
        {
            playerHealth.TakeDamage(damage);

            // Dừng whale sound và phát scream
            audioSource.Stop();
            audioSource.clip = screamSound;
            audioSource.Play();

            isScreaming = true;
            Invoke(nameof(ResetScream), screamSound.length);
        }
    }

    void ResetScream()
    {
        isScreaming = false;
    }

    private class TriggerForwarder : MonoBehaviour
    {
        private FlyingWhale whale;
        public void Setup(FlyingWhale w) => whale = w;

        void OnTriggerEnter(Collider other)
        {
            whale.OnPlayerEnter(other);
        }
    }
}
