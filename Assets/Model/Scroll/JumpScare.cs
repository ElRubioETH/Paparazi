using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class InteractSequence : MonoBehaviour
{
    public TMP_Text interactText;
    public GameObject panelUI;
    public GameObject targetObject;
    public Transform lookAtTarget;
    public Camera playerCamera;
    public Animator monsterAnimator;
    public MonoBehaviour playerController;
    public MonoBehaviour mouseLook;
    public GameObject finalPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip phase1Clip;
    public AudioClip phase2Clip;

    private bool inRange = false;
    private bool hasInteracted = false;

    void Start()
    {
        interactText.gameObject.SetActive(false);
        panelUI.SetActive(false);
        finalPanel.SetActive(false);
        targetObject.SetActive(false);
    }

    void Update()
    {
        if (inRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        hasInteracted = true;
        interactText.gameObject.SetActive(false);
        panelUI.SetActive(true);

        playerController.enabled = false;
        mouseLook.enabled = false;

        yield return new WaitForSeconds(3f);
        panelUI.SetActive(false);

        targetObject.SetActive(true);
        playerCamera.transform.LookAt(lookAtTarget);

        // Phase1
        monsterAnimator.SetTrigger("Phase1");
        PlayAudio(phase1Clip);
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo info = monsterAnimator.GetCurrentAnimatorStateInfo(0);
            return info.IsName("Phase1") && info.normalizedTime >= 1.0f;
        });
        StopAudioIfPlaying();

        // Phase2
        monsterAnimator.SetTrigger("Phase2");
        PlayAudio(phase2Clip);
        yield return new WaitForSeconds(1f); // chờ 1 giây sau khi trigger

        finalPanel.SetActive(true);
        StopAudioIfPlaying();

        //yield return new WaitForSeconds(1f); // đợi thêm 1 giây rồi chuyển cảnh
        //SceneManager.LoadScene("Stage3");
    }

    void PlayAudio(AudioClip clip)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = clip;
        audioSource.Play();
    }

    void StopAudioIfPlaying()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach") && !hasInteracted)
        {
            interactText.gameObject.SetActive(true);
            inRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            interactText.gameObject.SetActive(false);
            inRange = false;
        }
    }
}
