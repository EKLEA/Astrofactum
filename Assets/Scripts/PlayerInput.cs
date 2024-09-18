using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInput: MonoBehaviour
{
	public float rotationSpeed = 100.0f;
	public float moveSpeed = 100f;
	public float zoomSpeed = 100f;
	[Range(20,100)]public float maxDistance= 100f;
	[Range(10,90)]public float minDistance= 10f;
	public float maxAngleCam;
	public Camera cam;
	public GameObject camObj;
	
	
	float distanceToPlayer
	{
		get{return _disToPlayer;}
		set
		{
			if((value>=minDistance)&&(value<=maxDistance))
			{
				_disToPlayer=value;
			}
		}
	}
	float _disToPlayer;
	float cameraPosZ
	{
		get
		{
			return _camPosZ;
		}
		set
		{
			
			float minZ=-distanceToPlayer*math.cos(math.radians(maxAngleCam));
			if(value>minZ) _camPosZ=minZ;
			if((math.sqrt(distanceToPlayer*distanceToPlayer-value*value) >= GetGroundHeight()+2)&&(value<minZ)) _camPosZ=value;
		}
	}
	float _camPosZ;
	private bool isRotating = false;

	void Start()
	{
		_disToPlayer=100;
		camObj.transform.localPosition = new Vector3(0,70.7f,-70.7f);
	}
	void Update()
	{
		CameraRotation();
		CameraMove();
		cam.transform.LookAt(transform.position);
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
			transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);
			cameraPosZ += mouseY * rotationSpeed * Time.deltaTime;
			camObj.transform.localPosition=new Vector3(0,math.sqrt(distanceToPlayer*distanceToPlayer-cameraPosZ*cameraPosZ),cameraPosZ);
		}
	}


	void CameraMove()
	{
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		if (moveX != 0 || moveZ != 0)
		{
			Vector3 localDirection = new Vector3(moveX, 0, moveZ);

			Vector3 worldDirection = transform.TransformDirection(localDirection);

			transform.position += worldDirection * Time.deltaTime * moveSpeed;
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		distanceToPlayer -= scroll * zoomSpeed;
		
	}
	float GetGroundHeight()
	{
		RaycastHit hit;
		if (Physics.Raycast(cam.transform.position, Vector3.down, out hit))
		{
			return hit.point.y;
		}
		return 20; 
	}
}
