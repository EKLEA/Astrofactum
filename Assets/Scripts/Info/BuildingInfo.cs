using UnityEngine;

[CreateAssetMenu(menuName = "Astrofactum/BuildingInfo")]
public class BuildingInfo : FreatureInfo
{
	public GameObject prefab;
	public BuildingsTypes buildingType;
	public int tier;
	public float weight;
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