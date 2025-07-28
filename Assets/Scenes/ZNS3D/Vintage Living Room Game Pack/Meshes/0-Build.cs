using UnityEngine;

public class FloorBuilder : MonoBehaviour
{
    public GameObject floorPrefab;
    public int width = 3;
    public int height = 3;
    public float tileSize = 1f;

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 spawnPos = new Vector3(x * tileSize, 0, z * tileSize);
                Instantiate(floorPrefab, spawnPos, Quaternion.identity, transform);
            }
        }
    }
}
