using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ResourceGenerator : Building, IWorkWithItems, IAmTickable , IHavePorts,IWorkWithRecipe
{
    public string recipeID {get{return recipe!=null?recipe.Id:null;}}
    public float duration {get{return recipe!=null?recipe.Duration:0f;}}
    public ItemStack[] Outputs{get{return recipe!=null?recipe.Outputs.ToArray():null;}}
    private Recipe recipe;
    public float GenerationSpeed{get{return generationSpeed;}}
    [field:SerializeField] float generationSpeed;
    public Port[] InPorts =>null;

    public Port[] OutPorts => _outPorts;
    [SerializeField] Port[] _outPorts;
    private Slot GeneratorSlot;

    public List<Slot> inSlots =>null;

    public List<Slot> outSlots {get{return _outSlots;}}

    public float CurrentProcent {get{return _currentTime/(float)recipe.Duration*100;}}

    public bool CanAdd => throw new NotImplementedException();

    public bool IAmSetUped {get{return _isAmSetUped;}}

    public bool IsRemovedNow {get{return _isRemovedNow;}set{_isRemovedNow = value;}}

    public ProcessionState state {get;private set;}
    public RecipeTag recipeTag{get{return _recipeTag;}}

    public RecipeTag _recipeTag;

    bool _isRemovedNow;

    bool _isAmSetUped;
    int max;

    private List<Slot> _outSlots=new();
    protected float _currentTime;

    public event Action<ProcessionState> onStateChanged;
    public event Action OnUIUpdate;
    public bool canChange{ get; set; }
    public override void Init(string id)
    {
        base.Init(id);
        
    }
    public void SetUpLogic()
    {
        TickManager.Instance.Subscribe(this);
        _outPorts[0].fromBuilding=this;
        _isAmSetUped=true;
        
    }
    public void SetUpReciepe(string id)
    {
        recipe=InfoDataBase.recipeBase[id];
        GeneratorSlot=new Slot(recipe.Outputs[0].id,5);
        max=InfoDataBase.itemInfoBase.GetInfo(GeneratorSlot.Id).maxCountInPack;
        _outSlots.Clear();
        _outSlots.Add(GeneratorSlot);
        canChange = false;
        
    }
    public void Tick(float deltaTime)
    {   
        if(GeneratorSlot==null) return;
        if(GeneratorSlot.MaxCount-GeneratorSlot.Count<recipe.Outputs[0].amount)
        {
             state=ProcessionState.AwaitForOutput;
        }
        else
        {
            state=ProcessionState.Processed;
            if(_currentTime>=recipe.Duration)
            {
                state=ProcessionState.Processed;
                GeneratorSlot.AddItem(recipe.Outputs[0].amount);
                _currentTime=0;
                if(_outPorts[0].transferSlot==null) _outPorts[0].transferSlot=GeneratorSlot;
                if(GeneratorSlot==null) GeneratorSlot=new Slot(recipe.Outputs[0].id,5);
                OnUIUpdate?.Invoke();
            }
            else _currentTime+=deltaTime;
        }
        onStateChanged?.Invoke(state);
    }

    public SlotTransferArgs AddToBuilding(string id, int amount)
    {
        throw new NotImplementedException();
    }
    public override void Destroy()
    {
        foreach(var p in _outPorts)
        {
            p.fromBuilding=null;
            p.Ping();
        }
        TickManager.Instance.Unsubscribe(this);
        base.Destroy();
    }
    public void Clear()
    {
        OnUIUpdate?.Invoke();
        GeneratorSlot.RemoveItem();
        foreach(var p in _outPorts) p.transferSlot=null;
    }
}