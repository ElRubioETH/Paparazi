using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;

    void Start()
    {
        // Tự chạy timeline khi start
        // PlayTimeline();
    }

    public void PlayTimeline()
    {
        if (director != null)
        {
            director.Play();
        }
    }
}
