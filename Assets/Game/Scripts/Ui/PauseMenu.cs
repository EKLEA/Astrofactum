using UnityEngine;

public class PauseMenu : UIController
{
    public override void Enable()
    {
        base.Enable();
        WorldController.Instance.Pause();
    }
    public override void Disable()
    {
        base.Disable();
    }
    public void Continue()
    {
        Disable();
        WorldController.Instance.UnPause();
    }
    public void ReturnToMenu()
    {
        SceneController.Instance.ReturnToMainMenu();
    }
}
