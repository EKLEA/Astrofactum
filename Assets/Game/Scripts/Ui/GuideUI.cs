using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GuideUI : UIController
{
    WorldController worldController => WorldController.Instance;
    public override void Disable()
    {
        worldController.UnPause();
        base.Disable();
    }
    public override void Enable()
    {
        worldController.Pause();
        base.Enable();
    }
}