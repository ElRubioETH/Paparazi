using UnityEngine;
using UnityEngine.UI;

public class MiniGameController : MonoBehaviour
{
    public Slider slider;                  // Gắn Slider UI
    public float speed = 1.5f;             // Tốc độ slider
    private bool goingUp = true;           // Tăng hay giảm

    public GameObject handle;              // Gắn Handle image có Collider2D
    public GameObject successZone;         // Vùng thành công (có Collider2D)

    public FixCar fixCar;                  // Đối tượng FixCar (để gọi hàm success/fail)

    private bool isActive = true;

    void Update()
    {
        if (!isActive) return;

        MoveSlider();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsHandleInsideSuccessZone())
            {
                // Thành công
                isActive = false;
                gameObject.SetActive(false);
                fixCar.OnMiniGameSuccess();
            }
            else
            {
                // Thất bại
                isActive = false;
                fixCar.OnMiniGameFail();
            }
        }
    }

    void MoveSlider()
    {
        if (goingUp)
        {
            slider.value += speed * Time.deltaTime;
            if (slider.value >= 1f)
            {
                slider.value = 1f;
                goingUp = false;
            }
        }
        else
        {
            slider.value -= speed * Time.deltaTime;
            if (slider.value <= 0f)
            {
                slider.value = 0f;
                goingUp = true;
            }
        }
    }

    bool IsHandleInsideSuccessZone()
    {
        Collider handleCol = handle.GetComponent<Collider>();
        Collider zoneCol = successZone.GetComponent<Collider>();

        if (handleCol != null && zoneCol != null)
        {
            return handleCol.bounds.Intersects(zoneCol.bounds);
        }
        return false;
    }
}
