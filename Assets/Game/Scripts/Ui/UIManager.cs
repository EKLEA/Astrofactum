using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour
{
    public ActionsGrid actionsGrid;
    public static UIManager Instance;
    public ActionButton actionButtonExample;
    UIController[] controllers;
    public void Init()
    {
        //itemsGrid.Init();
        Instance = this;
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
                    if(con!=Info.Item1) con.Disable();
                }
            }
            Info.Item1.Enable();
       }
       else
       {
           Info.Item1.Disable();
       }
    }
}