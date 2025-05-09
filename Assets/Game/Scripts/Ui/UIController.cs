using UnityEngine;

public class UIController : MonoBehaviour
{
    public virtual void Init(){ }
    public virtual void Init(string id){ }
    public virtual void InvokeMethod(string id,ActionButton button){}
    public virtual void Disable(){gameObject.SetActive(false);}
    public virtual void Enable(){gameObject.SetActive(true);}
   
}