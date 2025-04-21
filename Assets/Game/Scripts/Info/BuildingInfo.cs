using UnityEngine;

[CreateAssetMenu(menuName = "Astrofactum/Actions/BuildingInfo")]
public class BuildingInfo : ActionInfo
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