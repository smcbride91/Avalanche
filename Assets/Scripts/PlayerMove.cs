using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    private SpawnManager spawnManager;

    // Movement parameters
    private const float Speed = 20.0f;
    private const float DownwardForce = 0.2f;

    private float horizontalInput;
    private float forwardInput;

    private void Start()
    {
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        rb = GetComponent<Rigidbody>();

        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager not found. Ensure there is a GameObject named 'SpawnManager' with the SpawnManager script attached.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody not found. Ensure this GameObject has a Rigidbody component.");
        }
    }

    private void Update()
    {
        if (spawnManager != null && spawnManager.isGameActive)
        {
            HandleInput();
            ApplyMovement();
            ApplyDownwardForce();
        }
    }

    private void HandleInput()
    {
        // Get horizontal and vertical movement input
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
    }

    private void ApplyMovement()
    {
        // Calculate movement direction
        Vector3 movement = Vector3.zero;

        // Moving forward or backward
        if (forwardInput != 0)
        {
            movement += transform.forward * forwardInput;
        }

        // Strafing left or right
        if (horizontalInput != 0)
        {
            movement -= transform.right * horizontalInput;
        }

        // Apply movement
        transform.position += movement * Speed * Time.deltaTime;

        // Update animation parameters based on movement input
        if (Mathf.Abs(forwardInput) > 0.1f || Mathf.Abs(horizontalInput) > 0.1f)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // Determine animation based on movement direction
        if (forwardInput > 0)
        {
            anim.SetBool("isRunningForward", true);
            anim.SetBool("isRunningBackward", false);
            anim.SetBool("isStrafingLeft", false);
            anim.SetBool("isStrafingRight", false);
        }
        else if (forwardInput < 0)
        {
            anim.SetBool("isRunningForward", false);
            anim.SetBool("isRunningBackward", true);
            anim.SetBool("isStrafingLeft", false);
            anim.SetBool("isStrafingRight", false);
        }
        else if (horizontalInput > 0)
        {
            anim.SetBool("isRunningForward", false);
            anim.SetBool("isRunningBackward", false);
            anim.SetBool("isStrafingLeft", false);
            anim.SetBool("isStrafingRight", true);
        }
        else if (horizontalInput < 0)
        {
            anim.SetBool("isRunningForward", false);
            anim.SetBool("isRunningBackward", false);
            anim.SetBool("isStrafingLeft", true);
            anim.SetBool("isStrafingRight", false);
        }
    }

    private void ApplyDownwardForce()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.down * DownwardForce * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
