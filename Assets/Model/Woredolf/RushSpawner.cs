using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnMoverTrigger : MonoBehaviour
{
    public GameObject objectToSpawn;      // Prefab con ma
    public Transform spawnPoint;          // Vị trí spawn
    public Transform[] waypoints;         // Các điểm di chuyển
    public GameObject DeadPanel;
    public Light[] flickerLights;         // 🆕 Mảng đèn sẽ chớp tắt

    [Header("References from Scene")]
    public Wardrobe wardrobeScript;
    public Doors doorScript;
    public Transform player;

    private bool hasSpawned = false;

    private void Start()
    {
        DeadPanel.SetActive(false);

        // ✅ Tìm tất cả GameObject trong scene
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<Light> lightList = new List<Light>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Point Light")
            {
                Light light = obj.GetComponent<Light>();
                if (light != null)
                {
                    lightList.Add(light);
                }
            }
        }

        flickerLights = lightList.ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;

        hasSpawned = true;

        StartCoroutine(FlickerThenSpawn());
    }

    private IEnumerator FlickerThenSpawn()
    {
        float flickerDuration = 3f;
        float timer = 0f;
        float interval = 0.1f;

        while (timer < flickerDuration)
        {
            foreach (Light light in flickerLights)
            {
                if (light != null)
                    light.enabled = !light.enabled;
            }

            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        // Đảm bảo tất cả đèn bật lại sau khi flicker
        foreach (Light light in flickerLights)
        {
            if (light != null)
                light.enabled = true;
        }

        // 👉 Spawn object sau khi flicker xong
        GameObject obj = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);

        MovingObject mover = obj.GetComponent<MovingObject>();
        if (mover != null)
        {
            mover.waypoints = waypoints;
            mover.wardrobeScript = wardrobeScript;
            mover.DeadPanel = DeadPanel;
        }

        Debug.Log("👻 Spawn ma sau khi flicker đèn xong!");
    }
}
