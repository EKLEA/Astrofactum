using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : Button
{
    [SerializeField] public TextMeshProUGUI textMeshPro=> GetComponentInChildren<TextMeshProUGUI>(); 
    private string _id;
    private UIController _controller;

    public void SetUpButton(string id, string title, Sprite sprite,UIController controller)
    {
        
        _id = id;
        textMeshPro.text=title;
        image.sprite=sprite;
        _controller = controller;
        onClick.AddListener(InvokeInfo);
    }

    void InvokeInfo()
    {
        _controller.InvokeMethod(_id, this);
    }
}
