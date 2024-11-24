using UnityEngine;

[CreateAssetMenu(menuName = "Astrofactum/BuildingInfo")]
public class BuildingInfo : ScriptableObject
{
	//public Vector3 size;
	public GameObject prefab;
	public Sprite icon;
	public string id;
	public string disctiption;
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
	
}