using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLadderMovement : MonoBehaviour
{
    public float climbSpeed = 3f;
    public bool isOnLadder = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isOnLadder)
        {
            rb.useGravity = false;

            Vector3 climbDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.Space))
            {
                climbDirection = Vector3.up;
            }
            else if (Input.GetKey(KeyCode.C))
            {
                climbDirection = Vector3.down;
            }

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbDirection.y * climbSpeed, rb.linearVelocity.z);
        }
        else
        {
            rb.useGravity = true;
        }
    }
}
