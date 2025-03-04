using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Search;
using UnityEngine;

public class LimetedList<T>
{

    int _maxCount;
    int _curCount;
    public int Lenght
    {
        get { return _maxCount; }
    }
    public int Count
    {
        get { return _curCount; }
    }
    public event Action OnItemInListOnEnd;
    public event Action OnItemMoved;
    public List<T> Values{get { return Values; } }
    List<T> values;
    public LimetedList(int maxCount)
    {
        _maxCount = maxCount;
        for (int i = 0; i < _maxCount; i++)
            values[i]=default;
    }
    public int IndexOf(T item)
    {
        return values.IndexOf(item);
    }
    public bool Add(T item)
    {
        if(_curCount==_maxCount||!values[0].Equals(default))
            return false;
        else
        {
            values[0]=item;
            _curCount++;
            return true;
        }
    }
    public bool Contains(T item)
    {
        return values.Contains(item);
    }
    public bool Move()
    {
        try
        {
            values.RemoveAt(values.FindLastIndex(default));
            values.Insert(0,default);
            OnItemMoved?.Invoke();
            if(!values[values.Count-1].Equals(default)) OnItemInListOnEnd?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            return false;
        }
    }
}