using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadStage4OnTouch : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Stage4");
        }
    }
}
