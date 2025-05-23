using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnteryPoint : MonoBehaviour
{
    public TextAsset recipeJson;
    public UIManager uIManager;
    public TextAsset enumImageJson;
    public WorldController worldController;

    void Start()
    {
        StartCoroutine(FindScene());
        
        RecipeManager.Init(recipeJson);
        InfoDataBase.InitBases();
        BuildingsImagesManager.LoadImages(enumImageJson);
        worldController.Init();
        uIManager.Init();
    }
    IEnumerator FindScene()
    {
        yield return new WaitUntil(() => SceneController.AllScenesLoaded);
        TestController testController = FindFirstObjectByType<TestController>();
        
        testController = FindInAdditiveScenes();
        Debug.Log(testController);
        if (testController != null)
        {
            testController.Init();
            Debug.Log("TestController инициализирован из аддитивной сцены!");
        }
        else
        {
            Debug.LogWarning("TestController не найден ни в одной сцене");
        }
    }
    private TestController FindInAdditiveScenes()
    {
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == gameObject.scene) continue;
            GameObject[] rootObjects = scene.GetRootGameObjects();
            Debug.Log(rootObjects.Length);
            foreach (var obj in rootObjects)
            {
                Debug.Log(obj.name);
                var controller = obj.GetComponentInChildren<TestController>(true);
                if (controller != null) return controller;
            }
        }
        return null;
    }
}