using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmSctructure
{
   public event Action endOfBuildingStructure;
   public event Action endOfColItems;
   public int countOfReqDrones{ get;  }
   public float timeOfBuild{ get;}
   public float currentTimeOfBuild{ get;  }
   public bool isEnouhtItems { get;}
   public IAmDronNetworkPart closestDronNetworkPart{ get;  }
   public Dictionary<Item,int> itemsToBuild{ get; }
   public Dictionary<Item,int> currentItemsToBuild{ get; }
   public virtual void StructureBuild(){}
   public virtual void RednderStucture(float currentTimeOfBuild){}
   public virtual void AddPoint(Vector3 pos,params string[] buildigns){}
   public virtual void AddPoints(List<Vector3> points,params string[] buildigns){}
}
