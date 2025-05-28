using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
public class BeltSpline : SplineParent,IAmTickable,IWorkWithItems
{   
    
    [SerializeField] float speed;
    public List<Slot> inSlots=>null;

    public List<Slot> outSlots
    {
        get
        {
            return new List<Slot>(){OutTransferSlot};
        }
    }

    public float CurrentProcent => throw new NotImplementedException();

    public bool CanAdd {
    get
    {
        return (splineItems.Count == 0 || splineItemsQueue.Count>0&&splineItems.Last().value > (1f / (float)countOfSegments))&&_isAmSetUped;
    }}

    [SerializeField] SplineItem prefab;
    
    Slot OutTransferSlot;
    
    int countOfSegments;
    bool canMove;
    bool isWisible;
    event Action<float> OnMove;
    event Action OnVisualUpdate;
    public event Action<ProcessionState> onStateChanged;

    List<SplineItem> splineItems=new();
    Queue<SplineItem> splineItemsQueue=new();
    public bool IAmSetUped {get{return _isAmSetUped;}}
    bool _isAmSetUped;
    public bool IsRemovedNow{get;set;}

    public ProcessionState state{get;private set;}
    int t;
    public override void Init(string tid)
    {
        base.Init(tid);
    }
    public void SetUpLogic()
    {
        TickManager.Instance.Subscribe(this);
        _isAmSetUped=true;
        countOfSegments =Mathf.FloorToInt(spline.GetLength()/3);
        for (int i = 0; i < countOfSegments; i++)
        {
            SplineItem item = Instantiate(prefab, transform);
            
            item.gameObject.SetActive(false);
            splineItemsQueue.Enqueue(item);
            Vector3 pos = spline.EvaluatePosition(0);
            item.transform.localPosition = new Vector3(pos.x, item.transform.localPosition.y+1f, pos.z);
        }
        canMove = true;
        isWisible=true;
        IsRemovedNow=false;
        //state=ProcessionState.AwaitForInput;
        //Ping();
    }
    public void UpdateBuilding()
    { 
        if(LogicInPort==null||LogicInPort.toBuilding==null) SetUpInPort(null);
        if(LogicOutPort==null||LogicOutPort.fromBuilding==null) SetUpOutPort(null);
    }
    public override void SetUpInPort(Port inPort)
    {
        base.SetUpInPort(inPort);
        LogicInPort.toBuilding=this;
        
    }
    public override void SetUpOutPort(Port outPort)
    {
        if(LogicOutPort!=null)LogicOutPort.onItemRemovedFromSlot=null;
        base.SetUpOutPort(outPort);
        LogicOutPort.fromBuilding=this;
        LogicOutPort.onItemRemovedFromSlot+=RemoveItem;
        
    }
    void Update()
    {
        OnVisualUpdate?.Invoke();
        //Debug.Log(LogicInPort[0].transform.parent.name+"     "+LogicInPort[0]+"         "+LogicInPort[0]._toBuilding+"      "+LogicInPort[0]._fromBuilding);
    }
    public void Tick(float deltaTime)
    {
       
       canMove=Move(deltaTime/countOfSegments*speed);
       onStateChanged?.Invoke(state);
    }
    bool Move(float deltaTime)
    {
        OnMove?.Invoke(deltaTime);
        LogicInPort.Ping();
        LogicOutPort.Ping();
        if (splineItems.Count > 0 && splineItems[0].value == 1)
        {
            OutTransferSlot=splineItems[0].slot;
            LogicOutPort.transferSlot=OutTransferSlot;
        }
        if(t!=splineItemsQueue.Count)
        {
            t=splineItemsQueue.Count;
            state=ProcessionState.Processed;
        }
        else  state=ProcessionState.AwaitForInput;  
        
        if (splineItemsQueue.Count == 0)
        {
            state=ProcessionState.AwaitForOutput;
            return false;
        }
        
        return false;
    }
        
    public SlotTransferArgs AddToBuilding(string id, int amount)
    {
        int max=InfoDataBase.itemInfoBase.GetInfo(id).maxCountInPack;
        int res=0;
        if(splineItemsQueue.Count>0)
        {
            if(splineItems.Count==0|| splineItems.Last().value>(1f/(float)countOfSegments))
            {
                var p=splineItemsQueue.Dequeue();
                var item=new Slot(id,max,amount);
                p.SetUpSplineItem(item,this.spline);
                splineItems.Add(p);
                
                p.target=(float)(countOfSegments-splineItems.IndexOf(p)) / (float)countOfSegments;
                p.gameObject.SetActive(true);
                OnMove+=p.UpdateValue;
                OnVisualUpdate+=p.ChangePos;
                res=amount;
            }
        }
        else if(splineItemsQueue.Count==0&&splineItems.Count>0)
        {
            if(splineItems.Last().slot.Id==id&&splineItems.Last().slot.Count!=splineItems.Last().slot.MaxCount)
            {
                res=splineItems.Last().slot.AddItem(amount);
            }
        }
        return new SlotTransferArgs(id,res);
    }

    public void RemoveItem(Port port)
    {
        if(splineItems.Count>0&&OutTransferSlot!=null)
        {
            if(splineItems[0].slot.Count==0)
            {
                splineItems[0].slot=null;
                splineItems[0].gameObject.SetActive(false);
                splineItemsQueue.Enqueue(splineItems[0]);
                OnMove-=splineItems[0].UpdateValue;
                OnVisualUpdate-=splineItems[0].ChangePos;
                splineItems.RemoveAt(0);
                foreach(var item in splineItems)
                {   
                    item.UpdateTarget((float)(countOfSegments-splineItems.IndexOf(item))/(float)countOfSegments);
                    
                }
            }
        }
    }
    public override bool ValidateSpline()
    {
        if (!CheckSplineCurvature(180,countOfSegments))
        {
            Debug.Log("Слишком резкие повороты");
            return false;
        }

        if (CheckSelfIntersections())
        {
            Debug.Log("Обнаружены самопересечения");
            return false;
        }

        return true;
    }
   public override void Destroy()
    {
        Clear();

        TickManager.Instance.Unsubscribe(this);

        if (LogicInPort != null)
        {
            LogicInPort.toBuilding = null;
            LogicInPort.transferSlot = null;
        }

        if (LogicOutPort != null)
        {
            LogicOutPort.fromBuilding = null;
            LogicOutPort.transferSlot = null;
            LogicOutPort.onItemRemovedFromSlot -= RemoveItem;
        }

        foreach (var item in splineItemsQueue)
        {
            if (item != null) Destroy(item.gameObject);
        }
        splineItemsQueue.Clear();
        if (IAmSetUped) LevelTaskController.Instance.RemoveScore(10);
        base.Destroy();

    }
    public void Clear()
    {
        if (splineItems.Count > 0)
        {
            for (int i = splineItems.Count - 1; i >= 0; i--)
            {
                if (splineItems[i] != null)
                {
                    splineItems[i].slot = null;
                    splineItems[i].gameObject.SetActive(false);
                    splineItemsQueue.Enqueue(splineItems[i]);
                    OnMove -= splineItems[i].UpdateValue;
                    OnVisualUpdate -= splineItems[i].ChangePos;
                }
            }
            if (IAmSetUped) LevelTaskController.Instance.RemoveScore(splineItems.Count);
            splineItems.Clear();
        }
        
        LogicInPort.transferSlot = null;
        LogicOutPort.transferSlot = null;
        OutTransferSlot = null;

        state = ProcessionState.AwaitForInput;
    }
}