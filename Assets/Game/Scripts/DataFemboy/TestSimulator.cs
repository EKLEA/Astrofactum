using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestSimulator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int minScore = 0;
    [SerializeField] private int maxScore = 100;
    [SerializeField] private float minTime = 5f;
    [SerializeField] private float maxTime = 30f;

    [Header("UI References")]
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private AttemptSender attemptSender; // Ссылка через инспектор

    private Button _button;

    private void Awake()
    {
        // Если не задан в инспекторе, находим автоматически
        if (attemptSender == null)
        {
            attemptSender = FindAnyObjectByType<AttemptSender>();
        }
    }

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SimulateTestCompletion);
    }

    public void SimulateTestCompletion()
    {
        int randomScore = Random.Range(minScore, maxScore + 1);
        float randomDuration = Random.Range(minTime, maxTime);

        if (debugText != null)
        {
            debugText.text = $"Тест завершен!\nБаллы: {randomScore}\nВремя: {randomDuration:F1}с";
        }

        if (attemptSender != null)
        {
            attemptSender.SendTestResults(randomScore, randomDuration);
        }
        else
        {
            Debug.LogError("AttemptSender не найден!");
        }
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(SimulateTestCompletion);
        }
    }
}