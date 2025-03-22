using System;
using UnityEngine;

public class Slot 
{
    public string id;
    public bool AddItem()
    {
        if (_count+1<=_maxCount)
        {
            _count++;
            IsEmpty=false;
            if(_count<_maxCount) IsFull=false;
            else IsFull=true;
            
            return true;
        }
        else return false;
    }
    public bool RemoveItem()
    {
        if (_count-1>=0)
        {
            _count--;
            if(_count==0) IsEmpty=true;
            return true;
        }
        else return false;
    }
    public bool IsFull;
    public bool IsEmpty;
    public void SetUpMaxCount(int newMax)
    {
        _maxCount=newMax;
    }
    int _count,_maxCount;
    
}