using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AttemptSender : MonoBehaviour
{
    public string saveAttemptUrl = "http://localhost:5000/save_attempt_from_game";

    public void SendTestResults(int score, double duration)
    {
        StartCoroutine(SendAttemptCoroutine(score, duration));
    }

    IEnumerator SendAttemptCoroutine(int score, double duration)
    {
        if (string.IsNullOrEmpty(GameSessionData.ActiveCode))
        {
            Debug.LogError("Код теста не найден!");
            yield break;
        }

        // Сохраняем код перед очисткой
        string currentCode = GameSessionData.ActiveCode;

        using UnityWebRequest request = new UnityWebRequest(saveAttemptUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(
            System.Text.Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(new AttemptData { 
                    code = currentCode, // Используем сохраненный код
                    score = score,
                    duration = duration
                })
            )
        );
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            GameSessionData.ClearCode();
            Debug.Log("Результаты успешно сохранены!");
        }
        else
        {
            Debug.LogError($"Ошибка сохранения: {request.downloadHandler.text}");
        }
    }

    [System.Serializable]
    private class AttemptData { 
        public string code;
        public int score;
        public double duration;
    }
}