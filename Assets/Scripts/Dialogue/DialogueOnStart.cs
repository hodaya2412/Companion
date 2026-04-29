using UnityEngine;
using System.Collections;

public class DialogueOnStart : MonoBehaviour
{
    public DialogueAsset dialogue;
    public float delay = 0.3f;

    [Header("Optional Tutorial")]
    [Tooltip("גררי לכאן את ה-ScriptableObject של הטוטוריאל אם את רוצה שיופיע פופ-אפ")]
    public TutorialStepSO tutorialStep;

    private IEnumerator Start()
    {
        if (dialogue == null) yield break;

        string id = string.IsNullOrWhiteSpace(dialogue.dialogueId) ? dialogue.name : dialogue.dialogueId;

        bool alreadyPlayed = GameEvents.RequestFlagState?.Invoke(id) ?? false;
        if (alreadyPlayed) yield break;

        // מחכה שכל המערכות יעלו
        yield return null;
        yield return null;

        // הדיליי שלך
        yield return new WaitForSeconds(delay);

        Begin();
    }

    void Begin()
    {
        // 1. הפעלת הדיאלוג
        GameEvents.OnDialogueRequested?.Invoke(dialogue);

        // 2. הפעלת הפופ-אפ של היצור (רק אם הוגדר כזה ב-Inspector)
        if (tutorialStep != null && CompanionTutorial.Instance != null)
        {
            CompanionTutorial.Instance.ShowStep(tutorialStep);
        }
    }
}