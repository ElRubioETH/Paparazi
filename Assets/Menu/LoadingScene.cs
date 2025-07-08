using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using MyGame.Scenes;

namespace MyGame.Scenes
{
    public enum GameScene
    {
        DataHolder,
        Menu
    }
}
public class LoadingScene : MonoBehaviour
{
    private AsyncOperation m_async;
    public UnityEvent<float> OnLoading;
    private void Start()
    {
        m_async = SceneManager.LoadSceneAsync(GameScene.DataHolder.ToString(), LoadSceneMode.Additive);
    }
    private void Update()
    {
        OnLoading?.Invoke(m_async.progress);
        if (m_async.isDone)
        {
            SceneManager.LoadScene(GameScene.Menu.ToString());
        }
    }
}
