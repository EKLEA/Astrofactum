using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class MovableList<T>
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
    
    public event Action<T> OnItemInListOnEnd;
    public event Action OnItemMoved;
    public event Action OnFirstItemNone;
    public T[] Values{get { return Values; } }
    T[] _values;
    public MovableList(int maxCount)
    {
       _maxCount = maxCount;
        _values = new T[maxCount];
        _curCount = 0;
    }
    public T this[int index]
    {
        get{return _values[index];}
    }
    public int IndexOf(T item)
    {
        return Array.IndexOf(_values,item);
    }
     public bool Add(T item)
    {
        if (EqualityComparer<T>.Default.Equals(_values[0], default))
        {
            _values[0] = item;
            _curCount++;
            return true;
        }
        return false;
    }

    public T Remove()
    {
        if (_curCount == 0)
            return default;
        T t=_values[_curCount - 1] ;
        _values[_curCount - 1] = default;
        _curCount--;
        return t;
    }

    public bool Contains(T item)
    {
        return Array.Exists(_values, x => EqualityComparer<T>.Default.Equals(x, item));
    }

    public bool Move()
    {
       
        if (_curCount == 0)
            return false;
        if(IndexOf(default)!=-1)
        {
           int lastIndex= Array.FindLastIndex(_values, x => EqualityComparer<T>.Default.Equals(x, default(T)));
           for(int i=0;i<lastIndex;i++)
           {
               _values[i+1]=_values[i];
           }
           _values[0]=default;
           OnFirstItemNone?.Invoke();
           OnItemMoved?.Invoke();
           if(!EqualityComparer<T>.Default.Equals(_values[_maxCount-1], default)) OnItemInListOnEnd?.Invoke(_values[_maxCount-1]);
            return true;
            
        }
        else
        {
            return false;
        }
    }
}