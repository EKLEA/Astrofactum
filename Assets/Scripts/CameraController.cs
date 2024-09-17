using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float rotationSpeed = 100.0f;
	public float moveSpeed = 100f;
	public float smoothTime = 0.2f;
	public Camera cam;
	private float verticalRotation = 0f;
	private bool isRotating = false;

	void Update()
	{
		CameraRotation();
		CameraMove();
	}

	void CameraRotation()
	{
		if (Input.GetMouseButtonDown(1))
		{
			isRotating = true;
		}
		if (Input.GetMouseButtonUp(1))
		{
			isRotating = false;
		}
		if (isRotating)
		{
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			transform.Rotate(Vector3.up, mouseX * rotationSpeed, Space.World);

			verticalRotation -= mouseY * rotationSpeed;
			verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); 

			cam.transform.localEulerAngles = new Vector3(verticalRotation, cam.transform.localEulerAngles.y, 0);
		}
	}

	void CameraMove()
	{
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		if (moveX != 0 || moveZ != 0)
		{
			Vector3 direction = new Vector3(moveX, 0, moveZ);
			transform.position+=direction*Time.deltaTime*moveSpeed;
		}
	}
}
