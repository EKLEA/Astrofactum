using UnityEngine;

[CreateAssetMenu(menuName = "Astrofactum/BuildingInfo")]
public class BuildingInfo : FreatureInfo
{
	public GameObject prefab;
	public Vector3 normal;
	public BuildingsTypes buildingType;
}
public enum BuildingsTypes
{
	Special,
	Production,
	Elect,
	Logistic,
	Logic,
	Foundation,
	Terrain
	
}