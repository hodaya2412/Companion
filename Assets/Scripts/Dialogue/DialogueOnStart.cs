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

        
        yield return null;
        yield return null;

        
        yield return new WaitForSeconds(delay);

        Begin();
    }

    void Begin()
    {
        
        GameEvents.OnDialogueRequested?.Invoke(dialogue);

        
        if (tutorialStep != null && CompanionTutorial.Instance != null)
        {
            CompanionTutorial.Instance.ShowStep(tutorialStep);
        }
    }
}