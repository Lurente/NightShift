using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float speed = 7.0f;
    public float jumpForce = 5.0f;
    public float dash = 5.0f;
    private float dashCooldown = 0.5f;
    public float stunVelocityThreshold = 5.0f;
    private float lastDashTime;
    private float horizontalInput;
    private float forwardInput;
    private Rigidbody playerRb;
    public bool isStun = false;
    public bool isOnGround = true;
    private bool isDashing = false;
    private bool isMovingTooFast = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        if (!isStun) // Only move if not stunned
        {
            // Calculate the desired movement direction based on input
            Vector3 movementDirection = new Vector3(horizontalInput, 0, forwardInput).normalized;

            // Check if there is significant input
            if (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(forwardInput) > 0.01f)
            {
                // Move at fixed speed in the direction of the input
                playerRb.velocity = movementDirection * speed + new Vector3(0, playerRb.velocity.y, 0);
                transform.rotation = Quaternion.LookRotation(movementDirection);
            }
            else
            {
                // Stop horizontal and forward movement, but preserve vertical velocity for gravity
                playerRb.velocity = new Vector3(0, playerRb.velocity.y, 0);
            }
        }
    }

    void Update()
    {
        if (isStun) { return; } // Don't process inputs if stunned

        // Get player inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        forwardInput = Input.GetAxisRaw("Vertical");

        // Handle dash
        if (Input.GetKeyDown(KeyCode.X) && Time.time > lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash());
            lastDashTime = Time.time;
        }

        // Handle jump
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // wall collison for stunning

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log(playerRb.velocity.magnitude);
        }
        if (collision.gameObject.CompareTag("Wall") && (isDashing || isMovingTooFast))
        {
            isStun = true;
            StartCoroutine(StunDuration());
        }

        //gound collision for jumping
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }
    IEnumerator PerformDash()
    {
        float originalSpeed = speed; // Store the original speed
        speed += dash; // Increase the speed by the dash value
        isDashing = true;

        yield return new WaitForSeconds(0.2f); // Dash duration

        speed = originalSpeed; // Revert to original speed
        isDashing = false;
    }
    IEnumerator StunDuration()
    {
        MakePlayerFall();
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        isStun = false; // Reset the stun state
        MakePlayerStandUp();
    }
    void MakePlayerFall()
    {
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        playerRb.AddForce(-transform.up * 0.1f, ForceMode.VelocityChange);
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void MakePlayerStandUp()
    {
        // Reset the rotation to stand the capsule upright
        transform.rotation = Quaternion.Euler(0, 0, 0);
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
}