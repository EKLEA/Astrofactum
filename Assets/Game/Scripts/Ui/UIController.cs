using UnityEngine;

public class UIController : MonoBehaviour
{
    public virtual void Init(){ }
    public virtual void Init(string id){ }
    public virtual void InvokeMethod(string id,ActionButton button){}
   
}