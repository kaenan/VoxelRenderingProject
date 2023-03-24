using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
	private float Xsensitivity;
	private float Ysensitivity;

	public Transform orientation;
	public Transform cameraHolder;

	float xRotate;
	float yRotate;

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		Xsensitivity = PlayerPrefs.GetFloat("MouseX");
		Ysensitivity = PlayerPrefs.GetFloat("MouseY");
    }

    // Update is called once per frame
    void Update()
    {
		float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * Xsensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Ysensitivity;

		yRotate += mouseX;
		xRotate -= mouseY;
		xRotate = Mathf.Clamp(xRotate, -90f, 90f);

		transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
		orientation.rotation = Quaternion.Euler(orientation.rotation.x, yRotate, orientation.rotation.z);
		transform.position = cameraHolder.transform.position;

	}
}
