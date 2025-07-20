using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    private float mouseX, mouseY;
    public float mouseSensitivity = 100f;
    public float interactionDistance = 3f;
    private bool isCanvasActive = false;
    public Canvas bookCanvas; 
    public QuizManager quizManager; // Tham chi?u ??n QuizManager

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (bookCanvas != null)
            bookCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // Di chuy?n Player
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Xoay Camera
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
        cameraTransform.localRotation = Quaternion.Euler(mouseY, 0, 0);

        // T??ng tác v?i sách ho?c c?a
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (Camera.main == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Book"))
            {
                isCanvasActive = !isCanvasActive;
                if (bookCanvas != null)
                {
                    bookCanvas.gameObject.SetActive(isCanvasActive);
                    Cursor.lockState = CursorLockMode.Locked;

                    if (isCanvasActive)
                    {
                        Book book = hit.collider.GetComponent<Book>();
                        if (book != null)
                        {
                            book.DisplayContent();
                        }
                    }
                }
            }
            else if (hit.collider.CompareTag("Door"))
            {
                if (quizManager != null)
                {
                    if (quizManager.failPanel.activeSelf)
                    {
                        quizManager.CloseFailPanel();
                    }
                    else
                    {
                        quizManager.ShowQuiz();
                    }
                }
            }
        }
    }
}