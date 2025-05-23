// SceneController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string baseScene = "BaseScene";
    [SerializeField] private string testScene = "TestScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("SceneController initialized");
    }

    public void LoadTestEnvironment() => StartCoroutine(LoadTestEnvironmentRoutine());
    public static bool AllScenesLoaded { get; private set; }

    private IEnumerator LoadTestEnvironmentRoutine()
    {
        AllScenesLoaded = false;
        Debug.Log("Starting scene loading process...");

        // Загружаем базовую сцену
        yield return LoadSceneSingle(baseScene);

        // Загружаем тестовую сцену аддитивно
        yield return LoadSceneAdditive(testScene);

        AllScenesLoaded = true;
        Debug.Log("All scenes loaded successfully");
    }

    private IEnumerator LoadSceneSingle(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;
        
        while (!asyncOp.isDone)
        {
            Debug.Log($"Loading progress: {asyncOp.progress * 100}%");
            yield return null;
        }
    }

    private IEnumerator LoadSceneAdditive(string sceneName)
    {
        Debug.Log($"Adding scene: {sceneName}");
        var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncOp.allowSceneActivation = true;
        
        while (!asyncOp.isDone)
        {
            Debug.Log($"Additive loading progress: {asyncOp.progress * 100}%");
            yield return null;
        }
        
        // Активируем загруженную сцену
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}