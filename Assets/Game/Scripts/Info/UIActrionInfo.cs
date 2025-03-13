using UnityEngine;
[CreateAssetMenu(menuName = "Astrofactum/UIActionInfo")]
public class UIActionInfo: ScriptableObject
{
    public string actionName;
    public UIController uiController;
    public bool DisableAnotherUI;
}
