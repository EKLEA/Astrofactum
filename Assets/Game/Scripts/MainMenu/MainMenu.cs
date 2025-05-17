using UnityEngine;

public class MainMenu : MonoBehaviour
{
    

    public void QuitApp(){
        Application.Quit();
        Debug.Log("\n再见！");
    }

    public void StartTesting()
    {
        
    }

    public void ToSite(string link){
        Application.OpenURL(link);
        Debug.Log("\n网上！");
    }
}
