using MimicSpace;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float arriveThreshold = 0.1f;
    public float rotateSpeed = 5f; // ← tốc độ xoay
    public GameObject DeadPanel;
    public AudioSource appear;
    [Header("Optional - Player Safety")]
    public Wardrobe wardrobeScript;
    Mimic myMimic;
    public AudioSource KilledPlayer;
    private int currentWaypoint = 0;

    private void Start()
    {
        myMimic = GetComponent<Mimic>();
    }

    void Update()
    {

        if (waypoints == null || waypoints.Length == 0) return;

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

            target = waypoints[currentWaypoint];
        }

        // 👉 Move ONLY — không xoay
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // ❌ Không đụng tới transform.forward hay rotation
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (wardrobeScript != null && wardrobeScript.safe)
        {
            Debug.Log("🛡️ Player đang núp → không bị freeze");
            return;
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Debug.Log("💀 Player bị freeze bởi ma");
            DeadPanel.SetActive(true);
            appear.Stop();
            KilledPlayer.Play();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
