using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float turnSpeed = 100.0f;
    private float horizontalInput;
    private float forwardInput;
    public float speed = 20.0f;
    public float xRange = 25.0f;
    public float xRangeLeft = 3.3f;
    public float xRangeRight = -22.2f;
    public float zRangeForward = 18.83f;
    public float zRangeBack = -1.2f;

    void Start()
    {
    }

    void Update()
    {
        if (transform.position.z > zRangeForward)

        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRangeForward);
        }

        if (transform.position.z < zRangeBack)

        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zRangeBack);
        }


        //    if (transform.position.x < -xRange)
        if (transform.position.x < xRangeRight)

        {
            transform.position = new Vector3(xRangeRight, transform.position.y, transform.position.z);
        }

        //if (transform.position.x > xRange)
        if (transform.position.x > xRangeLeft)
        {
            //transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
            transform.position = new Vector3(xRangeLeft, transform.position.y, transform.position.z);
        }

        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * horizontalInput);

       // horizontalInput = Input.GetAxis("Horizontal");
      //  transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);

    }
}
