using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class CodeVerifier : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public string verifyUrl = "http://localhost:5000/verify_code";
    
    public void OnVerifyButtonClick()
    {
        string code = codeInputField.text.Trim().ToUpper();
        StartCoroutine(VerifyCodeCoroutine(code));
    }

    IEnumerator VerifyCodeCoroutine(string code)
    {
        using UnityWebRequest request = new UnityWebRequest(verifyUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(
            System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new CodeData { code = code }))
        );
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Ошибка верификации: {request.downloadHandler.text}");
            yield break;
        }

        // Сохраняем код и загружаем сцену с тестом
        GameSessionData.ActiveCode = code;
        Debug.Log($"Код {code} сохранен, запускаем тест...");
        // Здесь загрузка сцены с тестом: SceneManager.LoadScene("TestScene");
    }

    [System.Serializable]
    private class CodeData { public string code; }
}