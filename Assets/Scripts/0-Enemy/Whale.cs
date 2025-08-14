using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    public Transform[] rayPoints; // 4 điểm phát ray
    public float rayLength = 2f;
    public LayerMask playerLayer;

    public float damage = 10f;
    public AudioSource audioSource;
    public AudioClip whaleSound; // tiếng "rú" lâu lâu
    public AudioClip screamSound; // tiếng hét khi player vào ray

    public float whaleSoundIntervalMin = 5f;
    public float whaleSoundIntervalMax = 10f;

    private float whaleSoundTimer;

    void Start()
    {
        // Hẹn giờ ngẫu nhiên lần đầu
        whaleSoundTimer = Random.Range(whaleSoundIntervalMin, whaleSoundIntervalMax);
    }

    void Update()
    {
        // Kiểm tra 4 ray
        foreach (Transform rayPoint in rayPoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayPoint.position, rayPoint.forward, out hit, rayLength, playerLayer))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);

                    // Phát tiếng hét
                    if (!audioSource.isPlaying)
                        audioSource.PlayOneShot(screamSound);
                }
            }

            // Vẽ ray debug trong Scene
            Debug.DrawRay(rayPoint.position, rayPoint.forward * rayLength, Color.red);
        }

        // Đếm thời gian phát tiếng cá voi
        whaleSoundTimer -= Time.deltaTime;
        if (whaleSoundTimer <= 0)
        {
            audioSource.PlayOneShot(whaleSound);
            whaleSoundTimer = Random.Range(whaleSoundIntervalMin, whaleSoundIntervalMax);
        }
    }
}
