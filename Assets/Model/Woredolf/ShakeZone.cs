using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class ShakeZoneTrigger : MonoBehaviour
{
    public ShakeData rush;

    public bool shake;

    public void Start()
    {
        shake = false;
    }
    private void Update()
    {
        if ( shake == true)
        {
            CameraShakerHandler.Shake(rush);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shake = true;     
        }
    }

}
