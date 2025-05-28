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
    public GameObject[] GuideLists;
    int currI;
    public void Change(int c)
    {
        currI +=c;
        if (currI >= GuideLists.Length) currI = 0;
        else if(currI<0) currI = GuideLists.Length-1;
        foreach (var guide in GuideLists) guide.gameObject.SetActive(false);
        GuideLists[currI].gameObject.SetActive(true);
    }
    public override void Disable()
    {
        if(gameObject.activeSelf)
        {
            worldController.UnPause();
            base.Disable();
        }
    }
    public override void Enable()
    {
        worldController.Pause();
        foreach (var guide in GuideLists) guide.gameObject.SetActive(false);
        GuideLists[currI].gameObject.SetActive(true);
        base.Enable();
    }
}