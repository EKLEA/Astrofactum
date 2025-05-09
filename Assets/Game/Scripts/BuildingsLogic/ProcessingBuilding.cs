using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ProcessingBuilding : Building, IWorkWithItems, IAmTickable , IHavePorts,IWorkWithRecipe
{
    public string recipeID {get{return recipe.Id;}}
    public float duration {get{return recipe.Duration;}}
    public ItemStack[] Outputs{get{return recipe.Outputs.ToArray();}}
    public ItemStack[] InPuts{get{return recipe.Inputs.ToArray();}}
    private Recipe recipe;
    
    public float GenerationSpeed{get{return generationSpeed;}}
    [SerializeField] float generationSpeed;
    public Port[] InPorts =>_inPorts;

    public Port[] OutPorts => _outPorts;
    
    [SerializeField] Port[] _outPorts;
    [SerializeField] Port[] _inPorts;

    public List<Slot> inSlots{get{return _inSlots;}}

    public List<Slot> outSlots {get{return _outSlots;}}

    public float CurrentProcent {get{return _currentTime/(float)recipe.Duration*100;}}

    public bool CanAdd => _inSlots.Where(f=>f.IsFull).Count()<_inSlots.Count();

    public bool IAmSetUped {get;private set;}

    public bool IsRemovedNow {get;set;}

    public ProcessionState state {get;private set;}

    public RecipeTag recipeTag;
    int max;
    Dictionary<string,(Slot,ItemStack,Port)> inSlotsD=new();
    Dictionary<string,(Slot,ItemStack,Port)> outSlotsD=new();
    private List<Slot> _outSlots=new();
    private List<Slot> _inSlots=new();
    protected float _currentTime;

    public event Action<ProcessionState> onStateChanged;

    public override void Init(string id)
    {
        base.Init(id);
        SetUpReciepe(null);
       
    }
    public void SetUpLogic()
    {
        TickManager.Instance.Subscribe(this);
        foreach(var i in _inPorts)
            i.toBuilding=this;
        foreach(var o in _outPorts)
            o.fromBuilding=this;
        IAmSetUped=true;
    }
    public void SetUpReciepe(string id)
    {
        
        recipe=InfoDataBase.recipeBase["iron_ingot"];
        if(recipeTag!=recipe.Tag) return;
        if(recipe.Inputs.Count>_inPorts.Count()|| recipe.Outputs.Count >_outPorts.Count()) return;
        for(int i=0;i<recipe.Inputs.Count();i++)
        {
            _inSlots.Add(new Slot(recipe.Inputs[i].id,1));
            inSlotsD.Add(recipe.Inputs[i].id,(_inSlots[i],recipe.Inputs[i],_inPorts[i]));
        }
        for(int i=0;i<recipe.Outputs.Count();i++)
        {
            _outSlots.Add(new Slot(recipe.Outputs[i].id,1));
            outSlotsD.Add(recipe.Outputs[i].id,(_outSlots[i],recipe.Outputs[i],_outPorts[i]));
        }
        
    }
    public void Tick(float deltaTime)
    {   
        var Inputs=inSlotsD.Where(f=>f.Value.Item1.Count>=f.Value.Item2.amount);
        if(Inputs.Count()!=inSlotsD.Count()) state=ProcessionState.AwaitForInput;
        var Outputs= outSlotsD.Where(f=>(f.Value.Item1.MaxCount-f.Value.Item1.Count)>=f.Value.Item2.amount&&f.Value.Item3.transferSlot==null);
        if(Outputs.Count()!=outSlotsD.Count) state=ProcessionState.AwaitForOutput;
        if(Inputs.Count()!=inSlotsD.Count()&&Outputs.Count()!=outSlotsD.Count)
        {
            state=ProcessionState.Processed;
            if(_currentTime>=recipe.Duration)
            {
                foreach(var i in inSlotsD.Values)
                {
                    i.Item1.RemoveItem(i.Item2.amount);
                }
                foreach(var o in outSlotsD.Values)
                {
                    o.Item1.AddItem(o.Item2.amount);
                    o.Item3.transferSlot=o.Item1;
                }
                _currentTime=0;
            }
            else _currentTime+=deltaTime;
        }
        onStateChanged?.Invoke(state);
       
    }
    public bool CanAddM(string _id)
    {
        return inSlotsD.Where(f=> f.Key==_id&&f.Value.Item1.MaxCount-f.Value.Item1.Count>=f.Value.Item2.amount).Count()>0;
    }
    public SlotTransferArgs AddToBuilding(string _id, int _amount)
    {
        var insl=_inSlots.Where(f=>f.Id==_id).ToArray();
        int r=0;
        if(insl.Count()>0)
        {
           r= insl[0].AddItem(_amount);
        }
        return new SlotTransferArgs(_id,r);
    }
    public void Clear()
    {
        foreach(var p in inSlotsD.Values)
        {
            p.Item1.RemoveItem();
            p.Item3.transferSlot=p.Item1;
        }
        foreach(var p in outSlotsD.Values)
        {
            p.Item1.RemoveItem();
            p.Item3.transferSlot=p.Item1;
        }
    }
    public override void Destroy()
    {
        TickManager.Instance.Unsubscribe(this);
        foreach(var i in _inPorts)
        {
            i.toBuilding=null;
            i.Ping();
        }
        foreach(var o in _outPorts)
        {
            o.fromBuilding=null;
            o.Ping();
        }
        base.Destroy();
    }
}