using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
	private float Xsensitivity;
	private float Ysensitivity;

	public GameObject playerGO;
	public Transform cameraHolder;

	private Rigidbody rb;

	float xRotate;
	float yRotate;

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		rb = playerGO.GetComponent<Rigidbody>();

		Xsensitivity = 400;
		Ysensitivity = 400;
    }

    // Update is called once per frame
    void Update()
    {
		//float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * Xsensitivity;
		//float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Ysensitivity;

		//yRotate += mouseX;
		//xRotate -= mouseY;
		//xRotate = Mathf.Clamp(xRotate, -90f, 90f);

		//transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
		//rb.rotation = Quaternion.Euler(rb.rotation.x, yRotate, rb.rotation.z);
		transform.position = cameraHolder.transform.position;
		//transform.rotation = Quaternion.Euler()
	}
}
