using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject planet;
    public float gravity = 1;

    public Transform cameraTransform;
    public Vector2 lookAngleMinMax = new Vector2(-75, 80);
    public float mouseSensitivityX = 400f;
    public float mouseSensitivityY = 400f;
    private float verticalLookRotation;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Camera.main.transform.SetParent(transform.GetChild(0), false);
        Camera.main.transform.position = transform.GetChild(0).position;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        Jump();

        Vector3 gravityUp = (rb.position - planet.transform.position).normalized;
        Vector3 localUp = rb.rotation * Vector3.up;
        rb.rotation = Quaternion.FromToRotation(localUp, gravityUp) * rb.rotation;
        rb.AddForce(-gravityUp * gravity);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, lookAngleMinMax.x, lookAngleMinMax.y);
        cameraTransform.localEulerAngles = (Vector3.left * verticalLookRotation);

    }

    private void PlayerMove()
    {
        Input.GetAxisRaw("Horizontal");
        Input.GetAxisRaw("Vertical");

        //if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        //{
        //    if (Input.GetKey(KeyCode.D))
        //    {
        //        rb.velocity = transform.right * 10f;
        //    } else if (Input.GetKey(KeyCode.A))
        //    {
        //        rb.velocity = -transform.right * 10f;
        //    }
        //} else
        //    rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);

        //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        //{
        //    if(Input.GetKey(KeyCode.W))
        //    {
        //        rb.velocity = transform.forward * 10f;
        //    } else if (Input.GetKey(KeyCode.S))
        //    {
        //        rb.velocity = -transform.forward * 10f;
        //    }
        //} else
        //    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = transform.up * 10f;
        }
    }
}
