using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connector : Building,IHavePorts,IAmTickable,IWorkWithItems
{
    [SerializeField] protected Port[] _inPortsGM;
    [SerializeField] protected Port[]_outPortsGM;
    public Port[] OutPorts => _outPortsGM;

    public Port[] InPorts => _inPortsGM;

    public bool IsProcessed => false;

    public float CurrentProcent => 100f;

    public bool CanAdd => slotsQueue.Count<3&&_outPortsGM[0].transferSlot==null&&_outPortsGM[0].toBuilding!=null&&_outPortsGM[0].toBuilding.CanAdd;

    public bool IsRemovedNow { get;set; }

    public bool IAmSetUped {get;private set;}

    public List<Slot> inSlots => throw new NotImplementedException();

    public List<Slot> outSlots => throw new NotImplementedException();
  
    int currInd;
    Queue<Slot> slotsQueue=new();
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
        _isWork = true;
	}    
    public SlotTransferArgs AddToBuilding(string id,int amount)
    {
        
        if(_inPortsGM[currInd].fromBuilding==null) currInd++;
        if(currInd>=_inPortsGM.Length) currInd=0;
       
        Slot s= new Slot(id,999,amount);
        slotsQueue.Enqueue(s);
       
        return new SlotTransferArgs(id,s.Count);
    }

    public virtual void Tick(float deltaTime)
    {
        if(slotsQueue.Count>0&&_outPortsGM[0].transferSlot==null) _outPortsGM[0].transferSlot=slotsQueue.Dequeue();
    }
}