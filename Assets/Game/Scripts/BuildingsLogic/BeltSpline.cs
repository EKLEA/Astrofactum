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
            return splineItems.Where(f=>f.value>=1).Select(f=>f.slot).ToList();
        }
    }

    public float CurrentProcent => throw new NotImplementedException();

    public bool CanAdd {get{return splineItems.Count == 0 || splineItemsQueue.Count>0&&splineItems.Last().value > (1f / countOfSegments);}}

    [SerializeField] SplineItem prefab;
    
    
    int countOfSegments;
    bool canMove;
    bool isWisible;
    public event Action<string> OnItemsCanAdded;
    public event Action<string> OnItemsCanRemoved;
    event Action<float> OnMove;
    event Action OnVisualUpdate;
    List<SplineItem> splineItems=new();
    Queue<SplineItem> splineItemsQueue=new();
    public bool IAmSetUped {get{return _isAmSetUped;}}
    bool _isAmSetUped;
    public override void Init(string tid)
    {
        base.Init(tid);
        _isWork = false;
    }
    public void Ping()
    {
        if(splineItems.Count==0) return;    
        if(splineItems[0]!=null&&splineItems[0].value==splineItems[0].target&&splineItems[0].target==1) InvokeCanRemoved(splineItems[0].slot.Id);
        if(splineItems.Last().value>(1/countOfSegments)||splineItemsQueue.Count!=0)
        {
            InvokeCanAdded(null);
        }
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
        Ping();
    }
    void Update()
    {
        OnVisualUpdate?.Invoke();
    }
    public void Tick(float deltaTime)
    {
       
       canMove=Move(deltaTime);
    }
    bool Move(float deltaTime)
    {
       
        Debug.Log(deltaTime);
        OnMove?.Invoke(deltaTime);

        
        if (splineItems.Count > 0 && splineItems[0].value == 1)
        {
            InvokeCanRemoved(splineItems[0].slot.Id);
        }

        
        if (splineItemsQueue.Count == 0)
            return false;

       
        if (splineItems.Count == 0 || splineItemsQueue.Count>0&&splineItems.Last().value > (1f / countOfSegments))
        {
            InvokeCanAdded(null);
            return true;
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
            if(splineItems.Last().slot.Id==id)
                res=splineItems.Last().slot.AddItem(amount);
        }
       return new SlotTransferArgs(id,res);
    }

    public SlotTransferArgs RemoveFromBuilding(string id, int amount)
    {
        int max=InfoDataBase.itemInfoBase.GetInfo(id).maxCountInPack;
        int res=0;
        if(splineItems.Count>0&&splineItems[0].slot.Id==id&&splineItems[0].value>=1)
        {
           res=splineItems[0].slot.RemoveItem(amount);
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
        return new SlotTransferArgs(id,res);
    }
    

    public void InvokeCanAdded(string id)
    {
        OnItemsCanAdded?.Invoke(id);
    }

    public void InvokeCanRemoved(string id)
    {
        OnItemsCanRemoved?.Invoke(id);
    }
    public override void UpdateInPort(Port port)
    {
        port.PortUpdateTo(this);
    }
    public override void UpdateOutPort(Port port)
    {
        port.PortUpdateFrom(this);
    }
}