using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class CodeCheker : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public string verifyUrl = "http://localhost:5000/verify_code";
    
    public void OnVerifyButtonClick()
    {
        string code = codeInputField.text.Trim().ToUpper();
        if(string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Поле кода пустое!");
            return;
        }
        if (code == "123")
        {
            GameSessionData.ActiveCode = code;
            SceneController.Instance.LoadTestEnvironment();
        }
        StartCoroutine(VerifyCodeCoroutine(code));
    }

    IEnumerator VerifyCodeCoroutine(string code)
    {
        Debug.Log($"Начинаем верификацию кода: {code}");
        
        using UnityWebRequest request = new UnityWebRequest(verifyUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(
            System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new CodeData { code = code }))
        );
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Ошибка верификации: {request.error}\nОтвет сервера: {request.downloadHandler.text}");
            yield break;
        }

        // Сохраняем код и запускаем загрузку сцен
        GameSessionData.ActiveCode = code;
        Debug.Log($"Код {code} верифицирован успешно! Загружаем сцены...");
        
        // Проверяем инициализацию SceneController
        if(SceneController.Instance == null)
        {
            Debug.LogError("SceneController не найден! Создайте объект со скриптом SceneController на сцене.");
            yield break;
        }
        
        SceneController.Instance.LoadTestEnvironment();
    }

    [System.Serializable]
    private class CodeData { public string code; }
}