using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PaperInteraction : MonoBehaviour
{
    public GameObject paperUI;
    public GameObject monster;
    public float paperDisplayTime = 5f;
    public float monsterSpawnDistance = 2f;
    public float delayBeforeSceneChange = 5f;
    public Camera mainCamera;
    private bool isReading = false;

    void Start()
    {
        paperUI.SetActive(false);
        monster.SetActive(false);
    }

    void OnMouseDown()
    {
        if (isReading) return;

        isReading = true;
        paperUI.SetActive(true);

        StartCoroutine(JumpScareSequence());
    }

    IEnumerator JumpScareSequence()
    {
        // 1. Đọc giấy trong vài giây
        yield return new WaitForSeconds(paperDisplayTime);

        // 2. Ẩn UI giấy
        paperUI.SetActive(false);

        // 3. Quay camera 180 độ
        yield return StartCoroutine(RotateCamera());

        // 4. Spawn quái ngay trước mặt sau khi quay xong
        Vector3 spawnPos = mainCamera.transform.position + mainCamera.transform.forward * monsterSpawnDistance;
        monster.transform.position = spawnPos;
        monster.transform.LookAt(mainCamera.transform); // Quái nhìn vào camera
        monster.SetActive(true);

        // 5. Đợi vài giây rồi đổi scene
        yield return new WaitForSeconds(delayBeforeSceneChange);
        ChangeScene();
    }

    IEnumerator RotateCamera()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        Quaternion startRot = mainCamera.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 180, 0); // Quay 180 độ theo trục Y

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
        SceneManager.LoadScene("TênSceneMới"); // 👉 Đổi thành tên thật của scene
    }
}
