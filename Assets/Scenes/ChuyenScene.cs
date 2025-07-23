using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineEndSceneLoader : MonoBehaviour
{
    public PlayableDirector timeline;

    void Start()
    {
        if (timeline == null)
        {
            timeline = GetComponent<PlayableDirector>();
        }

        if (timeline != null)
        {
            timeline.stopped += OnTimelineFinished;
        }
        else
        {
            Debug.LogError("Timeline (PlayableDirector) is not assigned.");
        }
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        if (director == timeline)
        {
            SceneManager.LoadScene(2); // Load scene index 2
        }
    }
}
