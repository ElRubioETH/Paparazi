using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TimelineEndLoad : MonoBehaviour
{
    public PlayableDirector timeline;

    void Start()
    {
        if (timeline != null)
        {
            timeline.stopped += OnTimelineStopped;
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        if (director == timeline)
        {
            SceneManager.LoadScene("EndStage");
        }
    }

    void OnDestroy()
    {
        if (timeline != null)
        {
            timeline.stopped -= OnTimelineStopped;
        }
    }
}
