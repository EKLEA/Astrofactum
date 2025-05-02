using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWorkWithItems
{
    public bool CanAdd{get;}
    public bool IsRemovedNow{get;set;}
    public bool IAmSetUped{get;}
    public List<Slot> inSlots{get;}
    public List<Slot> outSlots{get;}
    public SlotTransferArgs AddToBuilding(string id,int amount);
    public virtual void Ping(){}
    public bool CanAddM(string id){return true;}
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