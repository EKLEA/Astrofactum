using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ProcessingBuilding : Building, IWorkWithItems, IAmTickable , IHavePorts,IWorkWithRecipe
{
    public string recipeID {get{return recipe!=null?recipe.Id:null;}}
    public float duration {get{return recipe!=null?recipe.Duration:0f;}}
    public ItemStack[] Outputs{get{return recipe!=null?recipe.Outputs.ToArray():null;}}
    public ItemStack[] InPuts{get{return recipe!=null?recipe.Inputs.ToArray():null;}}
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

    public RecipeTag recipeTag{get{return _recipeTag;}}

    public RecipeTag _recipeTag;
    int max;
    Dictionary<string,(Slot,ItemStack,Port)> inSlotsD=new();
    Dictionary<string,(Slot,ItemStack,Port)> outSlotsD=new();
    private List<Slot> _outSlots=new();
    private List<Slot> _inSlots=new();
    protected float _currentTime;
    public bool canChange{ get; set; }

    public event Action<ProcessionState> onStateChanged;
    public event Action OnUIUpdate;

    public override void Init(string id)
    {
        base.Init(id);
        canChange = true;
       
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
        recipe=InfoDataBase.recipeBase[id];
        if(recipeTag!=recipe.Tag) return;
        if(recipe.Inputs.Count>_inPorts.Count()|| recipe.Outputs.Count >_outPorts.Count()) return;
        _inSlots.Clear();
        inSlotsD.Clear();
        for(int i=0;i<recipe.Inputs.Count();i++)
        {
            Slot slot=new Slot(recipe.Inputs[i].id,5);
            inSlotsD.Add(recipe.Inputs[i].id,(slot,recipe.Inputs[i],_inPorts[i]));
        }
        _inSlots=inSlotsD.Values.Select(f=>f.Item1).ToList();
        _outSlots.Clear();
        outSlotsD.Clear();
        for(int i=0;i<recipe.Outputs.Count();i++)
        {
            Slot slot=new Slot(recipe.Outputs[i].id,5);
            outSlotsD.Add(recipe.Outputs[i].id,(slot,recipe.Outputs[i],_outPorts[i]));
        }
        _outSlots=outSlotsD.Values.Select(f=>f.Item1).ToList();
        OnUIUpdate?.Invoke();
    }
    public void Tick(float deltaTime)
    {   
        if(recipe==null) return;
        var Inputs=inSlotsD.Where(f=>f.Value.Item1.Count>=f.Value.Item2.amount);
        if(Inputs.Count()!=inSlotsD.Count()) state=ProcessionState.AwaitForInput;
        
        var Outputs= outSlotsD.Where(f=>(f.Value.Item1.MaxCount-f.Value.Item1.Count)>=f.Value.Item2.amount);
        if(Outputs.Count()!=outSlotsD.Count) state=ProcessionState.AwaitForOutput;
        
        if(Inputs.Count()==inSlotsD.Count()&&Outputs.Count()==outSlotsD.Count)
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
                    //переделать
                }
                OnUIUpdate?.Invoke();
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
           Debug.Log("aaaa");
           OnUIUpdate?.Invoke();
        }
        return new SlotTransferArgs(_id,r);
    }
    public void Clear()
    {
        foreach(var p in inSlotsD.Values)
        {
            if (IAmSetUped) LevelTaskController.Instance.RemoveScore(p.Item1.Count);
            p.Item1.RemoveItem();
            p.Item3.transferSlot=p.Item1;
        }
        foreach(var p in outSlotsD.Values)
        {
            if (IAmSetUped) LevelTaskController.Instance.RemoveScore(p.Item1.Count);
            p.Item1.RemoveItem();
            p.Item3.transferSlot=p.Item1;
        }
        OnUIUpdate?.Invoke();
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
        if (IAmSetUped) LevelTaskController.Instance.RemoveScore(10);
        base.Destroy();
    }
}