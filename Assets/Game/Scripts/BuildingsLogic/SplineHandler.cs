using System.Linq;
using UnityEngine;

public class SplineHandler : BuildingLogicBase, IHavePorts
{
    public Port[] inPorts {get{return _inPorts;}}
    public Port[] OutPorts {get{return _outPorts;}}
    [SerializeField] Port[] _inPorts;
    [SerializeField] Port[] _outPorts;
    public void Awake()
    {
    }
    public virtual void AddItemToBuilding(Item item)
    {
        RemoveItemFromBuilding(item);
    }
    public virtual void RemoveItemFromBuilding(Item item)
	{
	    //передвижение по сплайну
	}
}
