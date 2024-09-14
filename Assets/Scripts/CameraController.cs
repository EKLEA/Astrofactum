using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float rotationSpeed = 100.0f;
	public float moveSpeed = 100f;
	public float smoothTime = 0.2f;
	public Camera cam;
	private Vector3 velocity = Vector3.zero;
	private Vector3 targetPosition;
	private float verticalRotation = 0f;

	private bool isRotating = false;

	void Start()
	{
		targetPosition = transform.position;
		Cursor.lockState=CursorLockMode.Locked;
	}

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

            // Вращение камеры вокруг оси Y (горизонтальное)
            transform.Rotate(Vector3.up, mouseX * rotationSpeed, Space.World);

            // Изменение вертикального угла наклона камеры (вверх-вниз)
            verticalRotation -= mouseY * rotationSpeed;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // Ограничиваем угол наклона

            // Применяем изменение вертикального угла наклона к камере
            cam.transform.localEulerAngles = new Vector3(verticalRotation, cam.transform.localEulerAngles.y, 0);
        }
	}

	void CameraMove()
	{
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		if (moveX != 0 || moveZ != 0)
		{
			// Определяем направление движения в плоскости XZ
			Vector3 direction = new Vector3(moveX, 0, moveZ);
			Vector3 moveDirection = transform.TransformDirection(direction);

			// Перемещение камеры
			targetPosition += moveDirection * moveSpeed * Time.deltaTime;
			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		}
	}
}
