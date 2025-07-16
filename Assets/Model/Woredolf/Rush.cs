using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float arriveThreshold = 0.1f;
    public GameObject DeadPanel;
    [Header("Optional - Player Safety")]
    public Wardrobe wardrobeScript;

    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 🔁 Xử lý cập nhật waypoint TRƯỚC
        Transform target = waypoints[currentWaypoint];
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < arriveThreshold)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                Debug.Log("👻 Tới điểm cuối → Xoá ma");
                Destroy(gameObject);
                return;
            }
            target = waypoints[currentWaypoint]; // Update lại target mới
        }

        // 👉 Bây giờ mới move
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
            transform.forward = direction;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Nếu player safe thì bỏ qua
        if (wardrobeScript != null && wardrobeScript.safe)
        {
            Debug.Log("🛡️ Player đang núp → không bị freeze");
            return;
        }

        // Player không safe → freeze
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Debug.Log("💀 Player bị freeze bởi ma");
            DeadPanel.SetActive(true);
        }
    }
}
