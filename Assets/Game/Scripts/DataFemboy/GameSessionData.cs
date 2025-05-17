using UnityEngine;

public static class GameSessionData
{
    private static string _activeCode;
    
    public static string ActiveCode {
        get => _activeCode;
        set {
            _activeCode = value?.Trim().ToUpper();
            Debug.Log($"Active code set to: {_activeCode}");
        }
    }

    public static void ClearCode() => _activeCode = null;
}