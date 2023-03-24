using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject planet;
    public float gravity = 1;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();

        Vector3 gravityUp = (rb.position - planet.transform.position).normalized;
        Vector3 localUp = rb.rotation * Vector3.up;
        rb.rotation = Quaternion.FromToRotation(localUp, gravityUp) * rb.rotation;
        rb.AddForce(-gravityUp * gravity);
    }

    private void PlayerMove()
    {
        float x;
        float z;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector3(10f, rb.velocity.y, rb.velocity.z);
            } else if (Input.GetKey(KeyCode.A))
            {
                rb.velocity = new Vector3(-10f, rb.velocity.y, rb.velocity.z);
            }
        } else
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if(Input.GetKey(KeyCode.W))
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 10f);
            } else if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -10f);
            }
        } else
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
    }
}
