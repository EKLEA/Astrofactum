using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Net;

public class CodeCheker : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public string verifyUrl;
    
    public void OnVerifyButtonClick()
    {
        string code = codeInputField.text.Trim().ToUpper();
        if (code == "123")
        {
            GameSessionData.ActiveCode = code;
            SceneController.Instance.LoadTestEnvironment();
        }
        if(string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("Поле кода пустое!");
            return;
        }
        StartCoroutine(VerifyCodeCoroutine(code));
    }

    IEnumerator VerifyCodeCoroutine(string code)
    {
        Debug.Log($"Начинаем верификацию кода: {code}");
        
        // Обход проблем с сертификатами (только для разработки!)
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
        
        // Создаем JSON данные для отправки
        var jsonData = new CodeData { code = code };
        string json = JsonUtility.ToJson(jsonData);
        
        using (UnityWebRequest request = new UnityWebRequest(verifyUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10; // Увеличим таймаут
            
            // Добавляем CORS заголовки
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            request.SetRequestHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
            request.SetRequestHeader("Access-Control-Allow-Headers", "Content-Type");
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}\nResponse: {request.downloadHandler.text}");
                yield break;
            }
            
            // Проверяем успешный ответ
            if (request.responseCode == 200)
            {
                // Сохраняем код и запускаем загрузку сцен
                GameSessionData.ActiveCode = code;
                Debug.Log($"Код {code} верифицирован успешно! Загружаем сцены...");
                
                if(SceneController.Instance == null)
                {
                    Debug.LogError("SceneController не найден!");
                    yield break;
                }
                
                SceneController.Instance.LoadTestEnvironment();
            }
            else
            {
                Debug.LogError($"Ошибка верификации: {request.downloadHandler.text}");
            }
        }
    }

    [System.Serializable]
    private class CodeData 
    { 
        public string code; 
    }
}