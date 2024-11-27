using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerMoveMP : NetworkBehaviour
{
//    public Animator anim;
    private Rigidbody rb;
    private NetworkManagerUI networkManagerUI;
    private float turnSpeed = 30.0f;
    private float horizontalInput;
    private float forwardInput;
    private float speed = 20.0f;
    private float xRangeLeft = 23.58f;
    private float xRangeRight = -26.4f;
    private float zRangeForward = 33.17f;
    private float zRangeBack = 12.0f;
    private SpawnMP spawnMP;

    void Start()
    {
        spawnMP = GameObject.Find("SpawnManager").GetComponent<SpawnMP>();
        this.rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
       
       if (spawnMP.playerDeath == false)
        {
                    Move();
                    Still();
                    CheckRange();

                    horizontalInput = Input.GetAxis("Horizontal");
                    forwardInput = Input.GetAxis("Vertical");

                    transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
                    transform.Translate(Vector3.left * Time.deltaTime * turnSpeed * horizontalInput);
        }
        
    }

    private void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
        Vector3 movement = this.transform.forward * verticalAxis + this.transform.right * horizontalAxis;
        movement.Normalize();
        this.transform.position += movement * 0.01f;

    }
    private void Still()
    {
        this.rb.AddForce(Vector3.down * 1 * Time.deltaTime, ForceMode.Impulse);
    }

    private void CheckRange()
    {
        if (transform.position.z > zRangeForward)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRangeForward);
        }

        if (transform.position.z < zRangeBack)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRangeBack);
        }

        if (transform.position.x < xRangeRight)
        {
            transform.position = new Vector3(xRangeRight, transform.position.y, transform.position.z);
        }

        if (transform.position.x > xRangeLeft)
        {
            transform.position = new Vector3(xRangeLeft, transform.position.y, transform.position.z);
        }
    }
}
