using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    public GameObject door; // Tham chi?u ??n cánh c?a
    public float openAngle = 90f; // Góc m? c?a
    public float rotationSpeed = 5f; // T?c ?? xoay c?a
    private bool isOpen = false; // Tr?ng thái c?a
    private Quaternion closedRotation; // Rotation khi ?óng
    private Quaternion openRotation; // Rotation khi m?
    private bool isRotating = false; // ?ang xoay hay không

    void Start()
    {
        if (door == null)
        {
            enabled = false;
            return;
        }

        closedRotation = door.transform.rotation;
        openRotation = Quaternion.Euler(door.transform.eulerAngles + new Vector3(0, openAngle, 0));
        int obstaclesLayer = LayerMask.NameToLayer("Obstacles");
        if (obstaclesLayer != -1)
        {
            door.gameObject.layer = obstaclesLayer;
        }
        Collider doorCollider = door.GetComponent<Collider>();
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isRotating && door != null)
        {
            isOpen = !isOpen;
            isRotating = true;

            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = !isOpen;
            }

            int targetLayer = isOpen ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Obstacles");
            if (targetLayer != -1)
            {
                door.gameObject.layer = targetLayer;
            }
        }

        if (isRotating && door != null)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            door.transform.rotation = Quaternion.Slerp(door.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(door.transform.rotation, targetRotation) < 0.1f)
            {
                isRotating = false;
                door.transform.rotation = targetRotation;
            }
        }
    }
}