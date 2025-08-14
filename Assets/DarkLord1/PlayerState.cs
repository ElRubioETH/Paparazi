using UnityEngine;

public class PlayerStateTracker : MonoBehaviour
{
    public bool isMoving { get; private set; }
    public bool isCrouching { get; private set; }

    private Vector3 lastPosition;
    private FirstPersonController controller;

    void Start()
    {
        controller = GetComponent<FirstPersonController>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (controller == null) return;

        // L?y tr?ng thái crouch t? script chính
        isCrouching = (bool)typeof(FirstPersonController)
            .GetField("isCrouched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(controller);

        // Tính toán di chuy?n
        isMoving = Vector3.Distance(transform.position, lastPosition) > 0.01f;
        lastPosition = transform.position;
    }
}
