using UnityEngine;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    private SpawnManager spawnManager;

    private const float Speed = 15.0f;
    private const float DownwardForce = 0.2f;

    public bool isLocalMultiplayer = false;
    public bool isNetworkMultiplayer = false;
    public int playerNumber = 1;

    private Vector3 serverInputDirection = Vector3.zero;

    private NetworkVariable<float> networkVertical = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<float> networkHorizontal = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Server);

    private void Start()
    {
      spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        rb = GetComponent<Rigidbody>();


        if (spawnManager == null)
            Debug.LogError("SpawnManager not found.");
        if (rb == null)
            Debug.LogError("Rigidbody not found.");

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            isNetworkMultiplayer = true;

        if (spawnManager.isNetworkMultiplayer && IsOwner && !IsHost)
        {
            playerNumber = 2;
            SetPlayerNumberServerRpc(playerNumber);
        }

    }

    private void Update()
    {
        if (spawnManager == null || !spawnManager.isGameActive)
            return;

        if (isLocalMultiplayer || (!isLocalMultiplayer && !isNetworkMultiplayer))
        {
            ApplyLocalMovement();
            ApplyDownwardForce();
        }
        else if (isNetworkMultiplayer && IsOwner)
        {
            HandleNetworkedInput();
            ApplyDownwardForce();
        }

        if (isNetworkMultiplayer && IsServer)
        {
            ApplyNetworkedMovement();
        }

        UpdateAnimations();
    }

    private void ApplyLocalMovement()
    {
        float verticalAxis = 0f;
        float horizontalAxis = 0f;

        if (!isLocalMultiplayer && !isNetworkMultiplayer)
        {
            verticalAxis = Input.GetAxis("Vertical");
            horizontalAxis = Input.GetAxis("Horizontal");
        }
        else
        {
            if (playerNumber == 1)
            {
                verticalAxis += Input.GetKey(KeyCode.W) ? 1 : 0;
                verticalAxis -= Input.GetKey(KeyCode.S) ? 1 : 0;
                horizontalAxis -= Input.GetKey(KeyCode.A) ? 1 : 0;
                horizontalAxis += Input.GetKey(KeyCode.D) ? 1 : 0;
            }
            else if (playerNumber == 2)
            {
                verticalAxis += Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
                verticalAxis -= Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
                horizontalAxis -= Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
                horizontalAxis += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            }
        }

        Vector3 movement = transform.forward * verticalAxis - transform.right * horizontalAxis;
        movement.Normalize();
        transform.position += movement * Speed * Time.deltaTime;
    }

    private void HandleNetworkedInput()
    {
        float verticalAxis = 0f;
        float horizontalAxis = 0f;

        // Combine input from both WASD and arrow keys
        verticalAxis += Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        verticalAxis -= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
        horizontalAxis -= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
        horizontalAxis += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0;

        Vector3 inputDirection = transform.forward * verticalAxis - transform.right * horizontalAxis;
        inputDirection.Normalize();

        // Update local animation immediately
        if (anim != null)
        {
            anim.SetFloat("vertical", verticalAxis);
            anim.SetFloat("horizontal", horizontalAxis);
        }

        // Inform the server
        SubmitInputServerRpc(inputDirection, verticalAxis, horizontalAxis);
    }

    private void ApplyNetworkedMovement()
    {
        transform.position += serverInputDirection * Speed * Time.deltaTime;
    }

    private void ApplyDownwardForce()
    {
        rb?.AddForce(Vector3.down * DownwardForce * Time.deltaTime, ForceMode.Impulse);
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;

        if (!isNetworkMultiplayer)
        {
            float verticalAxis = 0f;
            float horizontalAxis = 0f;

            if (isLocalMultiplayer)
            {
                if (playerNumber == 1)
                {
                    verticalAxis += Input.GetKey(KeyCode.W) ? 1 : 0;
                    verticalAxis -= Input.GetKey(KeyCode.S) ? 1 : 0;
                    horizontalAxis -= Input.GetKey(KeyCode.A) ? 1 : 0;
                    horizontalAxis += Input.GetKey(KeyCode.D) ? 1 : 0;
                }
                else if (playerNumber == 2)
                {
                    verticalAxis += Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
                    verticalAxis -= Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
                    horizontalAxis -= Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
                    horizontalAxis += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
                }
            }
            else
            {
                verticalAxis = Input.GetAxis("Vertical");
                horizontalAxis = Input.GetAxis("Horizontal");
            }

            anim.SetFloat("vertical", verticalAxis);
            anim.SetFloat("horizontal", horizontalAxis);
        }
        else
        {
            if (!IsOwner)
            {
                //  For non-owned objects (other players), use networked values
                anim.SetFloat("vertical", networkVertical.Value);
                anim.SetFloat("horizontal", networkHorizontal.Value);
            }
            //  NOTE: Owner's animation was already updated immediately in HandleNetworkedInput()
        }
    }

    [ServerRpc]
    private void SubmitInputServerRpc(Vector3 inputDirection, float verticalAxis, float horizontalAxis)
    {
        serverInputDirection = inputDirection;
        networkVertical.Value = verticalAxis;
        networkHorizontal.Value = horizontalAxis;
    }

    [ServerRpc]
    private void SetPlayerNumberServerRpc(int number)
    {
        playerNumber = number;
    }

    [ClientRpc]
    private void SetPlayerNumberClientRpc(int number)
    {
        playerNumber = number;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
            if (spawnManager == null)
            {
                Debug.LogError("SpawnManager not found in OnNetworkSpawn.");
                return;
            }

            // Determine the player number based on server logic
            int assignedPlayerNumber = spawnManager.RegisterPlayer(OwnerClientId); // RegisterPlayer should return the player number
            playerNumber = assignedPlayerNumber;
            // Optionally broadcast the player number
            SetPlayerNumberClientRpc(playerNumber);
        }
    }


}
