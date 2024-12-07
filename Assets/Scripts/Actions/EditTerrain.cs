using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class EditTerrain : ActionWithWorld
{
	public Terrain terrain;
	public float radius = 5f; //изменть
	private float targetHeight; 
	public TerrainGeneretion gameWorld;

	public override void SetUpAction(int minCount)
	{
		base.SetUpAction(minCount);
		gameWorld=TerrainGeneretion.Instance;
		canAction=true;
	}
	public override void AddPoint()
	{
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.gameObject.tag=="Ground")
			{
				if(points.Count==0)
				{
					targetHeight=gameWorld.GetHeight(new Vector2(hit.point.x, hit.point.z));
				}
				points.Add(hit.point);
				
			}
		}
	}

	HashSet<Vector2> uniquePoints = new HashSet<Vector2>();
	
	public override void Action()
	{
		for (int i = 0; i < points.Count-1; i++)
			AddPoints(points[i],points[i+1]);
			
			
		
		gameWorld.EditTerrain(uniquePoints.ToArray(),targetHeight);
	}
	
	void AddPoints(Vector3 point1,Vector3 point2)
	{
		Vector3 direction = (point2 - point1).normalized;
		float t =0;
		Vector3 p=point1;
		while (t<=Vector3.Distance(point1,point2))
		{
			
			
			for (float x = p.x- radius-2; x <=p.x + radius+2; x++)
			{
				for (float z = p.z - radius-2; z <= p.z + radius+2; z ++)
				{
					Vector2 pos = new Vector2(x, z);
					if (Vector2.Distance(new Vector2(p.x, p.z), pos) <= radius)
					{
						uniquePoints.Add(pos);
						
					}
				}
			}	
			p+= direction * radius;	
			t+=radius;
		}
		
	   
	}

}

