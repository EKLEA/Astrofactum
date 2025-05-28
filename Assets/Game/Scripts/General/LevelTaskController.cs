using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LevelTaskController : UIController
{
    public event Action<int> OnTaskDone;
    public TaskConfig config;
    public UIItem Item;
    public static LevelTaskController Instance;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI Score;
    public Transform grid;
    int score;
    UIItem[] uiItemPool = new UIItem[10];
    Dictionary<string, int> currItems = new();
    Dictionary<string, UIItem> linkedUIItems = new();
    void Awake()
    {
        for (int i = 0; i < uiItemPool.Length; i++)
        {
            var gm = Instantiate(Item, grid);
            gm.gameObject.SetActive(false);
            uiItemPool[i] = gm;
        }
        Instance = this;
    }

    public void SetUpTask()
    {
        score = config.score;
        for (int i = 0; i < uiItemPool.Length; i++)
        {
            uiItemPool[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < config.taskItems.Length; i++)
        {
            var item = InfoDataBase.itemInfoBase.GetInfo(config.taskItems[i].id);
            uiItemPool[i].image.sprite = item.icon;
            uiItemPool[i].text.text = item.title;
            uiItemPool[i].Amount.text = string.Format("0/{0}", config.taskItems[i].amount);
            uiItemPool[i].gameObject.SetActive(true);
            currItems.Add(config.taskItems[i].id, 0);
            linkedUIItems.Add(config.taskItems[i].id, uiItemPool[i]);
        }
        Score.text = "Очки: " + score.ToString();

    }
    public void AddItem(string id, int amount)
    {
        if (currItems.ContainsKey(id) && linkedUIItems.ContainsKey(id) && amount > 0)
        {
            currItems[id] += amount;

            string[] parts = linkedUIItems[id].Amount.text.Split('/');
            linkedUIItems[id].Amount.text = $"{currItems[id]}/{parts[1]}";
        }

        RemoveScore(amount);
        if (CheckWin()) OnTaskDone?.Invoke(score);
    }
    public void RemoveScore(int s)
    {
        score -= s;
        Score.text = "Очки: " + score.ToString();
    }
    bool CheckWin()
    {
        for (int i = 0; i < config.taskItems.Length; i++)
        {
            if (currItems[config.taskItems[i].id] < config.taskItems[i].amount) return false;
        }
        return true;
    }
}