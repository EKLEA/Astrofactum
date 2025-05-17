using UnityEngine;

public class PortArrow:MonoBehaviour
{
    public Material material;
    public void Disable()
    {
        gameObject.SetActive (false);
    }
    public void Enable()
    {
        gameObject.SetActive (true);
    }
}