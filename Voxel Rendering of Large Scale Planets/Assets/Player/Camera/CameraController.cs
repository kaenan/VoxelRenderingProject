using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	//[SerializeField] GameObject camHolder;

	private float Xsensitivity;
	private float Ysensitivity;

	//public Transform orientation;

	float xRotate;
	float yRotate;

	int speed;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Xsensitivity = 250f;
		Ysensitivity = 250f;
	}

	// Update is called once per frame
	void Update()
    {
		//transform.position = camHolder.transform.position;
		CameraControl();
		MoveCamera();
    }

	private void CameraControl()
	{
		float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * Xsensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Ysensitivity;

		yRotate += mouseX;
		xRotate -= mouseY;
		xRotate = Mathf.Clamp(xRotate, -90f, 90f);

		transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
		//orientation.rotation = Quaternion.Euler(0, yRotate, 0);
	}

	private void MoveCamera()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = 20;
		} else
		{
			speed = 5;
		}

		if (Input.GetKey(KeyCode.W))
		{
			transform.position += (transform.forward * Time.deltaTime) * speed;
		} 
		
		if (Input.GetKey(KeyCode.S))
		{
			transform.position += (-transform.forward * Time.deltaTime) * speed;
		} 
		
		if (Input.GetKey(KeyCode.D))
		{
			transform.position += (transform.right * Time.deltaTime) * speed;
		} 
		
		if (Input.GetKey(KeyCode.A))
		{
			transform.position += (-transform.right * Time.deltaTime) * speed;
		} 


	}
}
