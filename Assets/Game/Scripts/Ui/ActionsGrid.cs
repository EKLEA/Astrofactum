using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ActionsGrid : UIController
{
    public event Action<(UIController,UIActionInfo)> onActionInvoke;
    Dictionary<string, (UIController,UIActionInfo)> _uiActions=new();
    public override void Init()
    {
        UIActionInfo[] uIActionsInfo= Resources.LoadAll<UIActionInfo>("UIActions");
        foreach(UIActionInfo uIAction in uIActionsInfo)
        {
            var act=Instantiate(uIAction.uiController,transform.parent);
            act.Init();
            act.gameObject.SetActive(false);
            _uiActions.Add(uIAction.actionName,(act,uIAction));
            ActionButton s =Instantiate(UIManager.Instance.actionButtonExample,transform);
            s.SetUpButton(uIAction.name,this);
        }
    }
    public override void InvokeMethod(string id,ActionButton button)
    { 
        onActionInvoke(_uiActions[id]);
    } 
}