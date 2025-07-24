using UnityEngine;
using UnityEngine.InputSystem; // Chỉ cần nếu bạn dùng Input System mới

public class LockController : MonoBehaviour
{
    public DigitController[] digitControllers; // Kéo 6 cái DigitController vào đây
    public Animator doorAnimator;
    public GameObject Canvas;
    public string correctCode = "584721";
    public GameObject player;

    public void Summit()
    {
        // Nếu dùng hệ thống cũ:
      
            CheckPassword();
        

        // Nếu dùng Input System mới thì dùng PlayerInput system nhé
        // Bạn có thể gắn hàm CheckPassword vào Input Action trực tiếp
    }
    public void Exit()
    {
        Canvas.SetActive(false);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void CheckPassword()
    {
        string userInput = "";

        foreach (var digit in digitControllers)
        {
            userInput += digit.GetValue().ToString();
        }

        if (userInput == correctCode)
        {
            Debug.Log("Correct code entered: " + userInput);
            doorAnimator.SetTrigger("Open");
            Canvas.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Debug.Log("Wrong code: " + userInput);
        }
    }
}
