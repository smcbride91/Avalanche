using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerMoveMP : NetworkBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    private SpawnMP spawnMP;

    // Movement parameters
    private const float Speed = 50.0f;
    private const float DownwardForce = 0.2f;

    private float horizontalInput;
    private float forwardInput;

    private void Start()
    {
        spawnMP = GameObject.Find("SpawnManager").GetComponent<SpawnMP>();

        rb = GetComponent<Rigidbody>();

        if (spawnMP == null)
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
        if (spawnMP != null && spawnMP.isGameActive && spawnMP.playerDeath == false)
        {
            ApplyMovement();
            ApplyDownwardForce();
        }
    }


    private void ApplyMovement()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
        Vector3 movement = this.transform.forward * verticalAxis - this.transform.right *
        horizontalAxis;
        movement.Normalize();
        this.transform.position += movement * 0.05f;
        this.anim.SetFloat("vertical", verticalAxis);
        this.anim.SetFloat("horizontal", horizontalAxis);
    }

    private void ApplyDownwardForce()
    {
        this.rb.AddForce(Vector3.down * 1 * Time.deltaTime, ForceMode.Impulse);
    }
}
