using UnityEngine;
using System.Collections;

public class WardrobeHidingSystem : MonoBehaviour
{
    [Header("References")]
    public Animator wardrobeAnimator;
    public Animator playerAnimator;
    public Transform player;
    public Transform hidingSpot;
    public Transform enterExitPosition;
    public KeyCode interactKey = KeyCode.E;

    [Header("Settings")]
    public float interactionDistance = 2f;
    public float enterExitDuration = 1f;
    public float rotationSpeed = 5f; // Tốc độ xoay hướng về tủ

    private bool isHiding = false;
    private bool isAnimating = false;
    private bool isRotatingToWardrobe = false;
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;

    void Update()
    {
        if (isAnimating) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && Input.GetKeyDown(interactKey))
        {
            if (isHiding)
            {
                StartCoroutine(ExitWardrobe());
            }
            else
            {
                StartCoroutine(PrepareToEnterWardrobe());
            }
        }

        // Xử lý xoay player hướng về tủ
        if (isRotatingToWardrobe)
        {
            RotatePlayerTowardsWardrobe();
        }
    }

    IEnumerator PrepareToEnterWardrobe()
    {
        isAnimating = true;

        // Lưu vị trí và hướng ban đầu
        originalPlayerPosition = player.position;
        originalPlayerRotation = player.rotation;

        // Di chuyển player đến vị trí trước tủ
        player.position = enterExitPosition.position;

        // Bắt đầu quá trình xoay hướng về tủ
        isRotatingToWardrobe = true;

        // Tạm dừng để chờ xoay hoàn thành
        yield return new WaitUntil(() => !isRotatingToWardrobe);

        // Tiếp tục quy trình vào tủ
        StartCoroutine(EnterWardrobe());
    }

    void RotatePlayerTowardsWardrobe()
    {
        // Tính hướng nhìn về tủ
        Vector3 directionToWardrobe = (transform.position - player.position).normalized;
        directionToWardrobe.y = 0; // Giữ nguyên y để không nghiêng lên/xuống

        // Tính góc quay
        Quaternion targetRotation = Quaternion.LookRotation(directionToWardrobe);

        // Xoay mượt
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Kiểm tra nếu đã xoay đủ gần
        if (Quaternion.Angle(player.rotation, targetRotation) < 1f)
        {
            player.rotation = targetRotation;
            isRotatingToWardrobe = false;
        }
    }

    IEnumerator EnterWardrobe()
    {
        // Tắt điều khiển người chơi
        player.GetComponent<FirstPersonController>().enabled = false;

        // Mở cửa tủ
        wardrobeAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);

        // Bắt đầu animation chui vào tủ
        playerAnimator.SetTrigger("EnterWardrobe");

        // Đợi animation hoàn thành
        yield return new WaitForSeconds(enterExitDuration);

        // Di chuyển vào vị trí trốn
        player.position = hidingSpot.position;
        player.rotation = hidingSpot.rotation;

        // Đóng cửa tủ
        wardrobeAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);

        isHiding = true;
        isAnimating = false;
    }

    IEnumerator ExitWardrobe()
    {
        isAnimating = true;

        // Mở cửa tủ
        wardrobeAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);

        // Di chuyển player ra vị trí exit
        player.position = enterExitPosition.position;
        player.rotation = enterExitPosition.rotation;

        // Animation chui ra
        playerAnimator.SetTrigger("ExitWardrobe");
        yield return new WaitForSeconds(enterExitDuration);

        // Trở về vị trí và hướng ban đầu
        player.position = originalPlayerPosition;
        player.rotation = originalPlayerRotation;

        // Đóng cửa tủ
        wardrobeAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);

        // Bật lại điều khiển
        player.GetComponent<FirstPersonController>().enabled = true;

        isHiding = false;
        isAnimating = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (enterExitPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(enterExitPosition.position, 0.2f);
            Gizmos.DrawLine(enterExitPosition.position, transform.position);
        }
    }
}