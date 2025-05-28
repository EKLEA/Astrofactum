using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ActionsGrid : UIController
{
    
    public event Action<(UIController,UIActionInfo)> onActionInvoke;
    Dictionary<string, (UIController,UIActionInfo)> _uiActions=new();
    UIManager uiManager=>UIManager.Instance;
    public override void Init()
    {
        UIActionInfo[] uIActionsInfo= Resources.LoadAll<UIActionInfo>("UIActions");
        foreach(UIActionInfo uIAction in uIActionsInfo)
        {
            var act=Instantiate(uIAction.uiController,transform.parent);
            act.Init();
            
            _uiActions.Add(uIAction.id,(act,uIAction));
            ActionButton s =Instantiate(uiManager.actionButtonExample,transform);
            s.SetUpButton(uIAction.id,uIAction.title,uIAction.icon,this);
            act.Disable();
            
        }
        _uiActions.Values.Where(f => f.Item2.id == "Guide").FirstOrDefault().Item1.Enable();
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
        base.Enable();
    }
    public override void InvokeMethod(string id,ActionButton button)
    { 
        onActionInvoke(_uiActions[id]);
    } 
}