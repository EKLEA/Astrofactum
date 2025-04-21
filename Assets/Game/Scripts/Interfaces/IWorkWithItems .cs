using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWorkWithItems
{
    public bool CanAdd{get;}
    public bool IAmSetUped{get;}
    public event Action<string> OnItemsCanAdded;
    public event Action<string> OnItemsCanRemoved;
    public List<Slot> inSlots{get;}
    public List<Slot> outSlots{get;}
    public SlotTransferArgs AddToBuilding(string id,int amount);
    public SlotTransferArgs RemoveFromBuilding(string id,int amount);
    public void InvokeCanAdded(string id);
    public void InvokeCanRemoved(string id);
    public void ResetAddEvent(){}
    public void ResetRemoveEvent(){}
    public void Ping(){}
}
public class SlotTransferArgs:EventArgs
{
    public string Id{get{return _id;}}
    public int Amount{get{return _amount;}}
    int _amount;
    string _id;
    public SlotTransferArgs(string id,int amount)
    {
        _id=id;_amount=amount;
    }
}