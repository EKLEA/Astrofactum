using System;
using UnityEngine;

public class Slot 
{
    public string Id{get{return _id;}}
    public int Count{get{return _count;}}
    public int MaxCount{get{return _maxCount;}}
    public bool IsFull;
    public bool IsEmpty;
    string _id;
    int _count,_maxCount;
    public Slot(string id):this(id,999,0){}
    public Slot(string id,int maxCoun):this(id,maxCoun,0){}
    public Slot(string id,int maxCount, int count)
    {
        if(!string.IsNullOrWhiteSpace(id)) _id=id;
        if(maxCount>0) _maxCount=maxCount;//потом добавть конструкцию по умолчанию ссылаясь на джосн
        if(count<maxCount&&count>=0) _count=count;
    }
    public int AddItem(int value)
    {
        if(_count+value>_maxCount) //5+4>8
        {
            var r=_count+value-_maxCount;//1
            _count=_maxCount;
            return value-r;//добаили 3
        }
        {
            _count+=value;
            return value;//добаили все
        }
    }
    public int RemoveItem(int value)
    {
        
        if(_count-value<0) //4-5<0
        {
            var r=_count;;//4
            _count=0;
            return _count;//удалили 4
        }
        {
            _count-=value;
            return value;////удалили все
        }
    }
}