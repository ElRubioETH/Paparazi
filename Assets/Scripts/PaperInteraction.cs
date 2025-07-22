using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PaperInteraction : MonoBehaviour
{
    [Header("UI & Monster")]
    public GameObject paperUI;
    public GameObject monster;

    [Header("Cài đặt thời gian")]
    public float paperDisplayTime = 5f;
    public float monsterSpawnDistance = 2f;
    public float delayBeforeSceneChange = 5f;

    [Header("Camera và Điều khiển")]
    public Camera mainCamera;
    public MonoBehaviour mouseLookScript; // Gắn script điều khiển góc nhìn ở đây (vd: MouseLook hoặc FPC)

    [Header("Tên Scene muốn chuyển tới")]
    public string targetSceneName = "TênSceneMới";

    private bool isReading = false;

    void Start()
    {
        if (paperUI != null) paperUI.SetActive(false);
        if (monster != null) monster.SetActive(false);
    }

    void OnMouseDown()
    {
        if (isReading) return;

        isReading = true;
        if (paperUI != null) paperUI.SetActive(true);

        StartCoroutine(JumpScareSequence());
    }

    IEnumerator JumpScareSequence()
    {
        // 1. Đọc giấy
        yield return new WaitForSeconds(paperDisplayTime);

        if (paperUI != null) paperUI.SetActive(false);

        // 2. Tắt điều khiển góc nhìn
        if (mouseLookScript != null)
            mouseLookScript.enabled = false;

        // 3. Xoay camera 180 độ
        yield return StartCoroutine(RotateCamera());

        // 4. Bật lại điều khiển góc nhìn
        if (mouseLookScript != null)
            mouseLookScript.enabled = true;

        // 5. Triệu hồi quái vật phía trước mặt
        if (monster != null && mainCamera != null)
        {
            Vector3 spawnPos = mainCamera.transform.position + mainCamera.transform.forward * monsterSpawnDistance;
            monster.transform.position = spawnPos;
            monster.transform.LookAt(mainCamera.transform); // Quái nhìn vào camera
            monster.SetActive(true);
        }

        // 6. Đợi rồi chuyển scene
        yield return new WaitForSeconds(delayBeforeSceneChange);
        ChangeScene();
    }

    IEnumerator RotateCamera()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        Quaternion startRot = mainCamera.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 180, 0); // Quay 180 độ quanh trục Y

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            yield return null;
        }

        mainCamera.transform.rotation = endRot;
    }

    void ChangeScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
            SceneManager.LoadScene(targetSceneName);
        else
            Debug.LogWarning("Bạn chưa đặt tên scene để chuyển!");
    }
}
