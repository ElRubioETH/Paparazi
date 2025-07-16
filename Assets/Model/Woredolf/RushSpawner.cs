using UnityEngine;

public class SpawnMoverTrigger : MonoBehaviour
{
    public GameObject objectToSpawn;      // Prefab con ma
    public Transform spawnPoint;          // Vị trí spawn
    public Transform[] waypoints;         // Các điểm di chuyển
    public GameObject DeadPanel;
    [Header("References from Scene")]
    public Wardrobe wardrobeScript;
    public Doors doorScript;
    public Transform player; // Gán player trong Inspector

    private bool hasSpawned = false;
    private void Start()
    {

        DeadPanel.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;

        hasSpawned = true;

        GameObject obj = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);

        // 👉 Gán đường đi và script từ scene cho bản sao mới spawn
        MovingObject mover = obj.GetComponent<MovingObject>();
        if (mover != null)
        {
            mover.waypoints = waypoints;
            mover.wardrobeScript = wardrobeScript;
            mover.DeadPanel = DeadPanel;
        }

        Debug.Log("👻 Spawn ma, gán route + kiểm tra an toàn xong!");
    }
}
