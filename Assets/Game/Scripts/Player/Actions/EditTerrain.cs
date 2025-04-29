using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditTerrain : ActionWithWorld
{
	int radius=5;
	AnimationCurve curveX;
	AnimationCurve curveZ;
	private float targetHeight; 
	private bool isTargetHeightSet = false; 

	public EditTerrain(string id)
	{
		curveX = InfoDataBase.terrainBase.GetInfo(id).curveX;
		curveZ = InfoDataBase.terrainBase.GetInfo(id).curveZ;
		canAction=true;
	}

	//public TerrainGeneretion gameWorld => TerrainGeneretion.Instance;

	public override void UpdateFunc()
	{
		currentPos=hit.point;
		
	}
	public override void AddPoint()
	{
		
		if (hit.collider.gameObject.tag == "Ground")
		{
			/*if (!isTargetHeightSet && points.Count == 0)
			{
				//targetHeight = gameWorld.GetHeight(new Vector2(hit.point.x, hit.point.z));
				isTargetHeightSet = true;
				
			}*/
		}
		base.AddPoint();
	}


	public override void MouseWheelRotation(float Value)
	{
		radius=Math.Clamp(radius+(int)(Value * 10),1,10);
		Debug.Log(radius);
	}

	HashSet<Vector3> uniquePoints = new HashSet<Vector3>();

	public override void ActionL()
	{/*
		for (int i = 0; i < points.Count - 1; i++)
			AddPoints(points[i], points[i + 1]);
		
		//gameWorld.EditTerrain(uniquePoints.ToArray());
		*/
	}

	void AddPoints(Vector3 point1, Vector3 point2)
	{
		Vector3 direction = (point2 - point1).normalized;
		float distance=Vector3.Distance(point1, point2);
		float t = 0;
		Vector3 p = point1;

		while (t <= Vector3.Distance(point1, point2))
		{
			for (float x = p.x - radius - 2; x <= p.x + radius + 2; x++)
			{
				for (float z = p.z - radius - 2; z <= p.z + radius + 2; z++)
				{
					float evaluatedHeight =targetHeight* curveX.Evaluate(x / distance>0?distance:1	) * curveZ.Evaluate(z /distance>0?distance:1);
					Vector3 pos = new Vector3(x, z, evaluatedHeight);

					if (Vector2.Distance(new Vector2(p.x, p.z), new Vector2(pos.x, pos.y)) <= radius)
					{
						uniquePoints.Add(pos);
					}
				}
			}
			p += direction * radius;
			t += radius;
			
		}
	}
}
