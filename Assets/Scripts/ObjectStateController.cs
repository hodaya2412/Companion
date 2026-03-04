using UnityEngine;

public class ObjectStateController : MonoBehaviour
{
    public string flagKey;    // למשל: "Forest_PuzzleSolved"
    public bool activeIfTrue; // האם האובייקט פעיל כשהדגל אמת?

    private void OnEnable()
    {
        // הרשמה לאירוע הגלובלי - אין צמידות לאובייקט ספציפי
        GameEvents.OnFlagChanged += HandleFlagChanged;
        Refresh();
    }

    private void OnDisable()
    {
        GameEvents.OnFlagChanged -= HandleFlagChanged;
    }

    private void HandleFlagChanged(string changedKey, bool newValue)
    {
        if (changedKey == flagKey) Refresh();
    }

    public void Refresh()
    {
        if (GameStateManager.Instance == null) return;
        bool flagValue = GameStateManager.Instance.GetFlag(flagKey);
        gameObject.SetActive(activeIfTrue ? flagValue : !flagValue);
    }
}