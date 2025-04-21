using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIController itemsGrid;//реализовать через ворлдконтрлолер
    public ActionsGrid actionsGrid;
    public static UIManager Instance;
    public ActionButton actionButtonExample;
    UIController[] controllers;
    void Awake()
    {
         if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
		Init();
    }
    public void Init()
    {
		//itemsGrid.Init();
		actionsGrid.Init();
		controllers=GetComponentsInChildren<UIController>(true);
		actionsGrid.onActionInvoke+=ChangeWindow;
    }
    void ChangeWindow((UIController,UIActionInfo) Info)
    {
       if(Info.Item1.gameObject.activeSelf==false)
       {
            if (Info.Item2.DisableAnotherUI)
            {
                foreach( var con in controllers)
                {
                    if(con!=Info.Item1) con.gameObject.SetActive(false);
                }
            }
            Info.Item1.gameObject.SetActive(true);
       }
       else
       {
           Info.Item1.gameObject.SetActive(false);
       }
    }
}