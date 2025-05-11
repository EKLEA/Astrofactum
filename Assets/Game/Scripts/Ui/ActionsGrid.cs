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
            _uiActions.Add(uIAction.id,(act,uIAction));
            ActionButton s =Instantiate(UIManager.Instance.actionButtonExample,transform);
            s.SetUpButton(uIAction.id,uIAction.title,uIAction.icon,this);
        }
    }
    public override void Disable()
    {
        foreach(var c in _uiActions.Values)
        {
            c.Item1.Disable();
        }
        base.Disable();
    }
    public override void Enable()
    {
        foreach(var c in _uiActions.Values)
        {
            c.Item1.Enable();
        }
        base.Enable();
    }
    public override void InvokeMethod(string id,ActionButton button)
    { 
        onActionInvoke(_uiActions[id]);
    } 
}