using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWithPorts : BuildingLogicBase
{
    public event Action<Slot> OnItemsCanAdded;
    public event Action<Slot> OnItemsCanRemoved;
    
    public Dictionary<Slot,bool> innerItemsSlots =new();
    public Dictionary<Slot,bool> outItemsSlots=new();
    public override void Init(string id)
    {
        base.Init(id);
        ///джсон
        ///инициализация словт
        ///
        
    }
    public virtual SlotTransferArgs AddToBuilding(Slot target)
    {
        return null;
    }
    public virtual SlotTransferArgs RemoveFromBuilding(Slot target)
    {
        return null;
    }
    public void InvokeCanAdded(Slot _slot)
    {
        OnItemsCanAdded?.Invoke(_slot);
    }
    public void InvokeCanRemoved(Slot _slot)
    { 
        OnItemsCanRemoved?.Invoke(_slot);
    }
}
public class SlotTransferArgs:EventArgs
{
    public Slot remainingSlot{get;}
    public SlotTransferArgs(Slot _slot)
    {
        remainingSlot=_slot;
    }
}