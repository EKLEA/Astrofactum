using UnityEngine;

public abstract class FreatureInfo : ScriptableObject
{
	public Sprite icon;
	public string id;
	public string title;
	public string description;
	public FreatureType FreatureType;
}
public enum FreatureType
{
    Action,
    item,
}
