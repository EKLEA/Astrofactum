using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
public class BeltSpline : SplineParent,IAmTickable,IWorkWithItems
{   
    
    [SerializeField] float speed;

    public bool IsProcessed => IsProcessed;
    private bool _isProcessed;
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
    get{return (splineItems.Count == 0 || splineItemsQueue.Count>0&&splineItems.Last().value > (1f / (float)countOfSegments))&&_isAmSetUped;}}

    [SerializeField] SplineItem prefab;
    
    Slot OutTransferSlot;
    
    int countOfSegments;
    bool canMove;
    bool isWisible;
    event Action<float> OnMove;
    event Action OnVisualUpdate;
    List<SplineItem> splineItems=new();
    Queue<SplineItem> splineItemsQueue=new();
    public bool IAmSetUped {get{return _isAmSetUped;}}
    bool _isAmSetUped;
    public bool IsRemovedNow{get;set;}
    public override void Init(string tid)
    {
        base.Init(tid);
        _isWork = false;
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
        _isWork = true;
        canMove = true;
        isWisible=true;
        IsRemovedNow=false;
        //Ping();
    }
    public void Ping()
    {
        LogicInPort.Ping();
        LogicOutPort.Ping();
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
    }
    bool Move(float deltaTime)
    {
        OnMove?.Invoke(deltaTime);
        LogicOutPort.Ping();
        LogicInPort.Ping();
        if (splineItems.Count > 0 && splineItems[0].value == 1)
        {
            OutTransferSlot=splineItems[0].slot;
            LogicOutPort.transferSlot=OutTransferSlot;
        }

        
        if (splineItemsQueue.Count == 0)
            return false;
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
            if(splineItems.Last().slot.Id==id)
                res=splineItems.Last().slot.AddItem(amount);
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
}