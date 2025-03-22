using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public interface IAmSctructure
{
   public event Action endOfBuildingStructure;
   public event Action endOfColItems;
   public int countOfReqDrones{ get;  }
   public float currentTimeOfBuild{ get;  }
   public bool isEnouhtItems { get;}
   public IAmDronNetworkPart closestDronNetworkPart{ get;  }
   public List<Slot> currentItemsToBuild{ get; }
   public virtual void StructureBuild(){}
   public virtual void RednderStucture(float currentTimeOfBuild){}
   public virtual void AddPoint(Vector3 pos,(float,string) buildigns){}
   public virtual void AddPoint(Vector3 pos){}
   public virtual void AddPoints(List<Vector3> points,params (float,string)[] buildigns){}
}
