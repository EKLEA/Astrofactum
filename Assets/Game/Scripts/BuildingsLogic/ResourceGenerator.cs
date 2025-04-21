using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ResourceGenerator : Building, IWorkWithItems, IAmTickable , IHavePorts,IWorkWithRecipe
{
    public string recipeID {get{return recipe.Id;}}
    public float duration {get{return recipe.Duration;}}
    public ItemStack[] Outputs{get{return recipe.Outputs.ToArray();}}
    private Recipe recipe;
    public float GenerationSpeed{get{return generationSpeed;}}
    [field:SerializeField] float generationSpeed;
    public Port[] InPorts =>null;

    public Port[] OutPorts => _outPorts;
    [SerializeField] Port[] _outPorts;

    public event Action<string> OnItemsCanAdded;
    public event Action<string> OnItemsCanRemoved;
    private Slot GeneratorSlot;
    public bool IsProcessed {get{return _isProcessed;}}

    public List<Slot> inSlots =>null;

    public List<Slot> outSlots {get{return _outSlots;}}

    public float CurrentProcent {get{return _currentTime/(float)recipe.Duration*100;}}

    public bool CanAdd => throw new NotImplementedException();

    public bool IAmSetUped {get{return _isAmSetUped;}}
    bool _isAmSetUped;

    private List<Slot> _outSlots=new();
    protected float _currentTime;
    protected bool _isProcessed;
    public override void Init(string id)
    {
        base.Init(id);
        _outPorts[0].PortUpdateFrom(this);
        SetUpReciepe(null);
       
    }
    public void SetUpLogic()
    {
        TickManager.Instance.Subscribe(this);
        _isAmSetUped=true;
    }
    
    public SlotTransferArgs AddToBuilding(string id, int amount)
    {
        return null;
    }

    public SlotTransferArgs RemoveFromBuilding(string id, int amount)
    {
       var removedItems =GeneratorSlot.RemoveItem(amount);
        return new SlotTransferArgs(id, removedItems);
    }

    public void InvokeCanAdded(string id)
    {
        //
    }

    public void InvokeCanRemoved(string id)
    {
       OnItemsCanRemoved?.Invoke(id);
    }

   
    public void SetUpReciepe(string id)
    {
        recipe=InfoDataBase.recipeBase["generate_iron_ore"];
        GeneratorSlot=new Slot(recipe.Outputs[0].id);
        _outSlots.Clear();
        _outSlots.Add(GeneratorSlot);
        
        
    }
    public void Tick(float deltaTime)
    {   
        if(GeneratorSlot.MaxCount-GeneratorSlot.Count<=recipe.Outputs[0].amount) _isProcessed=false;
        else
        {
            _isProcessed=true;
            if(_currentTime>=recipe.Duration)
            {
                GeneratorSlot.AddItem(recipe.Outputs[0].amount);
                _currentTime=0;
                InvokeCanRemoved(GeneratorSlot.Id);
            }
            else _currentTime+=deltaTime;
        }
    }
    public void ResetAddEvent(){OnItemsCanAdded=null;}
    public void ResetRemoveEvent(){OnItemsCanRemoved=null;}

}