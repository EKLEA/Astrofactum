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
	public GameWorld gameWorld;
	
	
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
			camObj.transform.localPosition=new Vector3(0,ChekY(math.sin(currentAngle*Mathf.Deg2Rad)*_disToPlayer ),-math.cos(currentAngle*Mathf.Deg2Rad)*_disToPlayer);
		}
	}
	float _disToPlayer;
	bool isRotating = false;
	public float _curAn;
	float currentAngle
	{
		get{ return _curAn;}
		set
		{
			if(value>maxAngleCam) _curAn=maxAngleCam;
			else if(value<0) _curAn=0;
			else if
			(
				value<=maxAngleCam
				&&value>=0
				&&((math.sin(value*Mathf.Deg2Rad)*distanceToPlayer)>=GetGroundHeight(camObj.transform.position)+0.01f)
				) _curAn=value;
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
		camObj.transform.localPosition=new Vector3(0,ChekY(math.sin(currentAngle*Mathf.Deg2Rad)*distanceToPlayer ),-math.cos(currentAngle*Mathf.Deg2Rad)*distanceToPlayer);
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
			currentAngle += mouseY * rotationSpeed * Time.deltaTime;
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
			transform.position=new Vector3(transform.position.x,GetGroundHeight(point.transform.position)+0.01f,transform.position.z);
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		distanceToPlayer -= scroll * zoomSpeed;
	}
	float ChekY(float yToChek)
	{
		float temp=  GetGroundHeight(camObj.transform.position);
		if (yToChek <=temp ) return temp+0.01f;
		else return yToChek;
	}
	float GetGroundHeight( Vector3 position)
	{
		RaycastHit hit;
		
		if (Physics.Raycast(position+Vector3.up*100, Vector3.down, out hit))
		{
			var terrain=hit.collider.GetComponent<Terrain>();
			Vector2 posChunkInWorld = gameWorld.GetChunkPos(new Vector2(hit.point.x,hit.point.z));
			return terrain.SampleHeight(new Vector3(hit.point.x-posChunkInWorld.x,0,hit.point.z-posChunkInWorld.y));
		}
		return 20; 
	}
}
