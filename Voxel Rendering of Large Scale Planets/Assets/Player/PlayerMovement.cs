using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public int speed = 10;

    [Header("Terraforming Settings")]
    public bool allowTerraforming = false;
    public int size = 2;
    public float weight = 0.5f;

    private Planet planetScript;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        planetScript = planet.GetComponent<Planet>();
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
        GravityToPlanet();
        TerraForming();
        Movement();

    }

    private void GravityToPlanet()
    {
        Vector3 gravityUp = (rb.position - planet.transform.position).normalized;
        Vector3 localUp = rb.rotation * Vector3.up;
        rb.rotation = Quaternion.FromToRotation(localUp, gravityUp) * rb.rotation;
        rb.AddForce(-gravityUp * gravity);

        Debug.DrawRay(rb.position, localUp, Color.yellow);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, lookAngleMinMax.x, lookAngleMinMax.y);
        cameraTransform.localEulerAngles = (Vector3.left * verticalLookRotation);

        Jump(gravityUp);
    }

    private void Movement()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rb.MovePosition(rb.position + transform.TransformDirection(direction) * Time.deltaTime * speed);
    }

    private void Jump(Vector3 localUp)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(localUp * 1000);
        }
    }

    private void TerraForming()
    {
        if (allowTerraforming)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            {
                float weightChange = weight;

                if (Input.GetKey(KeyCode.Mouse1))
                {
                    weightChange = -weight;
                }

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit, 100))
                {
                    Vector3Int hitPoint = new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
                    Vector3Int[] chunkIDs = new Vector3Int[7];
                    chunkIDs[0] = (hitPoint / 50) * 50;
                    chunkIDs[1] = new Vector3Int(chunkIDs[0].x + 50, chunkIDs[0].y, chunkIDs[0].z);
                    chunkIDs[2] = new Vector3Int(chunkIDs[0].x - 50, chunkIDs[0].y, chunkIDs[0].z);
                    chunkIDs[3] = new Vector3Int(chunkIDs[0].x, chunkIDs[0].y + 50, chunkIDs[0].z);
                    chunkIDs[4] = new Vector3Int(chunkIDs[0].x, chunkIDs[0].y - 50, chunkIDs[0].z);
                    chunkIDs[5] = new Vector3Int(chunkIDs[0].x, chunkIDs[0].y, chunkIDs[0].z + 50);
                    chunkIDs[6] = new Vector3Int(chunkIDs[0].x, chunkIDs[0].y, chunkIDs[0].z - 50);

                    Vector3Int startPoint = new Vector3Int(hitPoint.x - (size / 2), hitPoint.y - (size / 2), hitPoint.z - (size / 2));



                    planetScript.Terraform(startPoint, chunkIDs, size, weightChange);
                }
            }
        }
    }
}
