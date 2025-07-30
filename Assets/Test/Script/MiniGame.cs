using UnityEngine;

public class MiniGameRotation : MonoBehaviour
{
    private float angle = 0f;

    void Update()
    {
        // Xoay đoạn bằng phím mũi tên trong mini game
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle -= 2f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            angle += 2f * Time.deltaTime;
        }

        // Áp dụng xoay
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }
}