using UnityEngine;

public class TestController : MonoBehaviour
{
    public static TestController Instance { get; private set; }

    [Header("Настройки генераторов")]
    public ResourceGeneratorPair[] resourceGenerators;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        Debug.Log("Инициализация генераторов...");
        
        foreach (var pair in resourceGenerators)
        {
            if (pair.generator == null)
            {
                Debug.LogError("Объект генератора не назначен!");
                continue;
            }

            try
            {
                pair.generator.Init("drill");
                pair.generator.SetUpReciepe(pair.recipeID);
                pair.generator.canChange = false;
                Debug.Log($"Генератор {pair.generator.name} готов к работе");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка инициализации генератора: {e.Message}");
            }
        }
    }
}

[System.Serializable]
public class ResourceGeneratorPair
{
    public ResourceGenerator generator;
    public string recipeID;
}
