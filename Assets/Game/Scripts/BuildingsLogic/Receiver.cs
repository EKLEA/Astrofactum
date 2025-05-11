using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Receiver : Building, IWorkWithItems, IAmTickable,IHavePorts,IWorkWithRecipe
{
    public string recipeID {get{return recipe.Id;}}
    public float duration {get{return recipe.Duration;}}
    public ItemStack[] Outputs{get{return recipe.Outputs.ToArray();}}
    private Recipe recipe;
    public float GenerationSpeed{get{return generationSpeed;}}
    [field:SerializeField] float generationSpeed;
    public Port[] InPorts =>_inPorts;

    public Port[] OutPorts => null;
    [SerializeField] Port[] _inPorts;
    public bool IsProcessed {get{return _isProcessed;}}

    public List<Slot> inSlots =>null;

    public List<Slot> outSlots =>null;

    public float CurrentProcent {get{return _currentTime/(float)recipe.Duration*100;}}

    public bool CanAdd => true;

    public bool IAmSetUped {get{return _isAmSetUped;}}

    public bool IsRemovedNow {get{return _isRemovedNow;}set{_isRemovedNow = value;}}

    public ProcessionState state{get;private set;}
    public RecipeTag recipeTag{get{return _recipeTag;}}

    public RecipeTag _recipeTag;

    bool _isRemovedNow;

    bool _isAmSetUped;
    int max;
    Slot _slot;

    protected float _currentTime;
    protected bool _isProcessed;

    public event Action<ProcessionState> onStateChanged;
    public event Action OnUIUpdate;

    public override void Init(string id)
    {
        base.Init(id);
        _inPorts[0].toBuilding=this;
    }
    public void SetUpLogic()
    {
        _inPorts[0].toBuilding=this;
        _isAmSetUped=true;
        //задвать айди
        _slot=new Slot("iron_ore");
    }
    public void Tick(float deltaTime)
    {
        onStateChanged?.Invoke(state);
        OnUIUpdate?.Invoke();
    }
    public SlotTransferArgs AddToBuilding(string id, int amount)
    {
        state=ProcessionState.Processed;
        _slot.AddItem(amount);
        Debug.Log(amount);
        //обращаться к главному контролерру сбора метрик
        state=ProcessionState.AwaitForInput;
        return new SlotTransferArgs(id, amount);
    }
    public bool CanAddM(string id)
    {
        //проверять на йди
        return true;
    }
    public override void Destroy()
    {
        foreach(var p in _inPorts)
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
        foreach(var p in _inPorts) p.transferSlot=null;
    }
}