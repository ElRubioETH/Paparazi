using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetSceneButton : MonoBehaviour
{
    [SerializeField] private Button resetButton; // Reference to the UI Button

    void Start()
    {
        // Ensure the button is assigned
        if (resetButton == null)
        {
            resetButton = GetComponent<Button>();
        }

        // Add listener to the button's onClick event
        resetButton.onClick.AddListener(ResetCurrentScene);
    }

    void ResetCurrentScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}