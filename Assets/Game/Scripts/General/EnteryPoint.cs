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

        RecipeManager.Init(recipeJson);
        InfoDataBase.InitBases();
        BuildingsImagesManager.LoadImages(enumImageJson);
        worldController.Init();

		//удобно для сцен дебаговых
        TestController testController = Object.FindFirstObjectByType<TestController>();
        if (testController != null)
        {
            testController.Init();
            Debug.Log("TestController инициализирован из основной сцены!");
            uIManager.Init();
            yield break;
        }

        //НУ НАЙДИ ТЫ ЕГО ЧЕЕЕЕЛ ГОСПОДИ
        if (SceneManager.sceneCount > 1)
        {
            Scene testScene = SceneManager.GetSceneAt(1);
            yield return new WaitUntil(() => testScene.isLoaded);

            GameObject[] rootObjects = testScene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                testController = obj.GetComponentInChildren<TestController>(true);
                if (testController != null)
                {
                    testController.Init();
                    Debug.Log("TestController инициализирован из аддитивной сцены!");
                    break;
                }
            }
        }

        if (testController == null)
        {
            Debug.LogWarning("TestController не найден ни в одной сцене");
        }

        uIManager.Init();
    }
}