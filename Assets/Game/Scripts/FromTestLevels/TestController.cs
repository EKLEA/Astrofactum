using UnityEngine;

public class TestController : MonoBehaviour
{

    [Header("Настройки генераторов")]
    public ResourceGeneratorPair[] resourceGenerators;

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
                pair.generator.SetUpLogic();
                pair.generator.SetUpReciepe(pair.recipeID);
                pair.generator.canChange = false;
                pair.generator.canDestroy = false;
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
