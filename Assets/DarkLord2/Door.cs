using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public QuizManager quizManager; // Kéo QuizManager t? Inspector
    public GameObject player; // Kéo GameObject c?a nhân v?t vào ?ây
    public float interactionDistance = 2f; // Kho?ng cách t?i ?a ?? t??ng tác (??n v?)

    void Update()
    {
        if (quizManager == null || player == null) return;

        // Tính kho?ng cách gi?a c?a (GameObject này) và nhân v?t
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Ki?m tra n?u nhân v?t ?? g?n và nh?n E
        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            quizManager.ShowQuiz();
        }
    }
}