using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    private SpawnManager spawnManager;

    // Movement parameters
    private const float Speed = 15.0f;
    private const float DownwardForce = 0.2f;

    // Input & settings
    public bool isMultiplayer = false;
    public int playerNumber = 1;

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
            ApplyMovement();
            ApplyDownwardForce();
        }
    }

    private void ApplyMovement()
    {
        float verticalAxis = 0f;
        float horizontalAxis = 0f;

        if (!isMultiplayer)
        {
            // Single player uses Unity's input axis (WASD and Arrow keys)
            verticalAxis = Input.GetAxis("Vertical");
            horizontalAxis = Input.GetAxis("Horizontal");
        }
        else
        {
            // Multiplayer manual key handling
            if (playerNumber == 1)
            {
                // WASD controls
                verticalAxis += Input.GetKey(KeyCode.W) ? 1 : 0;
                verticalAxis -= Input.GetKey(KeyCode.S) ? 1 : 0;
                horizontalAxis -= Input.GetKey(KeyCode.A) ? 1 : 0;
                horizontalAxis += Input.GetKey(KeyCode.D) ? 1 : 0;
            }
            else if (playerNumber == 2)
            {
                // Arrow key controls
                verticalAxis += Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
                verticalAxis -= Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
                horizontalAxis -= Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
                horizontalAxis += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            }
        }

        Vector3 movement = this.transform.forward * verticalAxis - this.transform.right * horizontalAxis;
        movement.Normalize();
        this.transform.position += movement * Speed * Time.deltaTime;


        if (anim != null)
        {
            anim.SetFloat("vertical", verticalAxis);
            anim.SetFloat("horizontal", horizontalAxis);
        }
    }

    private void ApplyDownwardForce()
    {
        this.rb.AddForce(Vector3.down * DownwardForce * Time.deltaTime, ForceMode.Impulse);
    }
}
