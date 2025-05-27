using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Receiver : Building, IWorkWithItems,IHavePorts,IAmTickable
{
    public ItemStack[] Outputs => null;
    public Port[] InPorts =>_inPorts;

    public Port[] OutPorts => null;
    [SerializeField] Port[] _inPorts;

    public List<Slot> inSlots =>null;

    public List<Slot> outSlots =>null;

    public float CurrentProcent => 100;

    public bool CanAdd => true;

    public bool IAmSetUped {get{return _isAmSetUped;}}

    public bool IsRemovedNow { get; set; }


    bool _isAmSetUped;
    public bool canChange{ get; set; }

    public ProcessionState state => ProcessionState.AwaitForInput;

    protected float _currentTime;
    protected bool _isProcessed;

    public event Action OnUIUpdate;
    public event Action<ProcessionState> onStateChanged;

    public override void Init(string id)
    {
        base.Init(id);
        _inPorts[0].toBuilding=this;
        
    }
    public virtual void SetUpLogic()
	{
	    _inPorts[0].toBuilding=this;
        _isAmSetUped=true;
	}   
    public void Tick()
    {
        onStateChanged?.Invoke(state);
    }
    public SlotTransferArgs AddToBuilding(string id, int amount)
    {
        LevelTaskController.Instance.AddItem(id, amount);
        return new SlotTransferArgs(id, amount);
    }
    public bool CanAddM(string id)
    {
        return true;
    }
    public override void Destroy()
    {
        foreach(var p in _inPorts)
        {
            p.toBuilding=null;
            p.Ping();
        }
        if (_isAmSetUped) LevelTaskController.Instance.RemoveScore(10);
        base.Destroy();
    }

    public void Clear()
    {
        
    }
}