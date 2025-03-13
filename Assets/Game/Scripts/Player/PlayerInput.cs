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
	public GameObject point;
	//public TerrainGeneretion gameWorld;//переделать
	
	float _disToPlayer;
	bool isRotating = false;
	bool isMoving= true;
	float _curAn;	
	float distanceToPlayer
	{
		get{return _disToPlayer;}
		set
		{
			if(value==_disToPlayer) return;
			else 
			{
				if((value>=minDistance)&&(value<=maxDistance))		_disToPlayer=value;
				else if(value<minDistance)		_disToPlayer=minDistance;
				else if(value>maxDistance)		_disToPlayer=maxDistance;
			}
			camObj.transform.localPosition=new Vector3(0,math.sin(currentAngle*Mathf.Deg2Rad)*_disToPlayer,-math.cos(currentAngle*Mathf.Deg2Rad)*_disToPlayer);
		}
	}

	float currentAngle
	{
		get{ return _curAn;}
		set
		{
			var worldPos=camObj.transform.parent.TransformPoint(new Vector3(0,math.sin(value*Mathf.Deg2Rad)*distanceToPlayer,-math.cos(value*Mathf.Deg2Rad)*distanceToPlayer));
			if((GetGroundHeight(worldPos)+0.01f <= worldPos.y)&&value<=maxAngleCam&&(value>=0)) _curAn=value;
			else if(value>maxAngleCam) _curAn=maxAngleCam;
			else if(value<0) _curAn=0;
		}
	}

	void Start()
	{
		_disToPlayer=100;
		currentAngle=50;
	}
	void Update()
	{
		CameraRotation();
		CameraMove();
		camObj.transform.localPosition=new Vector3(0,math.sin(currentAngle*Mathf.Deg2Rad)*distanceToPlayer,-math.cos(currentAngle*Mathf.Deg2Rad)*distanceToPlayer);
		camObj.transform.position=new Vector3(camObj.transform.position.x,checkY(camObj.transform.position.y),camObj.transform.position.z);
		cam.transform.LookAt(transform.position);
		if(Input.GetButtonDown("Fire1"))  EditWorldController.Instance.LeftClick();
		if(Input.GetButtonDown("Fire2"))  EditWorldController.Instance.RightClick();
		
	}
	void CameraRotation()
	{
		if (Input.GetButtonDown("RotationCam"))
		{
			isRotating = true;
			isMoving=false;
		}
		if (Input.GetButtonUp("RotationCam"))
		{
			isRotating = false;
			isMoving = true;
		}
		if (isRotating)
		{
			
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");
			transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);
			currentAngle += mouseY * rotationSpeed * Time.deltaTime;
		}
	}
	float checkY(float y)
	{
		float terrHeight=GetGroundHeight(cam.transform.position);
		if(y<terrHeight)  return terrHeight+1f;
		else return y;
	}
	void CameraMove()
	{
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		if ((moveX != 0 || moveZ != 0 )&&isMoving)
		{
			Vector3 localDirection = new Vector3(moveX, 0, moveZ);

			Vector3 worldDirection = transform.TransformDirection(localDirection);

			transform.position += worldDirection * Time.deltaTime * moveSpeed;
			transform.position=new Vector3(transform.position.x,GetGroundHeight(transform.position)+0.2f,transform.position.z);
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if(Input.GetButton("RotationFreature"))
			EditWorldController.Instance.MouseWheelRotation(scroll);
			
		else
			distanceToPlayer -= scroll * zoomSpeed;
		
	}
	float GetGroundHeight( Vector3 position)
	{
		//return gameWorld.GetHeight(new Vector2(position.x,position.z));
		position=position+100*Vector3.up;
		Ray ray = new Ray(position,Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			return hit.point.y;
		}
		return 0;
	}
}
