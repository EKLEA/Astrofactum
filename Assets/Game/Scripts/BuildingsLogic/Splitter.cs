using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Splitter : Building,IHavePorts,IAmTickable,IWorkWithItems
{
    [SerializeField] protected Port[] _inPortsGM;
    [SerializeField] protected Port[]_outPortsGM;
    public Port[] OutPorts => _outPortsGM;

    public Port[] InPorts => _inPortsGM;

    public bool IsProcessed => false;

    public float CurrentProcent => 100f;

    public bool CanAdd => _slot==null&&_outPortsGM.Where(f=>f.transferSlot==null&&f.toBuilding!=null&&f.toBuilding.CanAdd).Count()>0;

    public bool IsRemovedNow { get;set; }

    public bool IAmSetUped {get;private set;}

    public List<Slot> inSlots => new List<Slot>(){_slot};

    public List<Slot> outSlots => throw new NotImplementedException();

    public ProcessionState state {get; private set;}

    Slot _slot;
    int currInd;

    public event Action<ProcessionState> onStateChanged;

    public override void Init(string tid)
    {
        base.Init(tid);
        foreach(var p in _inPortsGM)
            p.toBuilding=this;
        foreach(var p in _outPortsGM)
            p.fromBuilding=this;
    }
    public virtual void SetUpLogic()
	{
	    TickManager.Instance.Subscribe(this);
        IAmSetUped=true;
	}    
    public SlotTransferArgs AddToBuilding(string id,int amount)
    {
        
        _slot= new Slot(id,999,amount);
       
        return new SlotTransferArgs(id,_slot.Count);
    }

    public virtual void Tick(float deltaTime)
    {
        state=ProcessionState.AwaitForInput;
        if(_slot==null) return;
        state=ProcessionState.AwaitForOutput;
        if(_outPortsGM[currInd].toBuilding==null||_outPortsGM[currInd].toBuilding.CanAdd==false) currInd++;
        if(currInd>=_outPortsGM.Length) currInd=0;
        var t=_slot.RemoveItem();
        state=ProcessionState.Processed;
        if(t!=0) 
        {
            if(_outPortsGM[currInd].transferSlot==null) _outPortsGM[currInd].transferSlot=new Slot(_slot.Id,_slot.MaxCount,t);
        }
        else
        {
            _slot=null;
        }
        onStateChanged?.Invoke(state);
        
    }
    public override void Destroy()
    {
        foreach(var p in _outPortsGM)
        {
             p.toBuilding=null;
             p.Ping();
        }
        foreach(var p in _inPortsGM)
        {
            p.fromBuilding=null;
            p.Ping();
        }
        TickManager.Instance.Unsubscribe(this);
        if (IAmSetUped) LevelTaskController.Instance.RemoveScore(10);
        base.Destroy();
    }

    public void Clear()
    {
       
        foreach(var p in _inPortsGM)
        {
             if (IAmSetUped&&p.transferSlot!=null) LevelTaskController.Instance.RemoveScore(p.transferSlot.Count);
            p.transferSlot=null;
        }
        foreach(var p in _outPortsGM)
        {
             if (IAmSetUped&&p.transferSlot!=null)  LevelTaskController.Instance.RemoveScore(p.transferSlot.Count);
             p.transferSlot=null;
        }
    }
}