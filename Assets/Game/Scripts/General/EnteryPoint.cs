using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnteryPoint : MonoBehaviour
{
    public TextAsset recipeJson;
    public UIManager uIManager;
    public TextAsset enumImageJson;
    public WorldController worldController;

    private IEnumerator Start()
    {
        // Инициализация базовых систем
        RecipeManager.Init(recipeJson);
        InfoDataBase.InitBases();
        BuildingsImagesManager.LoadImages(enumImageJson);
        worldController.Init();

        // Ждем полной загрузки всех сцен
        yield return new WaitUntil(() => SceneController.AllScenesLoaded);

        // Поиск TestController в основной сцене (для дебага)
        TestController testController = FindFirstObjectByType<TestController>();
        if (testController != null)
        {
            testController.Init();
            Debug.Log("TestController инициализирован из основной сцены!");
            uIManager.Init();
            yield break;
        }
        
        // Поиск в аддитивных сценах (Запасной вариант короче)
        testController = FindInAdditiveScenes();
        if (testController != null)
        {
            testController.Init();
            Debug.Log("TestController инициализирован из аддитивной сцены!");
        }
        else
        {
            Debug.LogWarning("TestController не найден ни в одной сцене");
        }

        uIManager.Init();
    }

    private TestController FindInAdditiveScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == gameObject.scene) continue; // Пропускаем основную сцену

            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                var controller = obj.GetComponentInChildren<TestController>(true);
                if (controller != null) return controller;
            }
        }
        return null;
    }
}