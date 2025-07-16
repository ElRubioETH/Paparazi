using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Tên scene muốn load")]
    public string sceneToLoad = "MainMenu";

    // Gọi hàm này từ nút "Reload"
    public void ReloadCurrentScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
        Debug.Log("🔁 Reload scene: " + current.name);
    }

    // Gọi hàm này từ nút "Load scene"
    public void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            Debug.Log("➡️ Load scene: " + sceneToLoad);
        }
        else
        {
            Debug.LogWarning("❌ sceneToLoad trống!");
        }
    }
}
