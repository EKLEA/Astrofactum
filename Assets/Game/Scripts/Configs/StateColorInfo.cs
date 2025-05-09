using UnityEngine;

[CreateAssetMenu(fileName = "StateColorSettings", menuName = "Settings/State Colors")]
public class StateColorSettings : ScriptableObject
{
    [Header("State Colors")]
    [Tooltip("Цвет для состояния 'Processed'")]
    public Color processedColor = new Color(0f, 1f, 0f, 1f);

    [Tooltip("Цвет для состояния 'WaitingExit'")]
    public Color waitingExitColor = new Color(1f, 1f, 0f, 1f); 

    [Tooltip("Цвет для состояния 'WaitingEntry'")]
    public Color waitingEntryColor = new Color(1f, 0f, 0f, 1f);

    [Tooltip("Цвет по умолчанию")]
    public Color defaultColor = Color.white;
}