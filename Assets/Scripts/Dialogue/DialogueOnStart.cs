using UnityEngine;

public class DialogueOnStart : MonoBehaviour
{
    public DialogueAsset dialogue;
    public float delay = 0.2f;

    [Header("Play only once")]
    public string onceFlagKey = "CastleIntroPlayed"; // תני שם ברור

    void Start()
    {
        if (dialogue == null) return;

        // אם כבר נוגן פעם אחת - לא מפעילים שוב
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(onceFlagKey))
            return;

        Invoke(nameof(Begin), delay);
    }

    void Begin()
    {
        // מסמנים שכבר נוגן
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetFlag(onceFlagKey, true);

        DialogueManager.Instance.StartDialogue(dialogue);
    }
}